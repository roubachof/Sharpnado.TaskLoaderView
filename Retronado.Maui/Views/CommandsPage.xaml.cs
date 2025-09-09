using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace Sample.Views;

public partial class CommandsPage : ContentPage, IBindablePage
{
    public CommandsPage()
    {
        InitializeComponent();

        ResourcesHelper.SetSublimeGameMode();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var safeInsets = On<iOS>().SafeAreaInsets();
        safeInsets.Bottom = 0;
        Padding = safeInsets;
    }
}