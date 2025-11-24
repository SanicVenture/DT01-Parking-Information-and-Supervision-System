import 'package:http/http.dart' as http;
import 'dart:convert';

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
      await http.get(Uri.parse('https://localhost:7288/api/parkingspaceitems/'));

  if (response.statusCode == 200) {
    final List<dynamic> jsonList = jsonDecode(response.body);
    return jsonList
        .map((json) => ParkingSpace.fromJson(json as Map<String, dynamic>))
        .toList();
  } else {
    throw Exception('Failed to load parking space');
  }
}

Future<void> createNewParkingSpace() async {
  final response = await http.post(
    Uri.parse('https://localhost:7288/api/parkingspaceitems'),
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

// Future<void> createNewPSFinal() async {
//   final response = await http.post(
//     Uri.parse('https://localhost:7288/api/pstotalresultsitems'),
//     headers: <String, String>{
//       'Content-Type': 'application/json; charset=UTF-8',
//     },
//     body: jsonEncode(<String, dynamic>{
//       'floor': 5,
//       'occupied': false,
//       'maintenanceAlert': false
//     }),
//   );

//   if (response.statusCode != 201) {
//     throw Exception('Failed to send maintenance alert');
//   }
// }

