using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sharpnado.Presentation.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Snackbar : Frame
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(Snackbar));

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            nameof(FontFamily),
            typeof(string),
            typeof(Snackbar));

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(Snackbar),
            defaultValue: Color.White);

        public static readonly BindableProperty TextSizeProperty = BindableProperty.Create(
            nameof(TextSize),
            typeof(double),
            typeof(Snackbar),
            defaultValue: 14d);

        public static readonly BindableProperty TextHorizontalOptionsProperty = BindableProperty.Create(
            nameof(TextHorizontalOptions),
            typeof(LayoutOptions),
            typeof(Snackbar),
            LayoutOptions.Center);

        public static readonly BindableProperty DisplayDurationMillisecondsProperty = BindableProperty.Create(
            nameof(DisplayDurationMilliseconds),
            typeof(int),
            typeof(Snackbar),
            defaultValue: 4000);

        public Snackbar()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public double TextSize
        {
            get => (double)GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        public LayoutOptions TextHorizontalOptions
        {
            get => (LayoutOptions)GetValue(TextHorizontalOptionsProperty);
            set => SetValue(TextHorizontalOptionsProperty, value);
        }

        public int DisplayDurationMilliseconds
        {
            get => (int)GetValue(DisplayDurationMillisecondsProperty);
            set => SetValue(DisplayDurationMillisecondsProperty, value);
        }
    }
}