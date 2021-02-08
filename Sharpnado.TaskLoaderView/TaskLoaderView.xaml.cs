using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// XF min version 3.6.0.220655
namespace Sharpnado.Presentation.Forms.CustomViews
{
    public enum TaskStartMode
    {
        Manual = 0,
        Auto,
    }

    public enum TaskLoaderType
    {
        Normal = 0,
        ResultAsLoadingView,
    }

    [ContentProperty("ResultView")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskLoaderView : ContentView
    {
        public static readonly BindableProperty TaskSourceProperty = BindableProperty.Create(
            nameof(TaskSource),
            typeof(Func<Task>),
            typeof(TaskLoaderView));

        public static readonly BindableProperty TaskLoaderNotifierProperty = BindableProperty.Create(
            nameof(TaskLoaderNotifier),
            typeof(ITaskLoaderNotifier),
            typeof(TaskLoaderView));

        public static readonly BindableProperty TaskStartModeProperty = BindableProperty.Create(
            nameof(TaskStartMode),
            typeof(TaskStartMode),
            typeof(TaskLoaderView));

        public static readonly BindableProperty TaskLoaderTypeProperty = BindableProperty.Create(
            nameof(TaskLoaderType),
            typeof(TaskLoaderType),
            typeof(TaskLoaderView));

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            nameof(FontFamily),
            typeof(string),
            typeof(TaskLoaderView),
            null,
            BindingMode.OneWay);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(TaskLoaderView),
            defaultValue: ColorHelper.BlackText);

