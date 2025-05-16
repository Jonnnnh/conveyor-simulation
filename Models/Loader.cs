using System;
using System.Threading.Tasks;

namespace ConveyorApp.Models;

public class Loader
{
    private readonly Random _random = new();

    public event Action<string>? StatusChanged;
    public event Action<int>? MaterialsLoaded;

    public async Task LoadMaterialsAsync()
    {
        StatusChanged?.Invoke("Loader started working...");
        await Task.Delay(2000);
        
        var materials = _random.Next(50, 150);
        MaterialsLoaded?.Invoke(materials);
        StatusChanged?.Invoke($"Loaded {materials} materials");
    }
}