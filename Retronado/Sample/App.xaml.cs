using System.Net;
using Sample.Views;

using Sharpnado.MaterialFrame;
using Sharpnado.Tasks;

using Xamarin.Forms;

using Initializer = Sharpnado.TaskLoaderView.Initializer;

namespace Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var entryPoint = new CoreEntryPoint();
            entryPoint.RegisterDependencies();

            TaskMonitorConfiguration.ConsiderCanceledAsFaulted = true;

            Sharpnado.MaterialFrame.Initializer.Initialize(false, false);

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
