namespace Retronado.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Font-Awesome-5-Free-Solid-900.otf", "FontAwesome");
                fonts.AddFont("ka1.ttf", "FontKarmatic");
                fonts.AddFont("atarist.ttf", "FontAtariSt");
                fonts.AddFont("ac.ttf", "FontArcadeClassic");
            });

        builder.Services.AddLocalization();

        return builder.Build();
    }
}
