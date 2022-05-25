using System.Threading;
using System.Threading.Tasks;

using Sharpnado.Tasks;

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
    public partial class UserViewsPage : ContentPage, IBindablePage
    {
        private CancellationTokenSource _animationTokenSource;

        public UserViewsPage()
        {
            InitializeComponent();

            BusyImage.PropertyChanged += BusyImagePropertyChanged;
            ErrorNotificationView.PropertyChanged += ErrorNotificationViewPropertyChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ResourcesHelper.SetTosCellMode();

            var safeInsets = On<iOS>().SafeAreaInsets();
            safeInsets.Bottom = 0;
            Padding = safeInsets;
        }

        private void BusyImagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsVisible))
            {
                if (BusyImage.IsVisible)
                {
                    _animationTokenSource = new CancellationTokenSource();
                    TaskMonitor.Create(StartBeeAnimationAsync(_animationTokenSource.Token));
                }
                else
                {
                    _animationTokenSource?.Cancel();
                }
            }
        }

        private void ErrorNotificationViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsVisible))
            {
                if (ErrorNotificationView.IsVisible)
                {
                    TaskMonitor.Create(ErrorNotificationView.ScaleTo(1, 500));
                }
                else
                {
                    ErrorNotificationView.Scale = 0;
                }
            }
        }

        private async Task StartBeeAnimationAsync(CancellationToken cancellationToken)
        {
            const double deltaYMove = 15;
            const double deltaXMove = 30;

            await BusyImage.TranslateTo(deltaXMove, -deltaYMove, 125);
            while (!cancellationToken.IsCancellationRequested)
            {
                await BusyImage.TranslateTo(deltaXMove, 2 * deltaYMove, 200);
                await BusyImage.TranslateTo(2 * -deltaXMove, 2 * -deltaYMove);
                await BusyImage.TranslateTo(2 * -deltaXMove, 2 * deltaYMove, 200);
                await BusyImage.TranslateTo(deltaXMove, -deltaYMove);
            }
        }
    }
}
