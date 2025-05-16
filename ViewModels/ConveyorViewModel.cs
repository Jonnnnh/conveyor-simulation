using System;
using System.Reactive;
using Avalonia.Threading;
using ConveyorApp.Models;
using ReactiveUI;

namespace ConveyorApp.ViewModels;

public class ConveyorViewModel : ViewModelBase
{
    private readonly Conveyor _conveyor;
    private readonly Loader _loader;
    private string _status = "Not started";
    private int _producedCount;
    private int _materialCount;
    public event Action<string>? StatusChanged;

    public ConveyorViewModel(Conveyor conveyor, Loader loader, IMechanic mechanic)
    {
        _conveyor = conveyor;
        _loader = loader;

        _conveyor.StatusChanged += message => 
        {
            StatusChanged?.Invoke(message);
            OnStatusChanged(message);
        };
        _conveyor.MaterialProduced += OnMaterialProduced;
        _conveyor.MaterialsEmpty += OnMaterialsEmpty;
        _conveyor.ConveyorBroken += OnConveyorBroken;
        _conveyor.ConveyorRepaired += OnConveyorRepaired;

        _loader.MaterialsLoaded += OnMaterialsLoaded;

        StartCommand = ReactiveCommand.Create(Start);
        StopCommand = ReactiveCommand.Create(Stop);
    }

    public string Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public int ProducedCount
    {
        get => _producedCount;
        set => this.RaiseAndSetIfChanged(ref _producedCount, value);
    }

    public int MaterialCount
    {
        get => _materialCount;
        set => this.RaiseAndSetIfChanged(ref _materialCount, value);
    }

  public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    private void Start() => _conveyor.Start();
    private void Stop() => _conveyor.Stop();

    private void OnStatusChanged(string message)
    {
        Dispatcher.UIThread.Post(() => Status = message);
    }

    private void OnMaterialProduced()
    {
        Dispatcher.UIThread.Post(() => ProducedCount = _conveyor.ProducedItemsCount);
    }

    private async void OnMaterialsEmpty()
    {
        await _loader.LoadMaterialsAsync();
    }

    private void OnMaterialsLoaded(int count)
    {
        _conveyor.AddMaterials(count);
        Dispatcher.UIThread.Post(() => MaterialCount = _conveyor.MaterialCount);
    }

    private void OnConveyorBroken()
    {
        Dispatcher.UIThread.Post(() => Status = "Conveyor broken! Waiting for mechanic...");
    }

    private void OnConveyorRepaired()
    {
        Dispatcher.UIThread.Post(() => Status = "Conveyor repaired and running");
    }

    protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _conveyor.StatusChanged -= OnStatusChanged;
                _conveyor.MaterialProduced -= OnMaterialProduced;
                _conveyor.MaterialsEmpty -= OnMaterialsEmpty;
                _conveyor.ConveyorBroken -= OnConveyorBroken;
                _conveyor.ConveyorRepaired -= OnConveyorRepaired;
                _loader.MaterialsLoaded -= OnMaterialsLoaded;
                _conveyor.Dispose();
            }
        
            base.Dispose(disposing);
        }
}