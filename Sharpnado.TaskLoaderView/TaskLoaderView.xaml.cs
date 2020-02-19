using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// XF min version 3.4.0.33328
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

    [ContentProperty("Child")]
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

        public static readonly BindableProperty LoadingViewProperty = BindableProperty.Create(
            nameof(LoadingView),
            typeof(View),
            typeof(TaskLoaderView));

        public static readonly BindableProperty ErrorViewProperty = BindableProperty.Create(
            nameof(ErrorView),
            typeof(View),
            typeof(TaskLoaderView));

        public static readonly BindableProperty EmptyViewProperty = BindableProperty.Create(
            nameof(EmptyView),
            typeof(View),
            typeof(TaskLoaderView));

        public static readonly BindableProperty NotStartedViewProperty = BindableProperty.Create(
            nameof(NotStartedView),
            typeof(View),
            typeof(TaskLoaderView));

        public static readonly BindableProperty ErrorNotificationViewProperty = BindableProperty.Create(
            nameof(ErrorNotificationView),
            typeof(View),
            typeof(TaskLoaderView));

        private const string DefaultEmptyStateMessage = @"No result yet ¯\_(ツ)_/¯";

        public TaskLoaderView()
        {
            InitializeComponent();

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
                            child.IsVisible = NotStartedView == child;
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

        public View ErrorNotificationView
        {
            get => (View)GetValue(ErrorNotificationViewProperty);
            set => SetValue(ErrorNotificationViewProperty, value);
        }

        public View NotStartedView
        {
            get => (View)GetValue(NotStartedViewProperty);
            set => SetValue(NotStartedViewProperty, value);
        }

        public View EmptyView
        {
            get => (View)GetValue(EmptyViewProperty);
            set => SetValue(EmptyViewProperty, value);
        }

        public View ErrorView
        {
            get => (View)GetValue(ErrorViewProperty);
            set => SetValue(ErrorViewProperty, value);
        }

        public View LoadingView
        {
            get => (View)GetValue(LoadingViewProperty);
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

        public View Child
        {
            get => ResultView.Content;
            set
            {
                if (Container.Children.Contains(value))
                {
                    return;
                }

                ResultView.Content = value;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(TaskSource):
                    CreateFromTaskSource();
                    break;

                case nameof(TaskLoaderNotifier):
                    SetBindings();
                    break;

                case nameof(TaskStartMode):
                    OnTaskStartModeSet();
                    break;

                case nameof(TaskLoaderType):
                    OnTaskLoaderTypeSet();
                    break;

                case nameof(LoadingView):
                    UpdateLoadingView();
                    break;

                case nameof(ErrorView):
                    UpdateErrorView();
                    break;

                case nameof(EmptyView):
                    UpdateEmptyView();
                    break;

                case nameof(NotStartedView):
                    UpdateNotStartedView();
                    break;

                case nameof(ErrorNotificationView):
                    UpdateErrorNotificationView();
                    break;

                case nameof(EmptyStateImageSource):
                    UpdateEmptyStateImageSource();
                    break;

                case nameof(EmptyStateMessage):
                    UpdateEmptyStateMessage();
                    break;

                case nameof(AccentColor):
                    UpdateAccentColor();
                    break;

                case nameof(TextColor):
                    UpdateTextColor();
                    break;

                case nameof(FontFamily):
                    UpdateFontFamily();
                    break;

                case nameof(RetryButtonText):
                    UpdateRetryButtonText();
                    break;

                case nameof(NotificationBackgroundColor):
                    UpdateNotificationBackgroundColor();
                    break;

                case nameof(NotificationTextColor):
                    UpdateNotificationTextColor();
                    break;
            }
        }

        private void OnTaskStartModeSet()
        {
            if (TaskStartMode == TaskStartMode.Manual)
            {
                return;
            }

            TaskLoaderNotifier?.Load();
        }

        private void OnTaskLoaderTypeSet()
        {
            if (TaskLoaderType == TaskLoaderType.ResultAsLoadingView)
            {
                DefaultLoader.IsVisible = false;
                ResultView.SetBinding(
                    ContentView.IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.IsRunningOrSuccessfullyCompleted), source: TaskLoaderNotifier));
            }
        }

        private void UpdateAccentColor()
        {
            DefaultLoader.Color = AccentColor;
            ErrorViewButton.BackgroundColor = AccentColor;
            ErrorViewButton.TextColor = ColorHelper.GetTextColorFromBackground(AccentColor);
        }

        private void UpdateTextColor()
        {
            ErrorViewLabel.TextColor = TextColor;
            EmptyStateLabel.TextColor = TextColor;
        }

        private void UpdateFontFamily()
        {
            ErrorNotificationViewLabel.FontFamily = FontFamily;
            ErrorViewLabel.FontFamily = FontFamily;
            EmptyStateLabel.FontFamily = FontFamily;
        }

        private void UpdateRetryButtonText()
        {
            ErrorViewButton.Text = RetryButtonText;
        }

        private void UpdateEmptyStateImageSource()
        {
            EmptyStateImage.Source = EmptyStateImageSource;
        }

        private void UpdateEmptyStateMessage()
        {
            if (string.IsNullOrWhiteSpace(EmptyStateMessage))
            {
                return;
            }

            EmptyStateLabel.Text = EmptyStateMessage;
        }

        private void UpdateErrorNotificationView()
        {
            if (ErrorNotificationView == null)
            {
                return;
            }

            Container.Children.Remove(DefaultErrorNotificationView);

            var bounds = AbsoluteLayout.GetLayoutBounds(ErrorNotificationView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(ErrorNotificationView, new Rectangle(0, 1, 1, 50));
                AbsoluteLayout.SetLayoutFlags(
                    ErrorNotificationView,
                    AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
            }

            Container.Children.Insert((int)ViewIndex.Notification, ErrorNotificationView);
        }

        private void UpdateEmptyView()
        {
            if (EmptyView == null)
            {
                return;
            }

            Container.Children.Remove(DefaultEmptyStateView);

            var bounds = AbsoluteLayout.GetLayoutBounds(EmptyView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(EmptyView, new Rectangle(1, 1, 1, 1));
                AbsoluteLayout.SetLayoutFlags(EmptyView, AbsoluteLayoutFlags.All);
            }

            Container.Children.Insert((int)ViewIndex.Empty, EmptyView);
        }

        private void UpdateNotStartedView()
        {
            if (NotStartedView == null)
            {
                return;
            }

            var bounds = AbsoluteLayout.GetLayoutBounds(NotStartedView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(NotStartedView, new Rectangle(1, 1, 1, 1));
                AbsoluteLayout.SetLayoutFlags(NotStartedView, AbsoluteLayoutFlags.All);
            }

            Container.Children.Insert((int)ViewIndex.NotStarted, NotStartedView);
        }

        private void UpdateErrorView()
        {
            if (ErrorView == null)
            {
                return;
            }

            Container.Children.Remove(DefaultErrorView);

            var bounds = AbsoluteLayout.GetLayoutBounds(ErrorView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(ErrorView, new Rectangle(1, 1, 1, 1));
                AbsoluteLayout.SetLayoutFlags(ErrorView, AbsoluteLayoutFlags.All);
            }

            Container.Children.Insert((int)ViewIndex.Error, ErrorView);
        }

        private void UpdateLoadingView()
        {
            if (LoadingView == null)
            {
                return;
            }

            Container.Children.Remove(DefaultLoader);

            var bounds = AbsoluteLayout.GetLayoutBounds(LoadingView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(LoadingView, new Rectangle(1, 1, 1, 1));
                AbsoluteLayout.SetLayoutFlags(LoadingView, AbsoluteLayoutFlags.All);
            }

            Container.Children.Insert((int)ViewIndex.Loader, LoadingView);
        }

        private void UpdateNotificationBackgroundColor()
        {
            DefaultErrorNotificationView.BackgroundColor = NotificationBackgroundColor;
            ErrorNotificationViewLabel.TextColor = ColorHelper.GetTextColorFromBackground(NotificationTextColor);
        }

        private void UpdateNotificationTextColor()
        {
            ErrorNotificationViewLabel.TextColor = NotificationTextColor;
        }

        private void CreateFromTaskSource()
        {
            if (TaskSource == null)
            {
                return;
            }

            var taskSourceType = TaskSource.GetType();
            var taskType = taskSourceType.GenericTypeArguments[0];
            if (taskType.IsGenericType)
            {
                var taskResultType = taskType.GenericTypeArguments[0];
                var taskLoaderNotifierType = typeof(TaskLoaderNotifier<>).MakeGenericType(taskResultType);
                TaskLoaderNotifier = (ITaskLoaderNotifier)Activator.CreateInstance(taskLoaderNotifierType, TaskSource);
                return;
            }

            TaskLoaderNotifier = new TaskLoaderNotifier(TaskSource);
        }

        private void SetBindings()
        {
            if (TaskLoaderNotifier == null)
            {
                return;
            }

            ResultView.SetBinding(
                ContentView.IsVisibleProperty,
                new Binding(nameof(TaskLoaderNotifier.ShowResult), source: TaskLoaderNotifier));

            if (LoadingView != null)
            {
                LoadingView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.ShowLoader), source: TaskLoaderNotifier));
            }
            else
            {
                SetDefaultLoadingViewBindings();
            }

            if (NotStartedView != null)
            {
                NotStartedView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.IsNotStarted), source: TaskLoaderNotifier));
            }

            if (ErrorView != null)
            {
                ErrorView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.ShowError), source: TaskLoaderNotifier));
            }
            else
            {
                SetDefaultErrorViewBindings();
            }

            if (ErrorNotificationView != null)
            {
                ErrorNotificationView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.ShowErrorNotification), source: TaskLoaderNotifier, mode: BindingMode.TwoWay));
            }
            else
            {
                SetDefaultErrorNotificationViewBindings();
            }

            if (EmptyView != null)
            {
                EmptyView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.ShowEmptyState), source: TaskLoaderNotifier));
            }
            else
            {
                SetDefaultEmptyStateViewBindings();
            }

            OnTaskLoaderTypeSet();

            OnTaskStartModeSet();
        }

        private void SetDefaultLoadingViewBindings()
        {
            DefaultLoader.SetBinding(
                ActivityIndicator.IsRunningProperty,
                new Binding(nameof(TaskLoaderNotifier.ShowLoader), source: TaskLoaderNotifier));
        }

        private void SetDefaultErrorViewBindings()
        {
            DefaultErrorView.SetBinding(
                StackLayout.IsVisibleProperty,
                new Binding(nameof(TaskLoaderNotifier.ShowError), source: TaskLoaderNotifier));

            ErrorViewImage.IsVisible = ErrorImageConverter != null;
            if (ErrorViewImage.IsVisible)
            {
                ErrorViewImage.SetBinding(
                    Image.SourceProperty,
                    new Binding(
                        nameof(TaskLoaderNotifier.Error),
                        source: TaskLoaderNotifier,
                        converter: ErrorImageConverter));
            }

            ErrorViewLabel.SetBinding(
                Label.TextProperty,
                new Binding(
                    nameof(TaskLoaderNotifier.Error),
                    source: TaskLoaderNotifier,
                    converter: ErrorMessageConverter));

            ErrorViewButton.SetBinding(
                Button.CommandProperty,
                new Binding(nameof(TaskLoaderNotifier.ReloadCommand), source: TaskLoaderNotifier));
        }

        private void SetDefaultErrorNotificationViewBindings()
        {
            DefaultErrorNotificationView.SetBinding(
                Frame.IsVisibleProperty,
                new Binding(nameof(TaskLoaderNotifier.ShowErrorNotification), source: TaskLoaderNotifier, mode: BindingMode.TwoWay));

            ErrorNotificationViewLabel.SetBinding(
                Label.TextProperty,
                new Binding(nameof(TaskLoaderNotifier.Error), source: TaskLoaderNotifier, converter: ErrorMessageConverter));
        }

        private void SetDefaultEmptyStateViewBindings()
        {
            DefaultEmptyStateView.SetBinding(
                Label.IsVisibleProperty,
                new Binding(nameof(TaskLoaderNotifier.ShowEmptyState), source: TaskLoaderNotifier));

            EmptyStateImage.IsVisible = EmptyStateImageSource != null;
            if (EmptyStateImage.IsVisible)
            {
                EmptyStateImage.Source = EmptyStateImageSource;
            }
        }
    }
}