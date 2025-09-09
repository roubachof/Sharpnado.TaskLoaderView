using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace Sample.Views;

public partial class DefaultViewsSkeletonPage : ContentPage, IBindablePage
{
    public DefaultViewsSkeletonPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ResourcesHelper.SetSublimeGameMode();

        var safeInsets = On<iOS>().SafeAreaInsets();
        safeInsets.Bottom = 0;
        Padding = safeInsets;
    }
}