using System;

namespace ConveyorApp.Models;

public interface IMechanic
{
    void RepairConveyor(Conveyor conveyor);
    event Action<string>? RepairStarted;
    event Action<string>? RepairCompleted;
}