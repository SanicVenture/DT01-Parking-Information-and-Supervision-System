import 'dart:async';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:maintenanceapplication/httpmanager.dart';

class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback = (X509Certificate cert, String host, int port) => true;
  }
}
void main() {
  WidgetsFlutterBinding.ensureInitialized();
  HttpOverrides.global = MyHttpOverrides();
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  MyApp({super.key});
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flutter Demo',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
      ),
      home: MyHomePage(title: 'Maintenance Application'),
    );
  }
}

class SecondRoute extends StatefulWidget {
  SecondRoute({super.key});
  
  @override
  State<StatefulWidget> createState() => _SecondRouteState();
}

class _SecondRouteState extends State<SecondRoute> {

  final List<String> trueOrFalse = ['True', 'False'];

  String? selectedvalue1 = 'True';
  String? selectedvalue2 = 'True';
  String? selectedvalue3 = 'True';
  int selectedvalue4 = 0;


  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Parking Spot Simulation'),
      ),
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
              title: const Text('Subsystem Demo Menu'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => SecondRoute()),
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

class _ObjectInSpotRouteState extends State<ObjectInSpotRoute> {

  final List<String> trueOrFalse = ['True', 'False'];

  String? selectedvalue2 = 'True';
  int selectedvalue4 = 0;


  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Parking Spot Simulation'),
      ),
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
              title: const Text('Subsystem Demo Menu'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => SecondRoute()),
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

// open cv result code

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
        title: const Text('Parking Spot Simulation'),
      ),
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
              title: const Text('Subsystem Demo Menu'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => SecondRoute()),
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
            Text('Select Parking Space Id:'),
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

//home page code

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}


bool shownOnce = false;
class _MyHomePageState extends State<MyHomePage> {
  // late Future<ParkingSpace> futureParkingSpace;
  late Future<List<CompleteParkingSpace>> futureParkingSpaces;
  var oldSpaces;
  bool failure = false;

  Timer? _timer;

  @override
  void initState() {
    super.initState();
    futureParkingSpaces = fetchCompleteParkingSpaces();

    _timer = Timer.periodic(const Duration(seconds: 5), (Timer t) {
      setState(() {
        futureParkingSpaces = fetchCompleteParkingSpaces();     
        WidgetsBinding.instance.addPostFrameCallback((_) {
          if (failure) {
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
              title: const Text('Subsystem Demo Menu'),
              onTap: () {
                Navigator.push(context,
                  MaterialPageRoute(builder: (context) => SecondRoute()),
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
            failure = false;
            final parkingSpaces = snapshot.data!;
            oldSpaces = parkingSpaces;
            return ListView.builder(
              itemCount: parkingSpaces.length,
              itemBuilder: (context, index) {
                final space = parkingSpaces[index];
                final styleoftext = TextStyle(
                    fontWeight: FontWeight.bold,
                    color: space.sensorConnectedToNetwork ? (space.maintenanceAlert ? Colors.red : Colors.black) : const Color.fromARGB(255, 238, 184, 104),
                  );
                // return ListTile(
                //   title: Text('Parking Space Number: ${space.id}', style: styleoftext),
                //   subtitle: Text('Floor: ${space.floor}, Occupied: ${space.occupied}', style: styleoftext),
                // );
                final imageURL = '${isAndroid ? staticRemoteUrl : staticLocalUrl}/images/output_frame_with_detections_${space.id}.png';
                final provider = NetworkImage(imageURL);
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
          } else if (snapshot.hasError) {
            if (oldSpaces != null) {
            failure = true;
            return ListView.builder(
              itemCount: oldSpaces!.length,
              itemBuilder: (context, index) {
                final space = oldSpaces![index];
                final styleoftext = TextStyle(
                    fontWeight: FontWeight.bold,
                    color: space.sensorConnectedToNetwork ? (space.maintenanceAlert ? Colors.red : Colors.black) : const Color.fromARGB(255, 238, 184, 104),
                  );
                // return ListTile(
                //   title: Text('Parking Space Number: ${space.id}', style: styleoftext),
                //   subtitle: Text('Floor: ${space.floor}, Occupied: ${space.occupied}', style: styleoftext),
                // );
                final imageURL = '${isAndroid ? staticRemoteUrl : staticLocalUrl}/images/output_frame_with_detections_${space.id}.png';
                final provider = NetworkImage(imageURL);
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
            else{
              return Center(child: Text('${snapshot.error}'));
            }

          }
          return const Center(child: CircularProgressIndicator());
        },
      ),

      floatingActionButton: FloatingActionButton(
        onPressed: _deleteOpenCVPoints,
        tooltip: 'Add Parking Space',
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
