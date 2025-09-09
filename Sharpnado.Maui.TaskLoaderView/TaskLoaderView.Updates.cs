using System.Runtime.CompilerServices;

using Microsoft.Maui.Layouts;

namespace Sharpnado.TaskLoaderView;

public partial class TaskLoaderView : ContentView
{
    private const string Tag = "LoaderView";

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

            case nameof(ResultView):
                UpdateResultView();
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
        UpdateResultView();
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
            BindResultView();
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

        DefaultErrorNotificationView.FontFamily = FontFamily;
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

    private void UpdateResultView()
    {
        if (TryCreateView(ResultView, ViewIndex.Result, ref _resultView))
        {
            BindResultView();
        }
    }

    private void UpdateNotStartedView()
    {
        if (TryCreateView(NotStartedView, ViewIndex.NotStarted, ref _notStartedView))
        {
            BindNotStartedView();
        }
    }

    private void UpdateLoadingView()
    {
        if (TryCreateView(LoadingView, ViewIndex.Loading, ref _loadingView))
        {
            Container.Children.Remove(DefaultLoader);
            DefaultLoader.BindingContext = null;

            BindLoadingView();
        }
    }

    private void UpdateEmptyView()
    {
        if (TryCreateView(EmptyView, ViewIndex.Empty, ref _emptyView))
        {
            Container.Children.Remove(DefaultEmptyStateView);
            DefaultEmptyStateView.BindingContext = null;

            BindEmptyView();
        }
    }

    private void UpdateErrorView()
    {
        if (TryCreateView(ErrorView, ViewIndex.Error, ref _errorView))
        {
            Container.Children.Remove(DefaultErrorView);
            DefaultErrorView.BindingContext = null;

            BindErrorView();
        }
    }

    private void UpdateErrorNotificationView()
    {
        if (TryCreateView(
                ErrorNotificationView,
                ViewIndex.ErrorNotification,
                ref _errorNotificationView,
                AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional,
                50))
        {
            Container.Children.Remove(DefaultErrorNotificationView);
            DefaultErrorNotificationView.BindingContext = null;

            BindErrorNotificationView();
        }
    }

    private bool TryCreateView(
        object source,
        ViewIndex index,
        ref View target,
        AbsoluteLayoutFlags flags = AbsoluteLayoutFlags.All,
        double height = 1)
    {
        if (Container?.Children == null || source == null)
        {
            return false;
        }

        var loggableTarget = target;
        InternalLogger.Debug(Tag, () => $"TryCreateView( source: {source}, index: {index}, target: {loggableTarget} )");

        if (target != null)
        {
            Container.Children.Remove(target);
            target.BindingContext = null;
        }

        target = source is DataTemplate dataTemplate
            ? (View)dataTemplate.CreateContent()
            : (View)source;
        target.IsVisible = false;

        var bounds = AbsoluteLayout.GetLayoutBounds(target);
        if (bounds.Width < 1 || bounds.Height < 1)
        {
            // Apply default bounds
            AbsoluteLayout.SetLayoutBounds(target, new Rect(1, 1, 1, height));
            AbsoluteLayout.SetLayoutFlags(target, flags);
        }

        InsertView(index, target);
        return true;
    }

    private void InsertView(ViewIndex viewIndex, View view)
    {
        if (Container?.Children == null)
        {
            return;
        }

        // Find insertion index based on ordered ViewIndex values
        int insertIndex = Container.Children.Count;
        for (int i = 0; i < Container.Children.Count; i++)
        {
            var existingIndex = GetViewIndexForChild((View)Container.Children[i]);
            if ((int)existingIndex > (int)viewIndex)
            {
                insertIndex = i;
                break;
            }
        }

        Container.Children.Insert(insertIndex, view);
    }

    private ViewIndex GetViewIndexForChild(View child)
    {
        if (child == _resultView) return ViewIndex.Result;
        if (child == _loadingView) return ViewIndex.Loading;
        if (child == _errorView) return ViewIndex.Error;
        if (child == _emptyView) return ViewIndex.Empty;
        if (child == _notStartedView) return ViewIndex.NotStarted;
        if (child == _errorNotificationView) return ViewIndex.ErrorNotification;
        return (ViewIndex)int.MaxValue;
    }


    private void UpdateNotificationBackgroundColor()
    {
        if (DefaultErrorNotificationView == null)
        {
            return;
        }

        DefaultErrorNotificationView.BackgroundColor = NotificationBackgroundColor;
    }

    private void UpdateNotificationTextColor()
    {
        if (DefaultErrorNotificationView == null)
        {
            return;
        }

        DefaultErrorNotificationView.TextColor = NotificationTextColor;
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

        InternalLogger.Debug(Tag, () => $"SetBindings() with TaskLoaderNotifier: {TaskLoaderNotifier}");

        BindResultView();
        BindNotStartedView();
        BindLoadingView();
        BindEmptyView();
        BindErrorView();
        BindErrorNotificationView();

        OnTaskLoaderTypeSet();

        OnTaskStartModeSet();
    }

    private void BindResultView()
    {
        if (TaskLoaderNotifier == null || _resultView == null)
        {
            return;
        }

        if (TaskLoaderType == TaskLoaderType.ResultAsLoadingView)
        {
            DefaultLoader.IsVisible = false;
            _resultView.SetBinding(
                ContentView.IsVisibleProperty,
                new Binding(nameof(TaskLoaderNotifier.IsRunningOrSuccessfullyCompleted), source: TaskLoaderNotifier));
        }
        else
        {
            _resultView.SetBinding(
                ContentView.IsVisibleProperty,
                new Binding(nameof(TaskLoaderNotifier.ShowResult), source: TaskLoaderNotifier));
        }
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
            IsVisibleProperty,
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
            IsVisibleProperty,
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
            IsVisibleProperty,
            new Binding(nameof(TaskLoaderNotifier.ShowErrorNotification), source: TaskLoaderNotifier, mode: BindingMode.TwoWay));

        DefaultErrorNotificationView.SetBinding(
            Snackbar.TextProperty,
            new Binding(nameof(TaskLoaderNotifier.Error), source: TaskLoaderNotifier, converter: ErrorMessageConverter));
    }
}