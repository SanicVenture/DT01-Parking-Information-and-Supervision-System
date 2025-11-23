using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace ParkingAvailabilityServer
{
    public class DatabaseManager
    {
        public void CreateTable()
        {
            try
            {
                string connectionString = "Data Source=../../../ParkingSpots.db;";
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string createTable = @"
                        CREATE TABLE IF NOT EXISTS ParkingSpaces (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Floor INTEGER,
                            Occupied INTEGER
                        );";
                    using (var command = new SqliteCommand(createTable, connection))
                    {
                        command.ExecuteNonQuery();

                    }
                }
            }

            catch (SqliteException ex)
            {
                Console.WriteLine(ex);
            }
        }


    }
}
