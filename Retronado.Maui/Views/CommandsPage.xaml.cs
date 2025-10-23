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

    private void OnLoadingControlTemplateLoaded(object? sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("[CommandsPage] Loading template loaded");
    }

    private void OnResultControlTemplateLoaded(object? sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("[CommandsPage] Result template loaded");
    }

    private void OnErrorControlTemplateLoaded(object? sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("[CommandsPage] Error template loaded");
    }

    private void OnEmptyControlTemplateLoaded(object? sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("[CommandsPage] Empty template loaded");
    }
}
