using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ConveyorApp.Views;

public partial class ConveyorView : UserControl
{
    public ConveyorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}