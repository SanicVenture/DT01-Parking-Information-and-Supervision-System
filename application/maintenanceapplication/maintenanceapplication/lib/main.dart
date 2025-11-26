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

        colorScheme: .fromSeed(seedColor: Colors.deepPurple),
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

//home page code

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  // late Future<ParkingSpace> futureParkingSpace;
  late Future<List<ParkingSpace>> futureParkingSpaces;
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    // futureParkingSpace = fetchParkingSpace();
    futureParkingSpaces = fetchParkingSpaces();
    _timer = Timer.periodic(const Duration(seconds: 10), (Timer t) {
      setState(() {
        futureParkingSpaces = fetchParkingSpaces();
      });
    });

  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  void _addParkingSpace() async {
    await createNewParkingSpace();
    setState(() {
      futureParkingSpaces = fetchParkingSpaces();
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

      body: FutureBuilder<List<ParkingSpace>>(
        future: futureParkingSpaces,
        builder: (context, snapshot) {
          if (snapshot.hasData) {
            final parkingSpaces = snapshot.data!;
            return ListView.builder(
              itemCount: parkingSpaces.length,
              itemBuilder: (context, index) {
                final space = parkingSpaces[index];
                final styleoftext = TextStyle(
                    fontWeight: FontWeight.bold,
                    color: space.maintenanceAlert ? Colors.red : Colors.black,
                  );
                return ListTile(
                  title: Text('Parking Space Number: ${space.id}', style: styleoftext),
                  subtitle: Text('Floor: ${space.floor}, Occupied: ${space.occupied}', style: styleoftext),
                );
              },
            );
          } else if (snapshot.hasError) {
            return Center(child: Text('${snapshot.error}'));
          }
          return const Center(child: CircularProgressIndicator());
        },
      ),

      floatingActionButton: FloatingActionButton(
        onPressed: _addParkingSpace,
        tooltip: 'Add Parking Space',
        child: const Icon(Icons.add),
      ),
    );
  }
}
