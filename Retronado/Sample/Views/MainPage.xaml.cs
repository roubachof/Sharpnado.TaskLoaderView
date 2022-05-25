using System;
using System.ComponentModel;

using Sample.Domain;
using Sample.Infrastructure;
using Sample.Navigation;
using Sample.ViewModels;

using Sharpnado.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

#if NET6_0_OR_GREATER
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Sample.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage, IBindablePage
    {
        private readonly INavigationService _navigationService;

        private readonly IRetroGamingService _retroGamingService;

        public MainPage()
        {
            InitializeComponent();
            _navigationService = DependencyContainer.Instance.GetInstance<INavigationService>();
            _retroGamingService = DependencyContainer.Instance.GetInstance<IRetroGamingService>();

            LoadOnDemandCard.BindingContext = new LoadOnDemandViewModel(_retroGamingService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var safeInsets = On<iOS>().SafeAreaInsets();
            safeInsets.Bottom = 0;
            Padding = safeInsets;

            ResourcesHelper.SetSublimeGameMode();
        }

        private void ButtonDefaultLayoutOnClicked(object sender, EventArgs e)
        {
            // TaskMonitor.Create(_navigationService.NavigateToViewAsync<DefaultIsBusyViewsPage>(GamePlatform.Console));
            TaskMonitor.Create(_navigationService.NavigateToViewAsync<DefaultViewsPage>(GamePlatform.Console));
        }

        private void ButtonLottieLayoutOnClicked(object sender, EventArgs e)
        {
            TaskMonitor.Create(_navigationService.NavigateToViewAsync<LottieViewsPage>(GamePlatform.Console));
        }

        private void ButtonCustomLayoutOnClicked(object sender, EventArgs e)
        {
            TaskMonitor.Create(_navigationService.NavigateToViewAsync<UserViewsPage>(GamePlatform.Computer));
        }

        private void SkeletonLoadingOnClicked(object sender, EventArgs e)
        {
            TaskMonitor.Create(_navigationService.NavigateToViewAsync<DefaultViewsSkeletonPage>(GamePlatform.Computer));
        }

        private void CommandsOnClicked(object sender, EventArgs e)
        {
            TaskMonitor.Create(_navigationService.NavigateToViewAsync<CommandsPage>(GamePlatform.Computer));
        }
    }
}
