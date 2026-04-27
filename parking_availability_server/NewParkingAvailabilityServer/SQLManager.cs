using Microsoft.Data.Sqlite;
using NewParkingAvailabilityServer.Models;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewParkingAvailabilityServer
{
    public class SQLManager
    {
        public string connectionString = "Data Source=ParkingSpots.db";
        public string psfinalconnectionString = "Data Source=PSFinal.db";
        public string objectinspotconnectionString = "Data Source=ObjectInSpot.db";
        public string openCVConnectionString = "Data Source=OpenCVResults.db";
        public string opencvpolygonsconnectionString = "Data Source=OpenCVPolygons.db";
        public bool microcontrollerON = true;
        public string microcontrollerIP = "http://192.168.0.120/";
        //public string microcontrollerIP = "http://10.18.28.240/";

        public void ChangeSpotFromFinalState(long id)
        {
            PSTotalResultsItem? currentItem = null;
            try
            {
                using (var conn = new SqliteConnection(psfinalconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"SELECT * FROM PSTotalResultsItems WHERE Id={id};";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            currentItem = new PSTotalResultsItem(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("vehicle"))),
                                Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("objectInSpot"))),
                                Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("parkingSpaceObstructed")))
                            );
                        }
                    }
                }

            }
            catch (SqliteException e)
            {


            }

            if (currentItem is not null)
            {
                try
                {
                    using (var conn = new SqliteConnection(connectionString))
                    {
                        conn.Open();
                        var command = conn.CreateCommand();
                        command.CommandText = $"UPDATE ParkingSpaceItems SET " +
                            $"occupied={currentItem.convertedSpot.occupied}, " +
                            $"maintenanceAlert={currentItem.convertedSpot.maintenanceAlert} " +
                            $"WHERE id={currentItem.Id}";
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqliteException ex)
                {

                }
            }
        }

        //called by the OpenCVResultsController
        public async Task CheckForMicrocontrollerData(long id)
        {
            ObjectInSpotItem? checkedItem = null; //microcontroller data
            try
            {
                using (var conn = new SqliteConnection(objectinspotconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"SELECT * FROM ObjectInSpotItems WHERE Id={id};";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            checkedItem = new ObjectInSpotItem(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("objectInSpot"))),
                                reader.GetInt32(reader.GetOrdinal("error"))
                            );
                        }
                    }
                }

            }
            catch (SqliteException e)
            {

            }

            if (checkedItem is null)
            {
                //we need to grab fresh data from the microcontroller.
                try
                {
                    if (microcontrollerON)
                    {
                        var http = new HttpClient();
                        var json = await http.GetStringAsync(microcontrollerIP);

                        var doc = JsonDocument.Parse(json);
                        int occupied = doc.RootElement.GetProperty("occupied").GetInt32();
                        int error = doc.RootElement.GetProperty("error").GetInt32();
                        checkedItem = new ObjectInSpotItem(
                            id,
                            Convert.ToBoolean(occupied),
                            error
                        );
                    }
                    else
                    {
                        checkedItem = new ObjectInSpotItem(
                            id, 
                            true,
                            2 //2 is the error code for a disconnected microcontroller
                        );

                    }



                }
                catch (Exception e)
                {
                    checkedItem = new ObjectInSpotItem(
                        id,
                        true,
                        2
                    );
                }
            }

            if (checkedItem is not null)
            {
                OpenCVResultsItem? openCVCorrespondingItem = null; //results of the YOLO object detection model
                using (var conn = new SqliteConnection(openCVConnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"SELECT * FROM OpenCVResultsItems WHERE Id={id};";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            openCVCorrespondingItem = new OpenCVResultsItem(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("vehicle"))),
                                Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("parkingSpaceObstructed")))
                            );
                        }
                    }

                    //deleting the other item so that we don't trigger an incorrect condition
                    command.CommandText = $"DELETE FROM OpenCVResultsItems Where Id = {id}";
                    command.ExecuteNonQuery();

                }


                //deleting the other item so that we don't trigger an incorrect condition
                using (var conn = new SqliteConnection(objectinspotconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"DELETE FROM ObjectInSpotItems Where Id={id}";
                    command.ExecuteNonQuery();
                }


                PSTotalResultsItem currentItem = new PSTotalResultsItem(openCVCorrespondingItem, checkedItem);

                //if currentItem is essentially not null
                if (currentItem.Id != -1)
                {
                    try
                    {
                        using (var conn = new SqliteConnection(connectionString))
                        {
                            conn.Open();
                            var command = conn.CreateCommand();
                            command.CommandText = $"UPDATE ParkingSpaceItems SET " +
                                $"occupied={currentItem.convertedSpot.occupied}, " +
                                $"maintenanceAlert={currentItem.convertedSpot.maintenanceAlert} " +
                                $"WHERE id={currentItem.Id}";
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (SqliteException ex)
                    {

                    }

                    try
                    {
                        using (var conn = new SqliteConnection(psfinalconnectionString))
                        {
                            conn.Open();
                            var command = conn.CreateCommand();
                            command.CommandText = $"UPDATE PSTotalResultsItems SET " +
                                $"vehicle={currentItem.vehicle}, " +
                                $"objectInSpot={currentItem.objectInSpot}, " +
                                $"parkingSpaceObstructed={currentItem.parkingSpaceObstructed}, " +
                                $"sensorConnectedToNetwork={currentItem.sensorConnectedToNetwork} " +
                                $"WHERE id={currentItem.Id}";
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (SqliteException ex)
                    {

                    }

                    try
                    {
                        //microcontrollerON is a variable basically just for quick debugging of features when a microcontroller is not on the network.
                        //in production, it would not exist.
                        if (microcontrollerON)
                        {
                            //here, we send the microcontroller the error code so that it can turn on the appropriate LED. The microcontroller only cares about the error code, so that's all we send it.

                            var errorState = JsonConvert.SerializeObject(Convert.ToInt64(currentItem.convertedSpot.maintenanceAlert));
                            var http = new HttpClient();
                            var content = new StringContent(errorState, System.Text.Encoding.UTF8, "text/plain");
                            var response = await http.PostAsync(microcontrollerIP + "error", content);
                            string json = await response.Content.ReadAsStringAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                }

            }
        }

        //called by the ObjectInSpotController
        public void CheckForOpenCVData(long id)
        {
            OpenCVResultsItem? checkedItem = null;
            try
            {
                using (var conn = new SqliteConnection(openCVConnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"SELECT * FROM OpenCVResultsItems WHERE Id={id};";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            checkedItem = new OpenCVResultsItem(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("vehicle"))),
                                Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("parkingSpaceObstructed")))
                            );
                        }
                    }
                }

            }
            catch (SqliteException e)
            {


            }

            if (checkedItem is not null)
            {
                ObjectInSpotItem? objectInSpotItem = null;
                try
                {
                    using (var conn = new SqliteConnection(objectinspotconnectionString))
                    {
                        conn.Open();
                        var command = conn.CreateCommand();
                        command.CommandText = $"SELECT * FROM ObjectInSpotItems WHERE Id={id};";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                objectInSpotItem = new ObjectInSpotItem(
                                    reader.GetInt32(reader.GetOrdinal("Id")),
                                    Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("objectInSpot"))),
                                    reader.GetInt32(reader.GetOrdinal("error"))
                                );
                            }
                        }

                        //deleting the other item so that we don't trigger an incorrect condition
                        command.CommandText = $"DELETE FROM ObjectInSpotItems Where Id={id}";
                        command.ExecuteNonQuery();

                    }

                    //deleting the other item so that we don't trigger an incorrect condition
                    using (var conn = new SqliteConnection(openCVConnectionString))
                    {
                        conn.Open();
                        var command = conn.CreateCommand();
                        command.CommandText = $"DELETE FROM OpenCVResultsItems Where Id={id}";
                        command.ExecuteNonQuery();
                    }

                }

                catch (SqliteException ex)
                {

                }


                PSTotalResultsItem currentItem = new PSTotalResultsItem(checkedItem, objectInSpotItem);

                try
                {
                    using (var conn = new SqliteConnection(connectionString))
                    {
                        conn.Open();
                        var command = conn.CreateCommand();
                        command.CommandText = $"UPDATE ParkingSpaceItems SET " +
                            $"occupied={currentItem.convertedSpot.occupied}, " +
                            $"maintenanceAlert={currentItem.convertedSpot.maintenanceAlert} " +
                            $"WHERE id={currentItem.Id}";
                        command.ExecuteNonQuery();

                    }
                }
                catch (SqliteException ex)
                {

                }
            }
        }

        //just for testing
        public async Task createnewPSTotalEntry(PSTotalResultsItem todoItem)
        {
            try
            {
                using (var conn = new SqliteConnection(psfinalconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"INSERT INTO PSTotalResultsItems " +
                        $"VALUES ({todoItem.Id}," +
                        $" {todoItem.vehicle}," +
                        $" {todoItem.objectInSpot}," +
                        $" {todoItem.parkingSpaceObstructed})";
                    command.ExecuteNonQuery();
                }
            }
            catch (SqliteException ex)
            {

            }
        }

        //Saving the results of the YOLO object detection model to the database. Primarly used in OpenCVManager.
        public async Task CreateNewOpenCVResultsEntry(OpenCVResultsItem todoItem) //fix this so that it can check if an object exists already. Gotta have maximum safety.
        {
            try
            {
                using (var conn = new SqliteConnection(openCVConnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"INSERT INTO OpenCVResultsItems " +
                        $"VALUES ({todoItem.Id}," +
                        $" {todoItem.vehicle}," +
                        $" {todoItem.parkingSpaceObstructed})";
                    command.ExecuteNonQuery();
                }
            }
            catch (SqliteException ex)
            {

            }
        }

        //just for testing
        public async Task createNewObjectInSpotEntry(ObjectInSpotItem todoItem)
        {
            try
            {
                using (var conn = new SqliteConnection(objectinspotconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"INSERT INTO ObjectInSpotItems " +
                        $"VALUES ({todoItem.Id}," +
                        $" {todoItem.objectInSpot})";
                    command.ExecuteNonQuery();
                }
            }
            catch (SqliteException ex)
            {

            }
        }

        //Saves the boundaries of the parking space created by the user.
        public async Task createNewPolygonEntry(OpenCvSharp.Point[][] todoItem, int Id)
        {
            try
            {
                using (var conn = new SqliteConnection(opencvpolygonsconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    string jsonPolygons = JsonConvert.SerializeObject(todoItem);
                    command.CommandText = $"INSERT INTO OpenCVPolygonsItems " +
                        $"VALUES ({Id}," +
                        $" '{jsonPolygons}')";
                    command.ExecuteNonQuery();
                }
            }
            catch (SqliteException ex)
            {

            }
        }

        //Retrieves the boundaries of the parking space created by the user.
        public OpenCvSharp.Point[][] OpenPolygonEntry(int Id)
        {
            OpenCvSharp.Point[][]? polygons = null;

            try
            {
                using (var conn = new SqliteConnection(opencvpolygonsconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"SELECT * FROM OpenCVPolygonsItems WHERE Id={Id};";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            polygons = JsonConvert.DeserializeObject<OpenCvSharp.Point[][]>(reader.GetString(reader.GetOrdinal("polygonPoints")));
                        }
                    }
                }
            }

            catch (SqliteException ex)
            {

            }

            return polygons;
        }
    }
}
