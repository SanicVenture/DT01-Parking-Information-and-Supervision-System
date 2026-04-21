import 'dart:async';
import 'dart:io';
import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:informationalapplication/httpmanager.dart';
import 'package:window_manager/window_manager.dart';
import 'package:shared_preferences/shared_preferences.dart';


class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback = (X509Certificate cert, String host, int port) => true;
  }
}

bool doneRunning = false;
 void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  if (!isDesktop)
  {  SystemChrome.setPreferredOrientations([DeviceOrientation.portraitUp]);}

  if (isDesktop) {
    windowManager.ensureInitialized();
    WindowOptions windowOptions = const WindowOptions (
      fullScreen: true,
      center: true,
    );
    windowManager.waitUntilReadyToShow(windowOptions, () async {
      await windowManager.show();
      await windowManager.focus();
    });
  }

  HttpOverrides.global = MyHttpOverrides(); // Apply the override
  runApp(MyApp());

  final SharedPreferences prefs = await SharedPreferences.getInstance();
  bool hasKey = prefs.containsKey('remoteIP');

  if (!hasKey) {
    await prefs.setString('remoteIP', remoteUrl);
  }
  else {
    String? storedIP = prefs.getString('remoteIP');
    if (storedIP != null) {
      remoteUrl = storedIP;
    }
  }

  doneRunning = true;
}

class MyApp extends StatelessWidget {
  MyApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    final double textScaleFactor = isDesktop ? 4.5 : 1;

    return MediaQuery(
      data: MediaQuery.of(context).copyWith(textScaler: TextScaler.linear(textScaleFactor)),
      child: MaterialApp(
        debugShowCheckedModeBanner: false,
        title: 'Flutter Demo',
        theme: ThemeData(
          colorScheme: .fromSeed(seedColor: const Color.fromARGB(255, 255, 255, 255), brightness: Brightness.dark),
          textTheme: GoogleFonts.overpassTextTheme(Theme.of(context).textTheme)
        ),
        home: MyHomePage(title: 'Parking Lot Entrance Information'),
      )
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}


bool shownOnce = false;

class IPRoute extends StatefulWidget {
  IPRoute({super.key});
  
  @override
  State<StatefulWidget> createState() => _IPRouteState();
}

class _IPRouteState extends State<IPRoute> {
  String selectedvalue4 = remoteUrl.substring(0, remoteUrl.length - 4);
  late final TextEditingController _controller;

  @override
  void initState() {
    super.initState();
    _controller = TextEditingController(text: selectedvalue4);
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {

        //     theme: ThemeData(
        //   colorScheme: .fromSeed(seedColor: const Color.fromARGB(255, 255, 255, 255), brightness: Brightness.dark),
        //   textTheme: GoogleFonts.overpassTextTheme(Theme.of(context).textTheme)
        // ),
    return Scaffold(
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            Text('Select Parking Space Id:', style: TextStyle(color: Theme.of(context).colorScheme.onSurface),),
            TextField(
              style: TextStyle(color: Theme.of(context).colorScheme.onSurface),
              controller: _controller,
              keyboardType: TextInputType.text,
              onChanged: (value) {
                setState(() {
                  selectedvalue4 = value;
                });
              },
            ),
            ElevatedButton(
              onPressed: () {
                remoteUrl = selectedvalue4 + '/api';
                saveIP();
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => const MyHomePage(title: 'Home')),
                );
              },
              child: const Text('Set IP Address'),
            ),
          ],
        )
      ),
    );
  }
}

void saveIP() async {
  final SharedPreferences prefs = await SharedPreferences.getInstance();
  prefs.setString('remoteIP', remoteUrl);
}

class _MyHomePageState extends State<MyHomePage> {
  Timer? _timer;
  late Future<List<ParkingSpace>> futureParkingSpaces;
  var oldSpaces;
  bool failure = false;


