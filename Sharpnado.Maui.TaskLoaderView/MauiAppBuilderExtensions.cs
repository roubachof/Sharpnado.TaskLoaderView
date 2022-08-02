namespace Sharpnado.TaskLoaderView;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder ConfigureTaskLoader(
        this MauiAppBuilder builder,
        bool loggerEnable,
        bool debugLogEnable = false,
        Initializer.LoggerDelegate? loggerDelegate = null)
    {
        InternalLogger.EnableDebug = debugLogEnable;
        InternalLogger.EnableLogging = loggerEnable;
        InternalLogger.LoggerDelegate = loggerDelegate;
        return builder;
    }
}
