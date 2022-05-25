using System;
using System.Globalization;

using Xamarin.Forms;

namespace Sharpnado.TaskLoaderView
{
    public class DefaultErrorMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Exception exception)
            {
                return exception.Message;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
