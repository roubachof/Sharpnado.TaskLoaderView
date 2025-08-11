using Sample;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;

namespace Retronado.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureTaskLoader(true)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Font-Awesome-5-Free-Solid-900.otf", "FontAwesome");
                fonts.AddFont("ka1.ttf", "FontKarmatic");
                fonts.AddFont("atarist.ttf", "FontAtariSt");
                fonts.AddFont("ac.ttf", "FontArcadeClassic");
            });

        builder.Services.AddLocalization();

        var entryPoint = new CoreEntryPoint();
        entryPoint.RegisterDependencies();

        TaskMonitorConfiguration.ConsiderCanceledAsFaulted = true;

        Initializer.Initialize(true, true);

        return builder.Build();
    }
}
