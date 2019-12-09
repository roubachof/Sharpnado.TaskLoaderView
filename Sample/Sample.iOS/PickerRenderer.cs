using Sample.iOS;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Picker), typeof(NoBorderPickerRenderer))]
namespace Sample.iOS
{
    public class NoBorderPickerRenderer : PickerRenderer
    {
        protected override UITextField CreateNativeControl()
        {
            var result = base.CreateNativeControl();
            result.Layer.BorderWidth = 0;
            result.BorderStyle = UITextBorderStyle.None;
            return result;
        }
    }
}