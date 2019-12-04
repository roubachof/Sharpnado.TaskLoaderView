using System;
using Sample.Infrastructure;
using Sample.Navigation;

using Sharpnado.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationToolBar : ContentView
    {
        private readonly INavigationService _navigationService;

        public NavigationToolBar()
        {
            InitializeComponent();
            _navigationService = DependencyContainer.Instance.GetInstance<INavigationService>();
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            TaskMonitor.Create(_navigationService.NavigateBackAsync());
        }
    }
}