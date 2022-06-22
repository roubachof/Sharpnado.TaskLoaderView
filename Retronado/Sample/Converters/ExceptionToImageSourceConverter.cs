using System;
using System.Globalization;

using Sample.Infrastructure;
using Sample.Services;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sample.Converters
{
    public class ExceptionToImageSourceConverter : IValueConverter, IMarkupExtension
    {
        private static ImageResourceExtension imageResourceExtension;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (imageResourceExtension == null)
            {
                imageResourceExtension = new ImageResourceExtension();
            }

            string imageName;

            switch (value)
            {
                case ServerException serverException:
                    imageName = "Sample.Images.server.png";
                    break;
                case NetworkException networkException:
                    imageName = "Sample.Images.the_internet.png";
                    break;
                default:
                    imageName = "Sample.Images.richmond.png";
                    break;
            }

            imageResourceExtension.Source = imageName;
            return (ImageSource)imageResourceExtension.ProvideValue(null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
