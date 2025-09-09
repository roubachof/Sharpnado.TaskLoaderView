using System.Globalization;
using Sample.Infrastructure;

namespace Sample.Converters
{
    public class StringToImageSourceResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ImageResourceExtension.GetImageSource((string) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
            throw new NotImplementedException();
        }
    }
}
