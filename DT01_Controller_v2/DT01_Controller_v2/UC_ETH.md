# Ethernet Status & Error Control

This document explains how to read the microcontroller’s status over HTTP and how to set the error state using `curl`. It also includes guidance for integrating these endpoints into a .NET C# project.

---

## Getting Status (HTTP GET)

The device exposes a JSON status response at the root path:

- **Endpoint:** `GET /`
- **Response:**

```json
{"occupied":0,"error":0}
```

### Example (curl)

```bash
curl http://10.18.28.240/
```

Expected response:

```json
{"occupied":1,"error":0}
```

Where:
- `occupied`: `1` means occupied, `0` means empty
- `error`: `1` means error state forced on, `0` means normal

---

## Setting Error State (HTTP POST)

The device accepts a POST request to set the error state:

- **Endpoint:** `POST /error`
- **Body:** `1` to set error, `0` to clear error
- **Content-Type:** optional (plain text is fine)

### Example (set error)

```bash
curl -X POST http://10.18.28.240/error -d "1"
```

### Example (clear error)

```bash
curl -X POST http://10.18.28.240/error -d "0"
```

The device replies with the current JSON state after each request.

---

## .NET C# Integration Guidance

This section shows how to read the status and set the error state from a .NET C# application.

### Add an HttpClient

In most .NET apps, use a single shared `HttpClient` instance for all requests.

### Read Status (GET)

```csharp
using System.Net.Http;
using System.Text.Json;

var http = new HttpClient();
var json = await http.GetStringAsync("http://10.18.28.240/");

// Parse JSON
var doc = JsonDocument.Parse(json);
int occupied = doc.RootElement.GetProperty("occupied").GetInt32();
int error = doc.RootElement.GetProperty("error").GetInt32();
```

### Set Error (POST)

```csharp
using System.Net.Http;
using System.Text;

var http = new HttpClient();
var content = new StringContent("1", Encoding.UTF8, "text/plain");
var response = await http.PostAsync("http://10.18.28.240/error", content);
string json = await response.Content.ReadAsStringAsync();
```

### Suggested Integration Pattern

- Poll status periodically (e.g., every 250–1000 ms) if you need near‑real‑time updates.
- Use a background task (e.g., `HostedService` or `Timer`) in server apps.
- In UI apps, avoid blocking the UI thread; use `async/await`.

### Example: Simple polling loop

```csharp
var http = new HttpClient();

while (true)
{
    var json = await http.GetStringAsync("http://10.18.28.240/");
    var doc = JsonDocument.Parse(json);

    int occupied = doc.RootElement.GetProperty("occupied").GetInt32();
    int error = doc.RootElement.GetProperty("error").GetInt32();

    Console.WriteLine($"Occupied: {occupied}, Error: {error}");
    await Task.Delay(500);
}
```

---

## Troubleshooting Tips

- Confirm the device IP printed over Serial matches what you use in `curl` and C#.
- Ensure the PC and microcontroller are on the same subnet.
- If `curl` hangs, verify your network switch allows the port and that the W5500’s CS pin is correct.
