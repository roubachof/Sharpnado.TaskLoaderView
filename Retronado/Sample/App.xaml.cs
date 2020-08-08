using System.Net;
using Sample.Views;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;

using Xamarin.Forms;

namespace Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
#if DEBUG
            // HotReloader.Current.Run(this); //, new HotReloader.Configuration
            // {
                // optionaly you may specify EXTENSION's IP (ExtensionIpAddress)
                // in case your PC/laptop and device are in different subnets
                // e.g. Laptop - 10.10.102.16 AND Device - 10.10.9.12
                // ExtensionIpAddress = IPAddress.Parse("10.0.2.2"), // use your PC/Laptop IP
            // });
#endif
            var entryPoint = new CoreEntryPoint();
            entryPoint.RegisterDependencies();

            Initializer.Initialize(true, true);
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
