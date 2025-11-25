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
            PSTotalResultsItem currentItem;

            using (var conn = new SqliteConnection(psfinalconnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = $"SELECT * FROM PSTotalResultsItems LIMIT 1 OFFSET {id-1};";
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

            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = $"";
            }


        }
    }
}
