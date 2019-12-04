using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultLayoutPage : ContentPage, IBindablePage
    {
        public DefaultLayoutPage()
        {
            InitializeComponent();

            ResourcesHelper.SetTosCellMode();
        }
    }
}