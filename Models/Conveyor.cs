using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConveyorApp.Models;

public class Conveyor : IDisposable
{
    private readonly Random _random = new();
    private readonly IMechanic _mechanic;
    private CancellationTokenSource? _cts;
    private bool _isRunning;
    private int _materialCount;

    public event Action<string>? StatusChanged;
    public event Action? MaterialsEmpty;
    public event Action? ConveyorBroken;
    public event Action? ConveyorRepaired;
    public event Action? MaterialProduced;

    public Conveyor(IMechanic mechanic, int initialMaterials = 100)
    {
        _mechanic = mechanic;
        _materialCount = initialMaterials;
        _mechanic.RepairCompleted += OnRepairCompleted;
    }

    public int MaterialCount
    {
        get => _materialCount;
        private set
        {
            _materialCount = value;
            StatusChanged?.Invoke($"Materials: {_materialCount}");
        }
    }

    public bool IsBroken { get; private set; }
    public int ProducedItemsCount { get; private set; }

    public void Start()
    {
        if (_isRunning) return;
        
        _isRunning = true;
        _cts = new CancellationTokenSource();
        Task.Run(() => RunConveyor(_cts.Token));
        StatusChanged?.Invoke("Conveyor started");
    }

    public void Stop()
    {
        _isRunning = false;
        _cts?.Cancel();
        StatusChanged?.Invoke("Conveyor stopped");
    }

    public void AddMaterials(int count)
    {
        MaterialCount += count;
        StatusChanged?.Invoke($"Added {count} materials");
    }

    private async Task RunConveyor(CancellationToken cancellationToken)
    {
        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            if (IsBroken)
            {
                await Task.Delay(1000, cancellationToken);
                continue;
            }

            if (MaterialCount <= 0)
            {
                MaterialsEmpty?.Invoke();
                await Task.Delay(2000, cancellationToken);
                continue;
            }

            if (_random.Next(0, 100) < 5)
            {
                BreakConveyor();
                continue;
            }

            await Task.Delay(500, cancellationToken);
            MaterialCount--;
            ProducedItemsCount++;
            MaterialProduced?.Invoke();
        }
    }

    private void BreakConveyor()
    {
        IsBroken = true;
        StatusChanged?.Invoke("Conveyor broken!");
        ConveyorBroken?.Invoke();
        _mechanic.RepairConveyor(this);
    }

    private void OnRepairCompleted(string message)
    {
        if (IsBroken)
        {
            IsBroken = false;
            ConveyorRepaired?.Invoke();
            StatusChanged?.Invoke("Conveyor repaired and running");
        }
    }

    public void Dispose()
    {
        Stop();
        _cts?.Dispose();
        _mechanic.RepairCompleted -= OnRepairCompleted;
    }
}