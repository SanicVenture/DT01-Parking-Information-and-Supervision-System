import 'dart:async';
import 'dart:io';
import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:informationalapplication/httpmanager.dart';
import 'package:window_manager/window_manager.dart';
import 'package:shared_preferences/shared_preferences.dart';

/**This application shows parking lot occupancy information to someone who wants to park in a parking area.
 * This application is fit for desktop and mobile.
 */


 // Here, we are overriding the HttpClient to allow self-signed certificates. This allows us to connect to the
 // Parking Availability Server, which, not being in production, uses a self-signed certificate. By setting the
 // badCertificateCallback to always return true, we are effectively telling the HttpClient to accept all certificates,
 // including self-signed ones. This would be highly unsafe in a production environment.
class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback = (X509Certificate cert, String host, int port) => true;
  }
}

 void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  if (!isDesktop)
  {  SystemChrome.setPreferredOrientations([DeviceOrientation.portraitUp]);} // Force portrait mode on mobile devices


  //Enables fullscreen on desktop platforms
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

  // Attempt to load IP address for Parking Availability Server from shared preferences
  final SharedPreferences prefs = await SharedPreferences.getInstance();
  bool hasKey = prefs.containsKey('remoteIP');

  // If it doesn't exist, create it with the default value.
  if (!hasKey) {
    await prefs.setString('remoteIP', remoteUrl);
  }
  else {
    String? storedIP = prefs.getString('remoteIP');
    if (storedIP != null) {
      remoteUrl = storedIP;
    }
  }
}

class MyApp extends StatelessWidget {
  MyApp({super.key});

  // Root of the application
  @override
  Widget build(BuildContext context) {
    final double textScaleFactor = isDesktop ? 4.5 : 1; //Lazy way to make text bigger on desktop

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

// Used for setting the server IP Address. In a production environment, either the address for the server
// would be preprogrammed, or there would be a user friendly list of parking garages.
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
    _controller = TextEditingController(text: selectedvalue4); // Sets default to saved address
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
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

bool shownOnce = false;
class _MyHomePageState extends State<MyHomePage> {
  Timer? _timer;
  late Future<List<ParkingSpace>> futureParkingSpaces;
  late List<ParkingSpace> oldSpaces; //cached parking spaces
  bool failure = false; //used to check if error message should be shown

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
                duration: Duration(seconds: 3),
              ),
            );
          }
        });
      });
    });

    //Only loads when app is initially launched
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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: !isDesktop ? AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(widget.title),
      ) : null,

      body: FutureBuilder<List<ParkingSpace>>(
        future: futureParkingSpaces,
        builder: (context, snapshot) {
          if (snapshot.hasData) {
            failure = false; //don't show error message
            final parkingSpaces = snapshot.data!;
            oldSpaces = parkingSpaces; //cache parking spaces
            final List<List<ParkingSpace>> byFloorList = [];
            final List<int> availablePerFloor = [];
            int maxFloor = 0;
            for (var space in parkingSpaces) {
              if (space.floor > maxFloor) {
                maxFloor = space.floor;
              }
            }

            for (var i = 1; i <= maxFloor; i++) {
              byFloorList.add(parkingSpaces.where((space) => space.floor == i).toList());
            }

            for (var floor in byFloorList) {
              final availableCount = floor.where((space) => !space.occupied).length;
              availablePerFloor.add(availableCount);
            }

            //Shows total open spots, and number of open spots per floor.
            return ListView.builder(
              itemCount: byFloorList.length + 3,
              itemBuilder: (context, index) {
                if (index == 2) {
                  return ListTile(
                    //total open spots
                    title: Text('Open Spots: ${availablePerFloor.reduce((a, b) => a + b)}', textAlign: TextAlign.center,),
                  );
                }
                else if (index == 0)
                {
                  return Padding(
                    //logo won't be so close to top of screen
                    padding: EdgeInsetsGeometry.symmetric(vertical: isDesktop ? 44 : 22),
                    child: SizedBox(
                      height: isDesktop ? 200 : 100,
                      child: Image.asset(
                        "assets/images/Akron-Zips-Logo-2008.png",
                        fit: BoxFit.fitHeight,  // Scales to fit within the SizedBox while maintaining aspect ratio
                        alignment: Alignment.center,
                      ),
                    )
                  );
                }
                else if (index == 1)
                {
                  return ListTile(
                    title: Text('Lot 37', textAlign: TextAlign.center,));
                }
                //Per floor occupancy. Green means at least one spot is open. Red means it is fully occupied.
                return ListTile(
                  title: Text(
                    'Floor: ${index-2}', textAlign: TextAlign.center,
                    style: TextStyle(color: availablePerFloor[index - 3] > 0 ? Colors.green : Colors.red)
                  ),
                  subtitle: 
                  Text(
                    'Open Spots: ${availablePerFloor[index - 3]}', 
                    textAlign: TextAlign.center, 
                    style: TextStyle(color: availablePerFloor[index - 3] > 0 ? Colors.green : Colors.red),
                  ),
                );
              },
            );
          } else if (snapshot.hasError) {
            //We will used cached parking spaces in order to at least show some information.
            if (oldSpaces != null)
            {
              failure = true; //show error message
              final List<List<ParkingSpace>> byFloorList = [];
              final List<int> availablePerFloor = [];
              int maxFloor = 0;
              for (var space in oldSpaces) {
                if (space.floor > maxFloor) {
                  maxFloor = space.floor;
                }
              }

              for (var i = 1; i <= maxFloor; i++) {
                byFloorList.add(oldSpaces.where((space) => space.floor == i).toList());
              }

              for (var floor in byFloorList) {
                final availableCount = floor.where((space) => !space.occupied).length;
                availablePerFloor.add(availableCount);
              }


              //Shows total open spots, and number of open spots per floor.
              return ListView.builder(
                itemCount: byFloorList.length + 3,
                itemBuilder: (context, index) {
                  if (index == 2) {
                    //total open spots
                    return ListTile(
                      title: Text('Open Spots: ${availablePerFloor.reduce((a, b) => a + b)}', textAlign: TextAlign.center,),
                    );
                  }
                  else if (index == 0)
                  {
                    return Padding(
                      //logo won't be so close to top of screen
                      padding: EdgeInsetsGeometry.symmetric(vertical: isDesktop ? 44 : 22),
                      child: SizedBox(
                        height: isDesktop ? 200 : 100,
                        child: Image.asset(
                          "assets/images/Akron-Zips-Logo-2008.png",
                          fit: BoxFit.fitHeight,  // Scales to fit within the SizedBox while maintaining aspect ratio
                          alignment: Alignment.center,
                        ),
                      )
                    );
                  }
                  else if (index == 1)
                  {
                    return ListTile(
                      title: Text('Lot 37', textAlign: TextAlign.center,));
                  }
                  //Per floor occupancy. Green means at least one spot is open. Red means it is fully occupied.
                  return ListTile(
                    title: Text(
                      'Floor: ${index-2}', textAlign: TextAlign.center,
                      style: TextStyle(color: availablePerFloor[index - 3] > 0 ? Colors.green : Colors.red)
                    ),
                    subtitle: 
                    Text(
                      'Open Spots: ${availablePerFloor[index - 3]}', 
                      textAlign: TextAlign.center, 
                      style: TextStyle(color: availablePerFloor[index - 3] > 0 ? Colors.green : Colors.red),
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
    );
  }
}
