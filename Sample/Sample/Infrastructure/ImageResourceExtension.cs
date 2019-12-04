using System;
using System.Collections.Generic;
using System.Reflection;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sample.Infrastructure
{
    [ContentProperty (nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        private static readonly Dictionary<string, ImageSource> Cache = new Dictionary<string, ImageSource>();

        public string Source { get; set; }

        public object ProvideValue (IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            if (Cache.TryGetValue(Source, out var imageSource))
            {
                return imageSource;
            }

            // Do your translation lookup here, using whatever method you require
            var newImageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtension).GetTypeInfo().Assembly);
            Cache.Add(Source, newImageSource);
            return newImageSource;
        }
    }
}
