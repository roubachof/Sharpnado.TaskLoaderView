using Sample;
using Sample.Views;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;

namespace Retronado.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new NavigationPage(new MainPage()));
    }
}
