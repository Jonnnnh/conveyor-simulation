using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;
using ConveyorApp.Models;
using ReactiveUI;

namespace ConveyorApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMechanic _mechanic;
    private readonly ObservableCollection<ConveyorViewModel> _conveyors = new();
    private readonly StringBuilder _logBuilder = new();
    private string _logText = string.Empty;

    public MainWindowViewModel(IMechanic mechanic)
    {
        _mechanic = mechanic;
        
        _mechanic.RepairStarted += OnRepairStarted;
        _mechanic.RepairCompleted += OnRepairCompleted;
        
        AddConveyorCommand = ReactiveCommand.Create(AddConveyor);
        RemoveConveyorCommand = ReactiveCommand.Create<ConveyorViewModel>(RemoveConveyor);
        Conveyors = new ReadOnlyObservableCollection<ConveyorViewModel>(_conveyors);
    }

    public ReactiveCommand<Unit, Unit> AddConveyorCommand { get; }
    public ReactiveCommand<ConveyorViewModel, Unit> RemoveConveyorCommand { get; }
    public ReadOnlyObservableCollection<ConveyorViewModel> Conveyors { get; }

    public string LogText
    {
        get => _logText;
        set => this.RaiseAndSetIfChanged(ref _logText, value);
    }

    private void AddConveyor()
    {
        var loader = new Loader();
        var conveyor = new Conveyor(_mechanic);
        var conveyorVM = new ConveyorViewModel(conveyor, loader, _mechanic);
        
        conveyorVM.StatusChanged += OnConveyorStatusChanged;
        
        _conveyors.Add(conveyorVM);
        AddLog($"Added new conveyor (Total: {_conveyors.Count})");
    }

    private void RemoveConveyor(ConveyorViewModel conveyorVM)
    {
        conveyorVM.StatusChanged -= OnConveyorStatusChanged;
        conveyorVM.Dispose();
        _conveyors.Remove(conveyorVM);
        AddLog($"Removed conveyor (Remaining: {_conveyors.Count})");
    }

    private void OnConveyorStatusChanged(string message)
    {
        AddLog($"Conveyor: {message}");
    }

    private void OnRepairStarted(string message)
    {
        AddLog($"Mechanic: {message}");
    }

    private void OnRepairCompleted(string message)
    {
        AddLog($"Mechanic: {message}");
    }

    private void AddLog(string message)
    {
        _logBuilder.AppendLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        LogText = _logBuilder.ToString();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _mechanic.RepairStarted -= OnRepairStarted;
            _mechanic.RepairCompleted -= OnRepairCompleted;
            
            foreach (var conveyor in _conveyors)
            {
                conveyor.StatusChanged -= OnConveyorStatusChanged;
                conveyor.Dispose();
            }
        }
        
        base.Dispose(disposing);
    }
}