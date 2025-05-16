using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ConveyorApp.Models;
using ConveyorApp.ViewModels;

namespace ConveyorApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var mechanic = new Mechanic();
        DataContext = new MainWindowViewModel(mechanic);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}