using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

#if NET6_0_OR_GREATER
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Sample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
}