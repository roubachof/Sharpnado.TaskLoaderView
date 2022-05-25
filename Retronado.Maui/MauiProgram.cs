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
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("ka1.ttf", "FontKarmatic");
				fonts.AddFont("atarist.ttf", "FontKarmatic");
				fonts.AddFont("ka1.ttf", "FontKarmatic");
				fonts.AddFont("ka1.ttf", "FontKarmatic");
			});

		return builder.Build();
	}
}
