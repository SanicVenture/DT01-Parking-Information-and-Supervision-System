import 'dart:async';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:maintenanceapplication/httpmanager.dart';
import 'package:shared_preferences/shared_preferences.dart';


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
void main() async{
  WidgetsFlutterBinding.ensureInitialized();
  HttpOverrides.global = MyHttpOverrides(); // Apply the override
  if (!isDesktop)
  {  SystemChrome.setPreferredOrientations([DeviceOrientation.portraitUp]);} // Force portrait mode on mobile devices
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
      staticRemoteUrl = storedIP.replaceAll('/api', '');
    }
  }
}

class MyApp extends StatelessWidget {
  MyApp({super.key});

  // Root of the application
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'Flutter Demo',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
      ),
      home: MyHomePage(title: 'Maintenance Application'),
    );
  }
}

// Used during midterm demo for simulating both camera vision output and microcontroller sensor output
class PSSimulateRoute extends StatefulWidget {
  PSSimulateRoute({super.key});
  
  @override
  State<StatefulWidget> createState() => _PSSimulateRouteState();
}

class _PSSimulateRouteState extends State<PSSimulateRoute> {
  final List<String> trueOrFalse = ['True', 'False'];
  String? selectedvalue1 = 'True';
  String? selectedvalue2 = 'True';
  String? selectedvalue3 = 'True';
  int selectedvalue4 = 0;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Parking Spot Simulation: Camera and Microcontroller Inputs'),
      ),
      //the sandwich menu on the top left
      drawer: Drawer(
        child: ListView(
          padding: EdgeInsets.zero,
          children: <Widget>[
            const DrawerHeader(
              decoration: BoxDecoration(
                color: Colors.blue,
              ),
              child: Text(
                'Maintenance Application',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 24,
                ),
              ),
            ),
            ListTile(
              leading: const Icon(Icons.home),
              title: const Text('Home'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => const MyHomePage(title: 'Home')),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Parking Spot Simulation'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => PSSimulateRoute()),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Simulate Microcontroller Output'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => ObjectInSpotRoute()),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Simulate OpenCV Output'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => OpenCVResultRoute()),
                );
              },
            ),
          ],
        ),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            Text('Select Parking Space Id:'),
            TextField(
              keyboardType: TextInputType.number,
              onChanged: (value) {
                setState(() {
                  selectedvalue4 = int.tryParse(value) ?? 0;
                });
              },
            ),
            Text('Select Vehicle Status:'),
            DropdownButton<String>(
              value: selectedvalue1,
              items: trueOrFalse.map((String value) {
                return DropdownMenuItem<String>(
                  value: value,
                  child: Text(value),
                );
              }).toList(),
              onChanged: (newValue) {
                setState(() {
                  selectedvalue1 = newValue;
                });
              },
            ),
            Text('Select if Object in Spot:'),
            DropdownButton<String>(
              value: selectedvalue2,
              items: trueOrFalse.map((String value) {
                return DropdownMenuItem<String>(
                  value: value,
                  child: Text(value),
                );
              }).toList(),
              onChanged: (newValue) {
                setState(() {
                  selectedvalue2 = newValue;
                });
              },
            ),
            Text('Select Parking Space Obstructed:'),
            DropdownButton<String>(
              value: selectedvalue3,
              items: trueOrFalse.map((String value) {
                return DropdownMenuItem<String>(
                  value: value,
                  child: Text(value),
                );
              }).toList(),
              onChanged: (newValue) {
                setState(() {
                  selectedvalue3 = newValue;
                });
              },
            ),
            ElevatedButton(
              onPressed: () {
                createNewPSFinal(
                  selectedvalue4,
                  selectedvalue1 ?? 'False',
                  selectedvalue2 ?? 'False',
                  selectedvalue3 ?? 'False'
                );
              },
              child: const Text('Send Statuses!'),
            ),
          ],
        )
      ),
    );
  }
}

// object in spot code

class ObjectInSpotRoute extends StatefulWidget {
  ObjectInSpotRoute({super.key});
  
