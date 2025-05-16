using System;
using System.Threading.Tasks;

namespace ConveyorApp.Models;

public class Mechanic : IMechanic
{
    private readonly Random _random = new();
    
    public event Action<string>? RepairStarted;
    public event Action<string>? RepairCompleted;

    public async void RepairConveyor(Conveyor conveyor)
    {
        var repairTime = _random.Next(3000, 8000);
        RepairStarted?.Invoke($"Repair started. Estimated time: {repairTime/1000} sec");
        
        await Task.Delay(repairTime);
        
        RepairCompleted?.Invoke($"Conveyor repaired in {repairTime/1000} seconds");
    }
}