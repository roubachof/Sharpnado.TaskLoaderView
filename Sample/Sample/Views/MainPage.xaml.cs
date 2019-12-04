using System;
using System.ComponentModel;

using Sample.Infrastructure;
using Sample.Navigation;
using Sample.ViewModels;

using Sharpnado.Tasks;

using Xamarin.Forms;

namespace Sample.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage, IBindablePage
    {
        private readonly INavigationService _navigationService;

        public MainPage()
        {
            InitializeComponent();
            _navigationService = DependencyContainer.Instance.GetInstance<INavigationService>();
        }

        private void ButtonDefaultLayoutOnClicked(object sender, EventArgs e)
        {
            TaskMonitor.Create(_navigationService.NavigateToAsync<DefaultLayoutViewModel>());
        }
    }
}
