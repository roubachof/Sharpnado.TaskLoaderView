using System.Threading;
using System.Threading.Tasks;

using Sharpnado.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace Sample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserViewsPage : ContentPage, IBindablePage
    {
        private CancellationTokenSource _animationTokenSource;

        public UserViewsPage()
        {
            InitializeComponent();

            ResourcesHelper.SetTosCellMode();

            BusyImage.PropertyChanged += BusyImagePropertyChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

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

        private async Task StartBeeAnimationAsync(CancellationToken cancellationToken)
        {
            const double deltaMove = 15;

            while (!cancellationToken.IsCancellationRequested)
            {
                await BusyImage.TranslateTo(deltaMove, -deltaMove, 125);
                await BusyImage.TranslateTo(deltaMove, 2 * deltaMove, 200);
                await BusyImage.TranslateTo(2 * -deltaMove, 2 * -deltaMove);
                await BusyImage.TranslateTo(2 * -deltaMove, 2 * deltaMove, 200);
                await BusyImage.TranslateTo(0, 0, 100);
            }
        }
    }
}
