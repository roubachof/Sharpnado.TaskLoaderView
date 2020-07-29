using System;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

// XF min version 3.6.0.220655
namespace Sharpnado.Presentation.Forms.CustomViews
{
    public partial class TaskLoaderView : ContentView
    {
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

        private void Initialize()
        {
            CreateFromTaskSource();
            SetBindings();

            OnTaskStartModeSet();
            OnTaskLoaderTypeSet();
            UpdateLoadingView();
            UpdateErrorView();
            UpdateEmptyView();
            UpdateNotStartedView();
            UpdateErrorNotificationView();
            UpdateEmptyStateImageSource();
            UpdateEmptyStateMessage();
            UpdateAccentColor();
            UpdateTextColor();
            UpdateFontFamily();
            UpdateRetryButtonText();
            UpdateNotificationBackgroundColor();
            UpdateNotificationTextColor();
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
            if (DefaultLoader == null)
            {
                return;
            }

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
            if (DefaultLoader == null)
            {
                return;
            }

            DefaultLoader.Color = AccentColor;
            ErrorViewButton.BackgroundColor = AccentColor;
            ErrorViewButton.TextColor = ColorHelper.GetTextColorFromBackground(AccentColor);
        }

        private void UpdateTextColor()
        {
            if (ErrorViewLabel == null)
            {
                return;
            }

            ErrorViewLabel.TextColor = TextColor;
            EmptyStateLabel.TextColor = TextColor;
        }

        private void UpdateFontFamily()
        {
            if (ErrorViewLabel == null)
            {
                return;
            }

            ErrorNotificationViewLabel.FontFamily = FontFamily;
            ErrorViewLabel.FontFamily = FontFamily;
            EmptyStateLabel.FontFamily = FontFamily;
        }

        private void UpdateRetryButtonText()
        {
            if (ErrorViewButton == null)
            {
                return;
            }

            ErrorViewButton.Text = RetryButtonText;
        }

        private void UpdateEmptyStateImageSource()
        {
            if (EmptyStateImage == null)
            {
                return;
            }

            EmptyStateImage.Source = EmptyStateImageSource;
        }

        private void UpdateEmptyStateMessage()
        {
            if (EmptyStateLabel == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(EmptyStateMessage))
            {
                return;
            }

            EmptyStateLabel.Text = EmptyStateMessage;
        }

        private void UpdateNotStartedView()
        {
            if (Container?.Children == null || NotStartedView == null)
            {
                return;
            }

            if (_notStartedView != null)
            {
                Container.Children.Remove(_notStartedView);
                _notStartedView.BindingContext = null;
            }

            _notStartedView = NotStartedView is DataTemplate dataTemplate
                ? (View)dataTemplate.CreateContent()
                : (View)NotStartedView;

            var bounds = AbsoluteLayout.GetLayoutBounds(_notStartedView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(_notStartedView, new Rectangle(1, 1, 1, 1));
                AbsoluteLayout.SetLayoutFlags(_notStartedView, AbsoluteLayoutFlags.All);
            }

            Container.Children.Insert((int)ViewIndex.NotStarted, _notStartedView);

            BindNotStartedView();
        }

        private void UpdateLoadingView()
        {
            if (Container?.Children == null || LoadingView == null)
            {
                return;
            }

            Container.Children.Remove(DefaultLoader);
            DefaultLoader.BindingContext = null;

            if (_loadingView != null)
            {
                Container.Children.Remove(_loadingView);
                _loadingView.BindingContext = null;
            }

            _loadingView = LoadingView is DataTemplate dataTemplate
                ? (View)dataTemplate.CreateContent()
                : (View)LoadingView;

            var bounds = AbsoluteLayout.GetLayoutBounds(_loadingView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(_loadingView, new Rectangle(1, 1, 1, 1));
                AbsoluteLayout.SetLayoutFlags(_loadingView, AbsoluteLayoutFlags.All);
            }

            Container.Children.Insert((int)ViewIndex.Loader, _loadingView);

            BindLoadingView();
        }

        private void UpdateEmptyView()
        {
            if (Container?.Children == null || EmptyView == null)
            {
                return;
            }

            Container.Children.Remove(DefaultEmptyStateView);
            DefaultEmptyStateView.BindingContext = null;

            if (_emptyView != null)
            {
                Container.Children.Remove(_emptyView);
                _emptyView.BindingContext = null;
            }

            _emptyView = EmptyView is DataTemplate dataTemplate
                ? (View)dataTemplate.CreateContent()
                : (View)EmptyView;

            var bounds = AbsoluteLayout.GetLayoutBounds(_emptyView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(_emptyView, new Rectangle(1, 1, 1, 1));
                AbsoluteLayout.SetLayoutFlags(_emptyView, AbsoluteLayoutFlags.All);
            }

            Container.Children.Insert((int)ViewIndex.Empty, _emptyView);

            BindEmptyView();
        }

        private void UpdateErrorView()
        {
            if (Container?.Children == null || ErrorView == null)
            {
                return;
            }

            Container.Children.Remove(DefaultErrorView);
            DefaultErrorView.BindingContext = null;

            if (_errorView != null)
            {
                Container.Children.Remove(_errorView);
                _errorView.BindingContext = null;
            }

            _errorView = ErrorView is DataTemplate dataTemplate
                ? (View)dataTemplate.CreateContent()
                : (View)ErrorView;

            var bounds = AbsoluteLayout.GetLayoutBounds(_errorView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(_errorView, new Rectangle(1, 1, 1, 1));
                AbsoluteLayout.SetLayoutFlags(_errorView, AbsoluteLayoutFlags.All);
            }

            Container.Children.Insert((int)ViewIndex.Error, _errorView);

            BindErrorView();
        }