  @override
  void initState() {
    super.initState();
    futureParkingSpaces = fetchParkingSpaces();
    _timer = Timer.periodic(const Duration(seconds: 5), (Timer t) {
      setState(() {
        futureParkingSpaces = fetchParkingSpaces();
        WidgetsBinding.instance.addPostFrameCallback((_) {
          if (failure && !isDesktop) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('Error fetching data, showing last successful data'),
                duration: Duration(seconds: 8),
              ),
            );
          }
        });
      });
    });

    if (!isDesktop && !shownOnce)
    {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        Navigator.push(context,
          MaterialPageRoute(builder: (context) => IPRoute()),
        );    
        shownOnce = true;
      });
    }

  }

  

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  void _updateParkingSpots() {
    setState(() {
      futureParkingSpaces = fetchParkingSpaces();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      // backgroundColor: Theme.of(context).colorScheme.primaryContainer,
      appBar: !isDesktop ? AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(widget.title),
      ) : null,

      body: FutureBuilder<List<ParkingSpace>>(
        future: futureParkingSpaces,
        builder: (context, snapshot) {
          if (snapshot.hasData) {
            failure = false;
            final parkingSpaces = snapshot.data!;
            oldSpaces = parkingSpaces;
            final List<List<ParkingSpace>> ByFloorList = [];
            final List<int> AvailablePerFloor = [];
            int maxFloor = 0;
            for (var space in parkingSpaces) {
              if (space.floor > maxFloor) {
                maxFloor = space.floor;
              }
            }

            for (var i = 1; i <= maxFloor; i++) {
              ByFloorList.add(parkingSpaces.where((space) => space.floor == i).toList());
            }

            for (var floor in ByFloorList) {
              final availableCount = floor.where((space) => !space.occupied).length;
              AvailablePerFloor.add(availableCount);
            }

            return ListView.builder(
              itemCount: ByFloorList.length + 3,
              itemBuilder: (context, index) {
                if (index == 2) {
                  return ListTile(
                    title: Text('Open Spots: ${AvailablePerFloor.reduce((a, b) => a + b)}', textAlign: TextAlign.center,),
                  );
                }
                else if (index == 0)
                {
                  return Padding(
                    padding: EdgeInsetsGeometry.symmetric(vertical: isDesktop ? 44 : 22),
                    child: SizedBox(
                      height: isDesktop ? 200 : 100,  // Adjust this value to your desired height (e.g., 50-200 pixels)
                      child: Image.asset(
                        "assets/images/Akron-Zips-Logo-2008.png",
                        fit: BoxFit.fitHeight,  // Scales to fit within the SizedBox while maintaining aspect ratio
                        alignment: Alignment.center,  // Center the image; adjust if needed
                      ),
                    )
                  );
                }
                else if (index == 1)
                {
                  return ListTile(
                    title: Text('Lot 37', textAlign: TextAlign.center,));
                }
                return ListTile(
                  title: Text(
                    'Floor: ${index-2}', textAlign: TextAlign.center,
                    style: TextStyle(color: AvailablePerFloor[index - 3] > 0 ? Colors.green : Colors.red)
                  ),
                  subtitle: 
                  Text(
                    'Open Spots: ${AvailablePerFloor[index - 3]}', 
                    textAlign: TextAlign.center, 
                    style: TextStyle(color: AvailablePerFloor[index - 3] > 0 ? Colors.green : Colors.red),
                  ),
                );

              },
            );
          } else if (snapshot.hasError) {
            if (oldSpaces != null)
            {
              failure = true;
              final List<List<ParkingSpace>> ByFloorList = [];
              final List<int> AvailablePerFloor = [];
              int maxFloor = 0;
              for (var space in oldSpaces) {
                if (space.floor > maxFloor) {
                  maxFloor = space.floor;
                }
              }

              for (var i = 1; i <= maxFloor; i++) {
                ByFloorList.add(oldSpaces.where((space) => space.floor == i).toList());
              }

              for (var floor in ByFloorList) {
                final availableCount = floor.where((space) => !space.occupied).length;
                AvailablePerFloor.add(availableCount);
              }

              return ListView.builder(
                itemCount: ByFloorList.length + 1,
                itemBuilder: (context, index) {
                  if (index == 0) {
                    return ListTile(
                      title: Text('Open Spots: ${AvailablePerFloor.reduce((a, b) => a + b)}'),
                    );
                  }
                  return ListTile(
                    title: Text('Floor: ${index}'),
                    subtitle: 
                    Text(
                      'Open Spots: ${AvailablePerFloor[index - 1]}'
                      ),
                  );
                },
              );              
            }
            else
            {
              return Center(child: Text('${snapshot.error}'));
            }

          }

          // By default, show a loading spinner.
          return const Center(child: CircularProgressIndicator());
        },
      ),

      // floatingActionButton: !isDesktop ? FloatingActionButton(
      //   onPressed: _updateParkingSpots,
      //   tooltip: 'Reload',
      //   child: const Icon(Icons.replay),
      // ) : null,
    );
  }
}
