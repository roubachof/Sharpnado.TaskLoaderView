using System.Runtime.CompilerServices;

using Sharpnado.Presentation.Forms.ViewModels;
using Sharpnado.TaskLoaderView;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sharpnado.Presentation.Forms.CustomViews
{
    [ContentProperty("Child")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskLoaderView : ContentView
    {
        public static readonly BindableProperty ViewModelLoaderProperty = BindableProperty.Create(
            nameof(ViewModelLoader),
            typeof(IViewModelLoader),
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
            "No results yet.");

        public static readonly BindableProperty ErrorMessageConverterProperty = BindableProperty.Create(
            nameof(ErrorMessageConverter),
            typeof(IValueConverter),
            typeof(TaskLoaderView));

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

        public static readonly BindableProperty ErrorNotificationViewProperty = BindableProperty.Create(
            nameof(ErrorNotificationView),
            typeof(View),
            typeof(TaskLoaderView));

        public TaskLoaderView()
        {
            InitializeComponent();
        }

        private enum ViewIndex
        {
            Result = 0,

            Loader = 1,

            Error = 2,

            Empty = 3,

            Notification = 4,
        }

        public IViewModelLoader ViewModelLoader
        {
            get => (IViewModelLoader)GetValue(ViewModelLoaderProperty);
            set => SetValue(ViewModelLoaderProperty, value);
        }

        public View ErrorNotificationView
        {
            get => (View)GetValue(ErrorNotificationViewProperty);
            set => SetValue(ErrorNotificationViewProperty, value);
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
                case nameof(ViewModelLoader):
                    SetBindings();
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

        private void SetBindings()
        {
            if (ViewModelLoader == null)
            {
                return;
            }

            ResultView.SetBinding(
                ContentView.IsVisibleProperty,
                new Binding(nameof(ViewModelLoader.ShowResult), source: ViewModelLoader));

            if (LoadingView != null)
            {
                LoadingView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(ViewModelLoader.ShowLoader), source: ViewModelLoader));
            }
            else
            {
                SetDefaultLoadingViewBindings();
            }

            if (ErrorView != null)
            {
                ErrorView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(ViewModelLoader.ShowError), source: ViewModelLoader));
            }
            else
            {
                SetDefaultErrorViewBindings();
            }

            if (ErrorNotificationView != null)
            {
                ErrorNotificationView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(ViewModelLoader.ShowErrorNotification), source: ViewModelLoader, mode: BindingMode.TwoWay));
            }
            else
            {
                SetDefaultErrorNotificationViewBindings();
            }

            if (EmptyView != null)
            {
                EmptyView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(ViewModelLoader.ShowEmptyState), source: ViewModelLoader));
            }
            else
            {
                SetDefaultEmptyStateViewBindings();
            }
        }

        private void SetDefaultLoadingViewBindings()
        {
            DefaultLoader.SetBinding(
                ActivityIndicator.IsRunningProperty,
                new Binding(nameof(ViewModelLoader.ShowLoader), source: ViewModelLoader));
        }

        private void SetDefaultErrorViewBindings()
        {
            DefaultErrorView.SetBinding(
                StackLayout.IsVisibleProperty,
                new Binding(nameof(ViewModelLoader.ShowError), source: ViewModelLoader));

            ErrorViewImage.IsVisible = ErrorImageConverter != null;
            if (ErrorViewImage.IsVisible)
            {
                ErrorViewImage.SetBinding(
                    Image.SourceProperty,
                    new Binding(
                        nameof(ViewModelLoader.Error),
                        source: ViewModelLoader,
                        converter: ErrorImageConverter));
            }

            ErrorViewLabel.SetBinding(
                Label.TextProperty,
                new Binding(
                    nameof(ViewModelLoader.Error),
                    source: ViewModelLoader,
                    converter: ErrorMessageConverter));

            ErrorViewButton.SetBinding(
                Button.CommandProperty,
                new Binding(nameof(ViewModelLoader.ReloadCommand), source: ViewModelLoader));
        }

        private void SetDefaultErrorNotificationViewBindings()
        {
            DefaultErrorNotificationView.SetBinding(
                Frame.IsVisibleProperty,
                new Binding(nameof(ViewModelLoader.ShowErrorNotification), source: ViewModelLoader, mode: BindingMode.TwoWay));

            ErrorNotificationViewLabel.SetBinding(
                Label.TextProperty,
                new Binding(nameof(ViewModelLoader.Error), source: ViewModelLoader, converter: ErrorMessageConverter));
        }

        private void SetDefaultEmptyStateViewBindings()
        {
            DefaultEmptyStateView.SetBinding(
                Label.IsVisibleProperty,
                new Binding(nameof(ViewModelLoader.ShowEmptyState), source: ViewModelLoader));

            EmptyStateImage.IsVisible = EmptyStateImageSource != null;
            if (EmptyStateImage.IsVisible)
            {
                EmptyStateImage.Source = EmptyStateImageSource;
            }
        }
    }
}