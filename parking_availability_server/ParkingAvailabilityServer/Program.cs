// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


public struct ParkingLotState(bool newvehicle, bool newobstructed)
{
    bool vehicle = newvehicle;
    bool obstructed = newobstructed;

    public bool[] GetAllParkingLotState()
    {
        return [vehicle, obstructed];
    }
}