        public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(TaskLoaderView),
            defaultValue: Color.Accent);

        public static readonly BindableProperty NotificationBackgroundColorProperty = BindableProperty.Create(
            nameof(NotificationBackgroundColor),
            typeof(Color),
            typeof(TaskLoaderView),
            defaultValue: ColorHelper.MaterialNotificationColor);

        public static readonly BindableProperty NotificationTextColorProperty = BindableProperty.Create(
            nameof(NotificationTextColor),
            typeof(Color),
            typeof(TaskLoaderView),
            defaultValue: ColorHelper.WhiteText);

        public static readonly BindableProperty RetryButtonTextProperty = BindableProperty.Create(
            nameof(RetryButtonText),
            typeof(string),
            typeof(TaskLoaderView));

        public static readonly BindableProperty EmptyStateImageSourceProperty = BindableProperty.Create(
            nameof(EmptyStateImageSource),
            typeof(ImageSource),
            typeof(TaskLoaderView));

        public static readonly BindableProperty EmptyStateMessageProperty = BindableProperty.Create(
            nameof(EmptyStateMessage),
            typeof(string),
            typeof(TaskLoaderView),
            defaultValue: DefaultEmptyStateMessage);

        public static readonly BindableProperty ErrorMessageConverterProperty = BindableProperty.Create(
            nameof(ErrorMessageConverter),
            typeof(IValueConverter),
            typeof(TaskLoaderView),
            defaultValue: new DefaultErrorMessageConverter());

        public static readonly BindableProperty ErrorImageConverterProperty = BindableProperty.Create(
            nameof(ErrorImageConverter),
            typeof(IValueConverter),
            typeof(TaskLoaderView));

        public static readonly BindableProperty ResultViewProperty = BindableProperty.Create(
            nameof(ResultView),
            typeof(object),
            typeof(TaskLoaderView),
            validateValue: ValidateCustomView);

        public static readonly BindableProperty LoadingViewProperty = BindableProperty.Create(
            nameof(LoadingView),
            typeof(object),
            typeof(TaskLoaderView),
            validateValue: ValidateCustomView);

        public static readonly BindableProperty ErrorViewProperty = BindableProperty.Create(
            nameof(ErrorView),
            typeof(object),
            typeof(TaskLoaderView),
            validateValue: ValidateCustomView);

        public static readonly BindableProperty EmptyViewProperty = BindableProperty.Create(
            nameof(EmptyView),
            typeof(object),
            typeof(TaskLoaderView),
            validateValue: ValidateCustomView);

        public static readonly BindableProperty NotStartedViewProperty = BindableProperty.Create(
            nameof(NotStartedView),
            typeof(object),
            typeof(TaskLoaderView),
            validateValue: ValidateCustomView);

        public static readonly BindableProperty ErrorNotificationViewProperty = BindableProperty.Create(
            nameof(ErrorNotificationView),
            typeof(object),
            typeof(TaskLoaderView));

        private const string DefaultEmptyStateMessage = @"No result yet ¯\_(ツ)_/¯";

        private View _resultView;
        private View _loadingView;
        private View _errorView;
        private View _emptyView;
        private View _notStartedView;
        private View _errorNotificationView;

        public TaskLoaderView()
        {
            InitializeComponent();

            Initialize();

            StartTaskCommand = new Command(
                () =>
                    {
                        if (TaskLoaderNotifier == null)
                        {
                            Trace.WriteLine(
                                "StartTaskCommand was called but the TaskSource AND the TaskLoaderNotifier property are null");
                            return;
                        }

                        TaskLoaderNotifier.Load();
                    });

            ResetCommand = new Command(
                () =>
                    {
                        foreach (var child in Container.Children)
                        {
                            child.IsVisible = _notStartedView == child;
                        }

                        TaskLoaderNotifier?.Reset();
                    });
        }

        private enum ViewIndex
        {
            Result = 0,

            Loader = 1,

            Error = 2,

            Empty = 3,

            NotStarted = 4,

            Notification = 5,
        }

        public ICommand StartTaskCommand { get; }

        public ICommand ResetCommand { get; }

        public Func<Task> TaskSource
        {
            get => (Func<Task>)GetValue(TaskSourceProperty);
            set => SetValue(TaskSourceProperty, value);
        }

        public ITaskLoaderNotifier TaskLoaderNotifier
        {
            get => (ITaskLoaderNotifier)GetValue(TaskLoaderNotifierProperty);
            set => SetValue(TaskLoaderNotifierProperty, value);
        }

        public TaskStartMode TaskStartMode
        {
            get => (TaskStartMode)GetValue(TaskStartModeProperty);
            set => SetValue(TaskStartModeProperty, value);
        }

        public TaskLoaderType TaskLoaderType
        {
            get => (TaskLoaderType)GetValue(TaskLoaderTypeProperty);
            set => SetValue(TaskLoaderTypeProperty, value);
        }

        public object ErrorNotificationView
        {
            get => GetValue(ErrorNotificationViewProperty);
            set => SetValue(ErrorNotificationViewProperty, value);
        }

        public object ResultView
        {
            get => GetValue(ResultViewProperty);
            set => SetValue(ResultViewProperty, value);
        }

        public object NotStartedView
        {
            get => GetValue(NotStartedViewProperty);
            set => SetValue(NotStartedViewProperty, value);
        }

        public object EmptyView
        {
            get => GetValue(EmptyViewProperty);
            set => SetValue(EmptyViewProperty, value);
        }

        public object ErrorView
        {
            get => GetValue(ErrorViewProperty);
            set => SetValue(ErrorViewProperty, value);
        }

        public object LoadingView
        {
            get => GetValue(LoadingViewProperty);
            set => SetValue(LoadingViewProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Color AccentColor
        {
            get => (Color)GetValue(AccentColorProperty);
            set => SetValue(AccentColorProperty, value);
        }

        public Color NotificationBackgroundColor
        {
            get => (Color)GetValue(NotificationBackgroundColorProperty);
            set => SetValue(NotificationBackgroundColorProperty, value);
        }

        public Color NotificationTextColor
        {
            get => (Color)GetValue(NotificationTextColorProperty);
            set => SetValue(NotificationTextColorProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public string RetryButtonText
        {
            get => (string)GetValue(RetryButtonTextProperty);
            set => SetValue(RetryButtonTextProperty, value);
        }

        public ImageSource EmptyStateImageSource
        {
            get => (ImageSource)GetValue(EmptyStateImageSourceProperty);
            set => SetValue(EmptyStateImageSourceProperty, value);
        }

        public string EmptyStateMessage
        {
            get => (string)GetValue(EmptyStateMessageProperty);
            set => SetValue(EmptyStateMessageProperty, value);
        }

        public IValueConverter ErrorImageConverter
        {
            get => (IValueConverter)GetValue(ErrorImageConverterProperty);
            set => SetValue(ErrorImageConverterProperty, value);
        }

        public IValueConverter ErrorMessageConverter
        {
            get => (IValueConverter)GetValue(ErrorMessageConverterProperty);
            set => SetValue(ErrorMessageConverterProperty, value);
        }

        private static bool ValidateCustomView(BindableObject bindable, object value)
        {
            if (!(value is DataTemplate) && !(value is View))
            {
                var stringBuilder = new StringBuilder(
                    "Since TaskLoaderView 2.1.0 all custom views could now be either a DataTemplate or a View.");
                stringBuilder.AppendLine();
                stringBuilder.Append("Thanks to that, you can now specify your custom views as resources in a ");
                stringBuilder.Append("ResourceDictionary and set them in your TaskLoaderView style.");
                throw new InvalidOperationException(stringBuilder.ToString());
            }

            return true;
        }
    }
}