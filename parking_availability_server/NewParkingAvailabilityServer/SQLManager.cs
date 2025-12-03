using Microsoft.Data.Sqlite;
using NewParkingAvailabilityServer.Models;

namespace NewParkingAvailabilityServer
{
    public class SQLManager
    {
        public string connectionString = "Data Source=ParkingSpots.db";
        public string psfinalconnectionString = "Data Source=PSFinal.db";
        public string objectinspotconnectionString = "Data Source=ObjectInSpot.db";
        public string opencvconnectionString = "Data Source=OpenCVResults.db";

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
                    //command.CommandText = $"UPDATE PSTotalResultsItems SET " +
                    //    $"vehicle={-1}, " +
                    //    $"objectInSpot={-1}, " +
                    //    $"parkingSpaceObstructed={-1} " +
                    //    $"WHERE Id = {id}";
                    //command.ExecuteNonQuery();
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
        public void CheckForMicrocontrollerData(long id)
        {
            ObjectInSpotItem? checkedItem = null;
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
                                Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("objectInSpot")))
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
                OpenCVResultsItem? openCVCorrespondingItem = null;
                using (var conn = new SqliteConnection(opencvconnectionString))
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

        //called by the ObjectInSpotController
        public void CheckForOpenCVData(long id)
        {
            OpenCVResultsItem? checkedItem = null;
            try
            {
                using (var conn = new SqliteConnection(opencvconnectionString))
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
                                    Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("objectInSpot")))
                                );
                            }
                        }

                        //deleting the other item so that we don't trigger an incorrect condition
                        command.CommandText = $"DELETE FROM ObjectInSpotItems Where Id={id}";
                        command.ExecuteNonQuery();

                    }

                    //deleting the other item so that we don't trigger an incorrect condition
                    using (var conn = new SqliteConnection(opencvconnectionString))
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

                        //command.CommandText = $"UPDATE PSTotalResultsItems SET " +
                        //    $"vehicle={-1}, " +
                        //    $"objectInSpot={-1}, " +
                        //    $"parkingSpaceObstructed={-1} " +
                        //    $"WHERE Id = {id}";
                        //command.ExecuteNonQuery();
                    }
                }
                catch (SqliteException ex)
                {

                }
            }
        }

        public async Task createnewPSTotalEntry(PSTotalResultsItem todoItem)
        {
            try
            {
                using (var conn = new SqliteConnection(psfinalconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"INSERT INTO PSTotalResultsItems " +
                        $"VALUES ({todoItem.Id}, {todoItem.vehicle}, {todoItem.objectInSpot}, {todoItem.parkingSpaceObstructed})";
                    command.ExecuteNonQuery();
                }
            }
            catch (SqliteException ex)
            {

            }
        }

        public async Task createnewOpenCVResultsEntry(OpenCVResultsItem todoItem) //fix this so that it can check if an object exists already. Gotta have maximum safety.
        {
            try
            {
                using (var conn = new SqliteConnection(opencvconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"INSERT INTO OpenCVResultsItems " +
                        $"VALUES ({todoItem.Id}, {todoItem.vehicle}, {todoItem.parkingSpaceObstructed})";
                    command.ExecuteNonQuery();
                }
            }
            catch (SqliteException ex)
            {

            }
        }

        public async Task createNewObjectInSpotEntry(ObjectInSpotItem todoItem)
        {
            try
            {
                using (var conn = new SqliteConnection(objectinspotconnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = $"INSERT INTO ObjectInSpotItems " +
                        $"VALUES ({todoItem.Id}, {todoItem.objectInSpot})";
                    command.ExecuteNonQuery();
                }
            }
            catch (SqliteException ex)
            {

            }
        }
    }
}
