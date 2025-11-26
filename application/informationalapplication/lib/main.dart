import 'dart:async';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:informationalapplication/httpmanager.dart';


class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback = (X509Certificate cert, String host, int port) => true;
  }
}
void main() {
  WidgetsFlutterBinding.ensureInitialized();
  HttpOverrides.global = MyHttpOverrides(); // Apply the override
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  MyApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flutter Demo',
      theme: ThemeData(
        colorScheme: .fromSeed(seedColor: Colors.deepPurple),
      ),
      home: MyHomePage(title: 'Parking Lot Entrance Information'),
    );
  }
}

abstract class ListItem {

  Widget buildTitle(BuildContext context);

  Widget buildSubtitle(BuildContext context);

}

class HeadingItem implements ListItem {
  final String heading; 

  HeadingItem(this.heading);

  @override
  Widget buildTitle(BuildContext context) {
    return Text(heading, style: Theme.of(context).textTheme.headlineSmall);
  }
  
  @override
  Widget buildSubtitle(BuildContext context) => const SizedBox.shrink();
}

class MessageItem implements ListItem {
  final String sender;
  final String body;

  MessageItem(this.sender, this.body);

  @override
  Widget buildTitle(BuildContext context) => Text(sender);

  @override
  Widget buildSubtitle(BuildContext context) => Text(body);
}



class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  Timer? _timer;
  late Future<List<ParkingSpace>> futureParkingSpaces;

  @override
  void initState() {
    super.initState();
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

  void _updateParkingSpots() {
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

      body: FutureBuilder<List<ParkingSpace>>(
        future: futureParkingSpaces,
        builder: (context, snapshot) {
          if (snapshot.hasData) {
            final parkingSpaces = snapshot.data!;
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
              itemCount: ByFloorList.length,
              itemBuilder: (context, index) {
                return ListTile(
                  title: Text('Floor: ${index + 1}'),
                  subtitle: Text('Available Spots: ${AvailablePerFloor[index]}\nTotal Spots: ${ByFloorList[index].length}'),
                );
              },
            );
          } else if (snapshot.hasError) {
            return Center(child: Text('${snapshot.error}'));
          }

          // By default, show a loading spinner.
          return const Center(child: CircularProgressIndicator());
        },
      ),

      floatingActionButton: FloatingActionButton(
        onPressed: _updateParkingSpots,
        tooltip: 'Reload',
        child: const Icon(Icons.replay),
      ),
    );
  }
}
