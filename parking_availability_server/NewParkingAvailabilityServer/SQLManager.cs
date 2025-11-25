using Microsoft.Data.Sqlite;
using NewParkingAvailabilityServer.Models;

namespace NewParkingAvailabilityServer
{
    public class SQLManager
    {
        public string connectionString = "Data Source=ParkingSpots.db";
        public string psfinalconnectionString = "Data Source=PSFinal.db";

        public void ChangeSpotFromFinalState(long id)
        {
            PSTotalResultsItem? currentItem = null;

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
    }
}