  @override
  State<StatefulWidget> createState() => _ObjectInSpotRouteState();
}

// Used during midterm demo for simulating microcontroller sensor output
class _ObjectInSpotRouteState extends State<ObjectInSpotRoute> {
  final List<String> trueOrFalse = ['True', 'False'];
  String? selectedvalue2 = 'True';
  int selectedvalue4 = 0;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Microcontroller Output Simulation'),
      ),
      //the sandwich menu on the top left
      drawer: Drawer(
        child: ListView(
          padding: EdgeInsets.zero,
          children: <Widget>[
            const DrawerHeader(
              decoration: BoxDecoration(
                color: Colors.blue,
              ),
              child: Text(
                'Maintenance Application',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 24,
                ),
              ),
            ),
            ListTile(
              leading: const Icon(Icons.home),
              title: const Text('Home'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => const MyHomePage(title: 'Home')),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Parking Spot Simulation'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => PSSimulateRoute()),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Simulate Microcontroller Output'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => ObjectInSpotRoute()),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Simulate OpenCV Output'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => OpenCVResultRoute()),
                );
              },
            ),
          ],
        ),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            Text('Select Parking Space Id:'),
            TextField(
              keyboardType: TextInputType.number,
              onChanged: (value) {
                setState(() {
                  selectedvalue4 = int.tryParse(value) ?? 0;
                });
              },
            ),
            Text('Select if Object in Spot:'),
            DropdownButton<String>(
              value: selectedvalue2,
              items: trueOrFalse.map((String value) {
                return DropdownMenuItem<String>(
                  value: value,
                  child: Text(value),
                );
              }).toList(),
              onChanged: (newValue) {
                setState(() {
                  selectedvalue2 = newValue;
                });
              },
            ),
            ElevatedButton(
              onPressed: () {
                addObjectInSpotState(
                  selectedvalue4,
                  selectedvalue2 ?? 'False'
                );
              },
              child: const Text('Send Statuses!'),
            ),
          ],
        )
      ),
    );
  }
}

// Used during midterm demo for simulating camera vision output
class OpenCVResultRoute extends StatefulWidget {
  OpenCVResultRoute({super.key});
  
  @override
  State<StatefulWidget> createState() => _OpenCVResultRouteState();
}

class _OpenCVResultRouteState extends State<OpenCVResultRoute> {
  final List<String> trueOrFalse = ['True', 'False'];
  String? selectedvalue1 = 'True';
  String? selectedvalue3 = 'True';
  int selectedvalue4 = 0;


  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Camera Vision Output Simulation'),
      ),
      //the sandwich menu on the top left. 
      drawer: Drawer(
        child: ListView(
          padding: EdgeInsets.zero,
          children: <Widget>[
            const DrawerHeader(
              decoration: BoxDecoration(
                color: Colors.blue,
              ),
              child: Text(
                'Maintenance Application',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 24,
                ),
              ),
            ),
            ListTile(
              leading: const Icon(Icons.home),
              title: const Text('Home'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => const MyHomePage(title: 'Home')),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Parking Spot Simulation'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => PSSimulateRoute()),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Simulate Microcontroller Output'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => ObjectInSpotRoute()),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Simulate OpenCV Output'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => OpenCVResultRoute()),
                );
              },
            ),
          ],
        ),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            Text('Select Parking Space Id:'),
            TextField(
              keyboardType: TextInputType.number,
              onChanged: (value) {
                setState(() {
                  selectedvalue4 = int.tryParse(value) ?? 0;
                });
              },
            ),
            Text('Select Vehicle Status:'),
            DropdownButton<String>(
              value: selectedvalue1,
              items: trueOrFalse.map((String value) {
                return DropdownMenuItem<String>(
                  value: value,
                  child: Text(value),
                );
              }).toList(),
              onChanged: (newValue) {
                setState(() {
                  selectedvalue1 = newValue;
                });
              },
            ),
            Text('Select Parking Space Obstructed:'),
            DropdownButton<String>(
              value: selectedvalue3,
              items: trueOrFalse.map((String value) {
                return DropdownMenuItem<String>(
                  value: value,
                  child: Text(value),
                );
              }).toList(),
              onChanged: (newValue) {
                setState(() {
                  selectedvalue3 = newValue;
                });
              },
            ),
            ElevatedButton(
              onPressed: () {
                addOpenCVResultState(
                  selectedvalue4,
                  selectedvalue1 ?? 'False',
                  selectedvalue3 ?? 'False'
                );
              },
              child: const Text('Send Statuses!'),
            ),
          ],
        )
      ),
    );
  }
}

