import 'package:http/http.dart' as http;

Future<http.Response> fetchAlbum() {
    return http.get(Uri.parse('https://localhost:7288/api/todoitems'));
}