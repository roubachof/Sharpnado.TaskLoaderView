using System;
using System.Globalization;

using Sample.Services;

using Xamarin.Forms;

namespace Sample.Converters
{
    public class ExceptionToLottieConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string imageName;

            switch (value)
            {
                case ServerException serverException:
                    imageName = "server_error.json";
                    break;
                case NetworkException networkException:
                    imageName = "connection_grey.json";
                    break;
                default:
                    imageName = "sketch_grey.json";
                    break;
            }

            return imageName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
            throw new NotImplementedException();
        }
    }
}
