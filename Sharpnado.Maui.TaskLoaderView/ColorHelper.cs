using Xamarin.Forms;

namespace Sharpnado.TaskLoaderView
{
    public class ColorHelper
    {
        public static readonly Color BlackText = Color.FromHex("#B0000000");

        public static readonly Color WhiteText = Color.FromHex("#FFFFFF");

        public static readonly Color MaterialNotificationColor = Color.FromHex("#323232");

        /// <summary>
        /// https://stackoverflow.com/questions/1855884/determine-font-color-based-on-background-color
        /// </summary>
        public static Color GetTextColorFromBackground(Color color)
        {
            // Counting the perceptive luminance - human eye favors green color...

            #if NET6_0_OR_GREATER
            double luminance = (0.299 * color.Red * 255 + 0.587 * color.Green * 255 + 0.114 * color.Blue * 255) / 255;
            #else
            double luminance = (0.299 * color.R * 255 + 0.587 * color.G * 255 + 0.114 * color.B * 255) / 255;
            #endif

            return luminance > 0.5 ? BlackText : WhiteText;
        }
    }
}