        private void UpdateErrorNotificationView()
        {
            if (Container?.Children == null || ErrorNotificationView == null)
            {
                return;
            }

            Container.Children.Remove(DefaultErrorNotificationView);
            DefaultErrorNotificationView.BindingContext = null;

            if (_errorNotificationView != null)
            {
                Container.Children.Remove(_errorNotificationView);
                _errorNotificationView.BindingContext = null;
            }

            _errorNotificationView = ErrorNotificationView is DataTemplate dataTemplate
                ? (View)dataTemplate.CreateContent()
                : (View)ErrorNotificationView;

            var bounds = AbsoluteLayout.GetLayoutBounds(_errorNotificationView);
            if (bounds == Rectangle.Zero)
            {
                // Apply default bounds
                AbsoluteLayout.SetLayoutBounds(_errorNotificationView, new Rectangle(0, 1, 1, 50));
                AbsoluteLayout.SetLayoutFlags(
                    _errorNotificationView,
                    AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
            }

            Container.Children.Insert((int)ViewIndex.Notification, _errorNotificationView);

            BindErrorNotificationView();
        }

        private void UpdateNotificationBackgroundColor()
        {
            if (ErrorNotificationViewLabel == null)
            {
                return;
            }

            DefaultErrorNotificationView.BackgroundColor = NotificationBackgroundColor;
            ErrorNotificationViewLabel.TextColor = ColorHelper.GetTextColorFromBackground(NotificationTextColor);
        }

        private void UpdateNotificationTextColor()
        {
            if (ErrorNotificationViewLabel == null)
            {
                return;
            }

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

            BindNotStartedView();
            BindLoadingView();
            BindEmptyView();
            BindErrorView();
            BindErrorNotificationView();

            OnTaskLoaderTypeSet();

            OnTaskStartModeSet();
        }

        private void BindNotStartedView()
        {
            if (TaskLoaderNotifier == null)
            {
                return;
            }

            if (_notStartedView != null)
            {
                _notStartedView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.IsNotStarted), source: TaskLoaderNotifier));
            }
        }

        private void BindLoadingView()
        {
            if (TaskLoaderNotifier == null)
            {
                return;
            }

            if (_loadingView != null)
            {
                _loadingView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.ShowLoader), source: TaskLoaderNotifier));
            }
            else
            {
                SetDefaultLoadingViewBindings();
            }
        }

        private void BindEmptyView()
        {
            if (_emptyView != null)
            {
                _emptyView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.ShowEmptyState), source: TaskLoaderNotifier));
            }
            else
            {
                SetDefaultEmptyStateViewBindings();
            }
        }

        private void BindErrorView()
        {
            if (TaskLoaderNotifier == null)
            {
                return;
            }

            if (_errorView != null)
            {
                _errorView.SetBinding(
                    IsVisibleProperty,
                    new Binding(nameof(TaskLoaderNotifier.ShowError), source: TaskLoaderNotifier));
            }
            else
            {
                SetDefaultErrorViewBindings();
            }
        }

        private void BindErrorNotificationView()
        {
            if (_errorNotificationView != null)
            {
                _errorNotificationView.SetBinding(
                    IsVisibleProperty,
                    new Binding(
                        nameof(TaskLoaderNotifier.ShowErrorNotification),
                        source: TaskLoaderNotifier,
                        mode: BindingMode.TwoWay));
            }
            else
            {
                SetDefaultErrorNotificationViewBindings();
            }
        }

        private void SetDefaultLoadingViewBindings()
        {
            if (TaskLoaderNotifier == null)
            {
                return;
            }

            DefaultLoader?.SetBinding(
                ActivityIndicator.IsRunningProperty,
                new Binding(nameof(TaskLoaderNotifier.ShowLoader), source: TaskLoaderNotifier));
        }

        private void SetDefaultEmptyStateViewBindings()
        {
            if (TaskLoaderNotifier == null)
            {
                return;
            }

            if (DefaultEmptyStateView == null)
            {
                return;
            }

            DefaultEmptyStateView.SetBinding(
                Label.IsVisibleProperty,
                new Binding(nameof(TaskLoaderNotifier.ShowEmptyState), source: TaskLoaderNotifier));

            EmptyStateImage.IsVisible = EmptyStateImageSource != null;
            if (EmptyStateImage.IsVisible)
            {
                EmptyStateImage.Source = EmptyStateImageSource;
            }
        }

        private void SetDefaultErrorViewBindings()
        {
            if (TaskLoaderNotifier == null)
            {
                return;
            }

            if (DefaultErrorView == null)
            {
                return;
            }

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
            if (TaskLoaderNotifier == null)
            {
                return;
            }

            if (DefaultErrorNotificationView == null)
            {
                return;
            }

            DefaultErrorNotificationView.SetBinding(
                Frame.IsVisibleProperty,
                new Binding(nameof(TaskLoaderNotifier.ShowErrorNotification), source: TaskLoaderNotifier, mode: BindingMode.TwoWay));

            ErrorNotificationViewLabel.SetBinding(
                Label.TextProperty,
                new Binding(nameof(TaskLoaderNotifier.Error), source: TaskLoaderNotifier, converter: ErrorMessageConverter));
        }
    }
}