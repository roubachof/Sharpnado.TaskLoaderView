using System.Globalization;

namespace Sample.Converters;

public class CyclicLoadingLottieConverter : IValueConverter
{
    private int _counter = -1;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || (value is bool showLoader && !showLoader))
        {
            return null;
        }

        _counter = ++_counter > 2 ? 0 : _counter;
        switch (_counter)
        {
            case 0:
                return "delorean_grey.json";
            case 1:
                return "joystick_grey.json";
            case 2:
                return "cassette_grey.json";
        }

        return "delorean_grey.json";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // One-Way converter only
        throw new NotImplementedException();
    }
}