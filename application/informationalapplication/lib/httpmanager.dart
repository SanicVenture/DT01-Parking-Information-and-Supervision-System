import 'dart:io';
import 'package:http/http.dart' as http;
import 'dart:convert';

final String localUrl = 'https://localhost:7288/api/parkingspaceitems/';
final String remoteUrl = 'https://192.168.1.31:3124/api/parkingspaceitems/';
final bool isAndroid = Platform.isAndroid;
final bool isDesktop = Platform.isWindows || Platform.isLinux || Platform.isMacOS;

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

Future<List<ParkingSpace>> fetchParkingSpaces() async {
  final response =
      await http.get(Uri.parse(isAndroid ? remoteUrl : localUrl));

  if (response.statusCode == 200) {
    final List<dynamic> jsonList = jsonDecode(response.body);
    return jsonList
        .map((json) => ParkingSpace.fromJson(json as Map<String, dynamic>))
        .toList();
  } else {
    throw Exception('Failed to load parking space');
  }
}