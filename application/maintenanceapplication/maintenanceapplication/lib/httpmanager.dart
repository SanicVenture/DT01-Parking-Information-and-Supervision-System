import 'dart:io';
import 'package:http/http.dart' as http;
import 'dart:convert';

//In a production environment, we would probably use an actual domain name and the address wouldn't change.
final String localUrl = 'https://localhost:7288/api';
// final String remoteUrl = 'https://192.168.1.31:3124/api';
// final String remoteUrl = 'https://10.217.52.241:3124/api';
String remoteUrl = 'https://10.18.31.4:3124/api';

final String staticLocalUrl = 'https://localhost:7288';
String staticRemoteUrl = 'https://10.18.31.4:3124';
final bool isAndroid = Platform.isAndroid;
final bool isDesktop = Platform.isWindows || Platform.isLinux || Platform.isMacOS;

//this class is the basic parking spot data, also used by the informational app (the customer-facing parking area display
// and mobile app)
class ParkingSpace {
  final int id;
  final int floor;
  final bool occupied;
  final bool maintenanceAlert;

  const ParkingSpace({
    required this.id,
    required this.floor,
    required this.occupied,
    required this.maintenanceAlert,
  });

  factory ParkingSpace.fromJson(Map<String, dynamic> json) {
    return switch (json) {
      {'id': int id, 'floor': int floor, 'occupied': bool occupied, 'maintenanceAlert': bool maintenanceAlert} =>
        ParkingSpace(id: id, floor: floor, occupied: occupied, maintenanceAlert: maintenanceAlert),
      _ => throw Exception('Invalid JSON format for ParkingSpace'),
    };
  }
}

  //this class encompasses both camera and microcontroller raw data
class PSTotalResults {
  final int id;
  final bool vehicle;
  final bool objectInSpot;
  final bool parkingSpaceObstructed;
  final bool sensorConnectedToNetwork;

  const PSTotalResults({
    required this.id,
    required this.vehicle,
    required this.objectInSpot,
    required this.parkingSpaceObstructed,
    required this.sensorConnectedToNetwork,
  });

  factory PSTotalResults.fromJson(Map<String, dynamic> json) {
    return switch (json) {
      {'id': int id, 
      'vehicle': bool vehicle, 
      'objectInSpot': bool objectInSpot, 
      'parkingSpaceObstructed': bool parkingSpaceObstructed, 
      'sensorConnectedToNetwork': bool sensorConnectedToNetwork} =>
        PSTotalResults(id: id, 
          vehicle: vehicle, 
          objectInSpot: objectInSpot, 
          parkingSpaceObstructed: parkingSpaceObstructed, 
          sensorConnectedToNetwork: sensorConnectedToNetwork),
      _ => throw Exception('Invalid JSON format for PSTotalResults'),
    };
  }
}

//this is the combination of ParkingSpace and PSTotalResults
class CompleteParkingSpace
{
  final int id;
  final int floor;
  final bool occupied;
  final bool maintenanceAlert;
  final bool vehicleStatus;
  final bool objectInSpot;
  final bool parkingSpaceObstructed;
  final bool sensorConnectedToNetwork;
  const CompleteParkingSpace({
    required this.id,
    required this.floor,
    required this.occupied,
    required this.maintenanceAlert,
    required this.vehicleStatus,
    required this.objectInSpot,
    required this.parkingSpaceObstructed,
    required this.sensorConnectedToNetwork,
  });

  factory CompleteParkingSpace.fromParkingSpaceAndPSTotalResults(
    ParkingSpace parkingSpace, 
    PSTotalResults psTotalResults) {
    return CompleteParkingSpace(
      id: parkingSpace.id,
      floor: parkingSpace.floor,
      occupied: parkingSpace.occupied,
      maintenanceAlert: parkingSpace.maintenanceAlert,
      vehicleStatus: psTotalResults.vehicle,
      objectInSpot: psTotalResults.objectInSpot,
      parkingSpaceObstructed: psTotalResults.parkingSpaceObstructed,
      sensorConnectedToNetwork: psTotalResults.sensorConnectedToNetwork,
    );
  }
}

//only used to create a new PsTotalResults object, so that, in turn, we can create a CompleteParkingSpace object.
//essentially does the reverse of the server - instead of PSTotalResults being used to create a 
//ParkingSpace object, the ParkingSpace object is being used to create the vehicle status of the
//PSTotalResults.
bool convertParkingSpaceToVehicleStatus(ParkingSpace parkingSpace) {
  if (parkingSpace.occupied && !parkingSpace.maintenanceAlert) {
    return true;
  } else {
    return false;
  }
}

//only used to create a new PsTotalResults object, so that, in turn, we can create a CompleteParkingSpace object.
//essentially does the reverse of the server - instead of PSTotalResults being used to create a 
//ParkingSpace object, the ParkingSpace object is being used to create the obstructed of the
//PSTotalResults.
bool convertParkingSpaceToObstructedStatus(ParkingSpace parkingSpace) {
  if (!parkingSpace.occupied && parkingSpace.maintenanceAlert) {
    return true;
  } else if (parkingSpace.occupied) {
    return true;
  } else {
    return false;
  }
}