// Used for setting the server IP Address. In a production environment, either the address for the server
// would be preprogrammed, or there would be a user friendly list of parking garages.
class IPRoute extends StatefulWidget {
  IPRoute({super.key});
  
  @override
  State<StatefulWidget> createState() => _IPRouteState();
}

class _IPRouteState extends State<IPRoute> {
  String selectedvalue4 = staticRemoteUrl;

  final TextEditingController _controller = TextEditingController(text: staticRemoteUrl);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            Text('IP Address:'),
            TextField(
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
                staticRemoteUrl = selectedvalue4;
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

//home page code

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}


bool shownOnce = false;
class _MyHomePageState extends State<MyHomePage> {
  late Future<List<CompleteParkingSpace>> futureParkingSpaces;
  List<CompleteParkingSpace>? oldSpaces; //cached parking spaces
  bool failure = false; //used to check if error message should be shown

  Timer? _timer;

  @override
  void initState() {
    super.initState();
    futureParkingSpaces = fetchCompleteParkingSpaces();

    _timer = Timer.periodic(const Duration(seconds: 15), (Timer t) {
      setState(() {
        futureParkingSpaces = fetchCompleteParkingSpaces();     
        WidgetsBinding.instance.addPostFrameCallback((_) {
          if (failure) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('Error fetching data, showing last successful data'),
                duration: Duration(seconds: 5),
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

  void _addParkingSpace() async {
    await createNewParkingSpace();
    setState(() {
      futureParkingSpaces = fetchCompleteParkingSpaces();
    });
  }

  //deletes the four corners of the camera vision boundaries that are stored on the server end.
  void _deleteOpenCVPoints() async {
    await deleteOpenCVPoints();
    setState(() {
      futureParkingSpaces = fetchCompleteParkingSpaces();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(widget.title),
      ),
      //the sandwich menu on the top left
      drawer: Drawer(
        child: ListView(
          padding: EdgeInsets.zero,
          children: <Widget>[
            const DrawerHeader(
              decoration: BoxDecoration(
                color: Colors.blue,
              ),
              child: Text(
                'Maintenance Application',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 24,
                ),
              ),
            ),
            ListTile(
              leading: const Icon(Icons.home),
              title: const Text('Home'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => const MyHomePage(title: 'Home')),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('IP Configuration'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => IPRoute()),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Parking Spot Simulation'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => PSSimulateRoute()),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Simulate Microcontroller Output'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => ObjectInSpotRoute()),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.settings),
              title: const Text('Simulate OpenCV Output'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => OpenCVResultRoute()),
                );
              },
            ),
          ],
        ),
      ),

      body: FutureBuilder<List<CompleteParkingSpace>>(
        future: futureParkingSpaces,
        builder: (context, snapshot) {
          if (snapshot.hasData) {
            failure = false; //don't show error message
            final parkingSpaces = snapshot.data!;
            oldSpaces = parkingSpaces; //cache parking spaces
            return ListView.builder(
              itemCount: parkingSpaces.length,
              itemBuilder: (context, index) {
                final space = parkingSpaces[index];
                final styleoftext = TextStyle(
                    fontWeight: FontWeight.bold,
                    //If the microcontroller is offline, the entry is colored orange. If there is something wrong with
                    // the spot, then it is red. Otherwise, it is black.
                    color: space.sensorConnectedToNetwork ?
                      (space.maintenanceAlert ? Colors.red : Colors.black) : const Color.fromARGB(255, 238, 184, 104),
                  );
                //if there is a way for the detection images to not be static images, then it should be changed.
                final imageURL =
                 '${isAndroid ? staticRemoteUrl : staticLocalUrl}/images/output_frame_with_detections_${space.id}.png';
                final provider = NetworkImage(imageURL);
                //done to guarantee image is actually updated
                provider.evict();
                var imageKey = ValueKey(DateTime.now().toString());
                return ExpansionTile(
                  title: Text('Parking Space Number: ${space.id}', style: styleoftext),
                  subtitle: Text('Floor: ${space.floor}, Occupied: ${space.occupied}', style: styleoftext),
                  children: <Widget>[
                    Text('Is the Object a Vehicle: ${space.vehicleStatus}', style: styleoftext),
                    space.sensorConnectedToNetwork ?
                     Text('Object in Spot According To Sensor: ${space.objectInSpot}', style: styleoftext) : 
                     Text('Sensor Not Connected', style: styleoftext),
                    Text('Parking Space Occupied According To Camera: ${space.parkingSpaceObstructed}', style: styleoftext),
                    Text('YOLO Detections:', style: styleoftext),
                    Image(
                      image: provider,
                      errorBuilder: (context, error, stackTrace) {
                        return const Text('Error loading YOLO image');
                      },
                      height:400,
                      key: imageKey
                    ),
                  ],
                );
              },
            );
          } else if (snapshot.hasError) {
            if (oldSpaces != null) {
            failure = true;
            return ListView.builder(
              itemCount: oldSpaces!.length,
              itemBuilder: (context, index) {
                final space = oldSpaces![index];
                final styleoftext = TextStyle(
                    fontWeight: FontWeight.bold,
                    //If the microcontroller is offline, the entry is colored orange. If there is something wrong with the spot, then it is red. Otherwise, it is black.
                    color: space.sensorConnectedToNetwork ? (space.maintenanceAlert ? Colors.red : Colors.black) : const Color.fromARGB(255, 238, 184, 104),
                  );
                //if there is a way for the detection images to not be static images, then it should be changed.
                final imageURL = '${isAndroid ? staticRemoteUrl : staticLocalUrl}/images/output_frame_with_detections_${space.id}.png';
                final provider = NetworkImage(imageURL);
                //done to guarantee image is actually updated
                provider.evict();
                var imageKey = ValueKey(DateTime.now().toString());
                return ExpansionTile(
                  title: Text('Parking Space Number: ${space.id}', style: styleoftext),
                  subtitle: Text('Floor: ${space.floor}, Occupied: ${space.occupied}', style: styleoftext),
                  children: <Widget>[
                    Text('Is the Object a Vehicle: ${space.vehicleStatus}', style: styleoftext),
                    space.sensorConnectedToNetwork ? Text('Object in Spot According To Sensor: ${space.objectInSpot}', style: styleoftext) : Text('Sensor Not Connected', style: styleoftext),
                    Text('Parking Space Obstructed According To Camera: ${space.parkingSpaceObstructed}', style: styleoftext),
                    Text('YOLO Detections:', style: styleoftext),
                    Image(
                      image: provider,
                      errorBuilder: (context, error, stackTrace) {
                        return const Text('Error loading YOLO image');
                      },
                      height:400,
                      key: imageKey
                    ),
                  ],
                );
              },
            );
            }
            else {
              return Center(child: Text('${snapshot.error}'));
            }
          }
          return const Center(child: CircularProgressIndicator());
        },
      ),

      //In a production environment, there would be a menu to select which spot the worker wants to delete the camera vision points from. 
      //This is currently mostly for the purposes of quickly changing the boundary points for the demo day parking spot, which is spot ID = 1.
      floatingActionButton: FloatingActionButton(
        onPressed: _deleteOpenCVPoints,
        tooltip: 'Delete Parking Space 1 OpenCVPoints',
        child: const Icon(Icons.delete),
      ),
      // floatingActionButton: FloatingActionButton(
      //   onPressed: _addParkingSpace,
      //   tooltip: 'Add Parking Space',
      //   child: const Icon(Icons.add),
      // ),
    );
  }
}
