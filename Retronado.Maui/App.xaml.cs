using Sample;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;

namespace Retronado.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		var entryPoint = new CoreEntryPoint();
		entryPoint.RegisterDependencies();

		TaskMonitorConfiguration.ConsiderCanceledAsFaulted = true;

		Initializer.Initialize(true, true);
		MainPage = new NavigationPage(new MainPage());
	}
}