Future<List<CompleteParkingSpace>> fetchCompleteParkingSpaces() async {
  final parkingSpaces = await fetchParkingSpaces();
  final psTotalResults = await fetchPSTotalResults();

  //match each PSTotalResults object to its corresponding ParkingSpace. If the PSTotalResults is missing, then create a
  //new one. 
  //then, create the CompleteParkingSpace with the ParkingSpace and PSTotalResults.
  return parkingSpaces.map((space) {
    final psTotalResult = psTotalResults.firstWhere((result) => result.id == space.id, orElse: () => PSTotalResults(
      id: space.id, 
      vehicle: convertParkingSpaceToVehicleStatus(space), 
      objectInSpot: space.occupied, 
      parkingSpaceObstructed: convertParkingSpaceToObstructedStatus(space), 
      sensorConnectedToNetwork: false));
    return CompleteParkingSpace.fromParkingSpaceAndPSTotalResults(space, psTotalResult);
  }).toList();
}

Future<List<ParkingSpace>> fetchParkingSpaces() async {
  final response =
      await http.get(Uri.parse('${isAndroid ? remoteUrl : localUrl}/parkingspaceitems/'));

  if (response.statusCode == 200) {
    final List<dynamic> jsonList = jsonDecode(response.body);
    return jsonList
        .map((json) => ParkingSpace.fromJson(json as Map<String, dynamic>))
        .toList();
  } else {
    throw Exception('Failed to load parking space');
  }
}

Future<List<PSTotalResults>> fetchPSTotalResults() async {
  final response =
      await http.get(Uri.parse('${isAndroid ? remoteUrl : localUrl}/pstotalresultsitems/'));

  if (response.statusCode == 200) {
    final List<dynamic> jsonList = jsonDecode(response.body);
    return jsonList
        .map((json) => PSTotalResults.fromJson(json as Map<String, dynamic>))
        .toList();
  } else {
    throw Exception('Failed to load parking space');
  }
}

Future<void> createNewParkingSpace() async {
  final response = await http.post(
    Uri.parse('${isAndroid ? remoteUrl : localUrl}/parkingspaceitems'),
    headers: <String, String>{
      'Content-Type': 'application/json; charset=UTF-8',
    },
    body: jsonEncode(<String, dynamic>{
      'floor': 5,
      'occupied': false,
      'maintenanceAlert': false
    }),
  );

  if (response.statusCode != 201) {
    throw Exception('Failed to send maintenance alert');
  }
}

//Currently, this only deletes the four corners of the camera vision boundary of the parking space with Id=1. 
//Ideally, this could handle any parking space, but it currently only serves the purposes of Demo Day.
Future<void> deleteOpenCVPoints() async {
  final response = await http.delete(
    Uri.parse('${isAndroid ? remoteUrl : localUrl}/opencvpolygonsitems/1'),
    headers: <String, String>{
      'Content-Type': 'application/json; charset=UTF-8',
    },
    body: jsonEncode(<String, dynamic>{'id': 1,}),
  );

  if (response.statusCode != 204) {
    throw Exception('Failed to send maintenance alert');
  }
}

Future<void> createNewPSFinal(int Id, String vehicle, String objectinspace, String parkingspaceobstructed) async {
  final response = await http.put(
    Uri.parse('${isAndroid ? remoteUrl : localUrl}/pstotalresultsitems/$Id'),
    headers: <String, String>{
      'Content-Type': 'application/json; charset=UTF-8',
    },
    body: jsonEncode(<String, dynamic>{
      'id': Id,
      'vehicle': vehicle == 'True' ? true : false,
      'objectinspot': objectinspace == 'True' ? true : false,
      'parkingspaceobstructed': parkingspaceobstructed == 'True' ? true : false,
    }),
  );

  if (response.statusCode != 204) {
    throw Exception('Failed to send maintenance alert');
  }
}

// used to send simulated microcontroller data to the server.
Future<void> addObjectInSpotState(int Id, String objectinspace) async {
  final response = await http.put(
    Uri.parse('${isAndroid ? remoteUrl : localUrl}/objectinspotitems/$Id'),
    headers: <String, String>{
      'Content-Type': 'application/json; charset=UTF-8',
    },
    body: jsonEncode(<String, dynamic>{
      'id': Id,
      'objectinspot': objectinspace == 'True' ? true : false,
    }),
  );

  if (response.statusCode != 204) {
    throw Exception('Failed to send maintenance alert');
  }
}

// used to send simulated camera vision data to the server.
Future<void> addOpenCVResultState(int Id, String vehicle, String parkingspaceobstructed) async {
  final response = await http.put(
    Uri.parse('${isAndroid ? remoteUrl : localUrl}/opencvresultsitems/$Id'),
    headers: <String, String>{
      'Content-Type': 'application/json; charset=UTF-8',
    },
    body: jsonEncode(<String, dynamic>{
      'id': Id,
      'vehicle': vehicle == 'True' ? true : false,
      'parkingspaceobstructed': parkingspaceobstructed == 'True' ? true : false,
    }),
  );

  if (response.statusCode != 204) {
    throw Exception('Failed to send maintenance alert');
  }
}