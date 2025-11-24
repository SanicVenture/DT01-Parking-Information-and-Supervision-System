import 'package:flutter/material.dart';
import 'package:informationalapplication/httpmanager.dart';

void main() {
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
        // This is the theme of your application.
        //
        // TRY THIS: Try running your application with "flutter run". You'll see
        // the application has a purple toolbar. Then, without quitting the app,
        // try changing the seedColor in the colorScheme below to Colors.green
        // and then invoke "hot reload" (save your changes or press the "hot
        // reload" button in a Flutter-supported IDE, or press "r" if you used
        // the command line to start the app).
        //
        // Notice that the counter didn't reset back to zero; the application
        // state is not lost during the reload. To reset the state, use hot
        // restart instead.
        //
        // This works for code too, not just values: Most code changes can be
        // tested with just a hot reload.
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

  // This widget is the home page of your application. It is stateful, meaning
  // that it has a State object (defined below) that contains fields that affect
  // how it looks.

  // This class is the configuration for the state. It holds the values (in this
  // case the title) provided by the parent (in this case the App widget) and
  // used by the build method of the State. Fields in a Widget subclass are
  // always marked "final".

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  int _counter = 0;
  // late Future<ParkingSpace> futureParkingSpace;
  late Future<List<ParkingSpace>> futureParkingSpaces;

  @override
  void initState() {
    super.initState();
    // futureParkingSpace = fetchParkingSpace();
    futureParkingSpaces = fetchParkingSpaces();

  }


  void _incrementCounter() {
    setState(() {
      // This call to setState tells the Flutter framework that something has
      // changed in this State, which causes it to rerun the build method below
      // so that the display can reflect the updated values. If we changed
      // _counter without calling setState(), then the build method would not be
      // called again, and so nothing would appear to happen.
      futureParkingSpaces = fetchParkingSpaces();
    });
  }

  @override
  Widget build(BuildContext context) {
    // This method is rerun every time setState is called, for instance as done
    // by the _incrementCounter method above.
    //
    // The Flutter framework has been optimized to make rerunning build methods
    // fast, so that you can just rebuild anything that needs updating rather
    // than having to individually change instances of widgets.
    return Scaffold(
      appBar: AppBar(
        // TRY THIS: Try changing the color here to a specific color (to
        // Colors.amber, perhaps?) and trigger a hot reload to see the AppBar
        // change color while the other colors stay the same.
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        // Here we take the value from the MyHomePage object that was created by
        // the App.build method, and use it to set our appbar title.
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
            parkingSpaces.forEach((space) {
              if (space.floor > maxFloor) {
                maxFloor = space.floor;
              }
            });

            for (var i = 1; i <= maxFloor; i++) {
              ByFloorList.add(parkingSpaces.where((space) => space.floor == i).toList());
            }

            ByFloorList.forEach((floor) {
              final availableCount = floor.where((space) => !space.occupied).length;
              AvailablePerFloor.add(availableCount);
            });


            return ListView.builder(
              itemCount: ByFloorList.length,
              itemBuilder: (context, index) {
                return ListTile(
                  title: Text('Floor: ${index + 1}'),
                  subtitle: Text('Available Spots: ${AvailablePerFloor[index]}'),
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

      // body: Row(children: <Widget>[


        
      // ],)
      // ,

      // body: Center(
      //   // Center is a layout widget. It takes a single child and positions it
      //   // in the middle of the parent.
      //   child: Column(
      //     // Column is also a layout widget. It takes a list of children and
      //     // arranges them vertically. By default, it sizes itself to fit its
      //     // children horizontally, and tries to be as tall as its parent.
      //     //
      //     // Column has various properties to control how it sizes itself and
      //     // how it positions its children. Here we use mainAxisAlignment to
      //     // center the children vertically; the main axis here is the vertical
      //     // axis because Columns are vertical (the cross axis would be
      //     // horizontal).
      //     //
      //     // TRY THIS: Invoke "debug painting" (choose the "Toggle Debug Paint"
      //     // action in the IDE, or press "p" in the console), to see the
      //     // wireframe for each widget.
      //     mainAxisAlignment: .center,
      //     children: [
      //       const Text('You have pushed the button this many times:'),
      //       Text(
      //         '$_counter',
      //         style: Theme.of(context).textTheme.headlineMedium,
      //       ),
      //     ],
      //   ),
      // ),
      floatingActionButton: FloatingActionButton(
        onPressed: _incrementCounter,
        tooltip: 'Reload',
        child: const Icon(Icons.replay),
      ),
    );
  }
}
