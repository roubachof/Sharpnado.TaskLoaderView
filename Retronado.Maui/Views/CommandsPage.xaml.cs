using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Sharpnado.TaskLoaderView;

namespace Sample.Views;

public partial class CommandsPage : ContentPage, IBindablePage
{
    public CommandsPage()
    {
        InitializeComponent();

        ResourcesHelper.SetSublimeGameMode();

        // Subscribe to TemplatedTaskLoader events
        TaskLoader.LoadingControlTemplateLoaded += OnLoadingControlTemplateLoaded;
        TaskLoader.ResultControlTemplateLoaded += OnResultControlTemplateLoaded;
        TaskLoader.ErrorControlTemplateLoaded += OnErrorControlTemplateLoaded;
        TaskLoader.EmptyControlTemplateLoaded += OnEmptyControlTemplateLoaded;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var safeInsets = On<iOS>().SafeAreaInsets();
        safeInsets.Bottom = 0;
        Padding = safeInsets;
    }

    private void OnLoadingControlTemplateLoaded(object? sender, ControlTemplateLoadedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[CommandsPage] Loading template loaded. View type: {e.View?.GetType().Name}");

        if (e.View is ActivityIndicator indicator)
        {
            System.Diagnostics.Debug.WriteLine($"[CommandsPage] ActivityIndicator found - IsRunning: {indicator.IsRunning}, Color: {indicator.Color}");
        }
    }

    private void OnResultControlTemplateLoaded(object? sender, ControlTemplateLoadedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[CommandsPage] Result template loaded. View type: {e.View?.GetType().Name}");

        if (e.View is Grid grid)
        {
            System.Diagnostics.Debug.WriteLine($"[CommandsPage] Grid found with {grid.Children.Count} children");
        }

        var button = TaskLoader.GetTemplateChild("BuyItButton");
        System.Diagnostics.Debug.WriteLine("[CommandsPage] BuyIt Button found: " + button);
    }

    private void OnErrorControlTemplateLoaded(object? sender, ControlTemplateLoadedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[CommandsPage] Error template loaded. View type: {e.View?.GetType().Name}");

        if (e.View is StackLayout stackLayout)
        {
            System.Diagnostics.Debug.WriteLine($"[CommandsPage] StackLayout found with {stackLayout.Children.Count} children");
        }
    }

    private void OnEmptyControlTemplateLoaded(object? sender, ControlTemplateLoadedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[CommandsPage] Empty template loaded. View type: {e.View?.GetType().Name}");
    }
}
