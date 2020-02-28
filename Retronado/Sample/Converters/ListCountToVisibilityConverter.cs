using System;
using System.Collections;
using System.Globalization;
using Sample.Infrastructure;
using Sample.Services;

using Xamarin.Forms;

namespace Sample.Converters
{
    public class ListCountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection collection)
            {
                return collection.Count > 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
            throw new NotImplementedException();
        }
    }
}
