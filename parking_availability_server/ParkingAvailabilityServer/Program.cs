// See https://aka.ms/new-console-template for more information
using ParkingAvailabilityServer;

Console.WriteLine("Hello, World!");
DatabaseManager dbManager = new();

dbManager.CreateTable();


public struct ParkingLotState(bool newvehicle, bool newobstructed)
{
    bool vehicle = newvehicle;
    bool obstructed = newobstructed;

    public bool[] GetAllParkingLotState()
    {
        return [vehicle, obstructed];
    }
}
