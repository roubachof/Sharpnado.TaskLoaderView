using Xamarin.Forms;

#if NET6_0_OR_GREATER
using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;
#endif

[assembly: XmlnsDefinition("http://sharpnado.com", "Sharpnado.TaskLoaderView")]
[assembly: XmlnsPrefix("http://sharpnado.com", "sho")]