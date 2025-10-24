using System.ComponentModel;

namespace Sharpnado.TaskLoaderView;

public class ControlTemplateLoadedEventArgs : EventArgs
{
    public ControlTemplateLoadedEventArgs(View? view)
    {
        View = view;
    }

    public View? View { get; }
}

public class TemplatedTaskLoader : ContentView
{
    private EventHandler<ControlTemplateLoadedEventArgs>? _pendingEventHandler;

    public static readonly BindableProperty TaskLoaderNotifierProperty = BindableProperty.Create(
        nameof(TaskLoaderNotifier),
        typeof(ITaskLoaderNotifier),
        typeof(TaskLoaderView),
        propertyChanged: TaskLoaderChanged);

    public static readonly BindableProperty ResultControlTemplateProperty = BindableProperty.Create(
        nameof(ResultControlTemplate),
        typeof(ControlTemplate),
        typeof(TemplatedTaskLoader));

    public static readonly BindableProperty LoadingControlTemplateProperty = BindableProperty.Create(
        nameof(LoadingControlTemplate),
        typeof(ControlTemplate),
        typeof(TemplatedTaskLoader));

    public static readonly BindableProperty ErrorControlTemplateProperty = BindableProperty.Create(
        nameof(ErrorControlTemplate),
        typeof(ControlTemplate),
        typeof(TemplatedTaskLoader));

    public static readonly BindableProperty EmptyControlTemplateProperty = BindableProperty.Create(
        nameof(EmptyControlTemplate),
        typeof(ControlTemplate),
        typeof(TemplatedTaskLoader));

    public event EventHandler<ControlTemplateLoadedEventArgs>? ResultControlTemplateLoaded;

    public event EventHandler<ControlTemplateLoadedEventArgs>? LoadingControlTemplateLoaded;

    public event EventHandler<ControlTemplateLoadedEventArgs>? ErrorControlTemplateLoaded;

    public event EventHandler<ControlTemplateLoadedEventArgs>? EmptyControlTemplateLoaded;

    public ITaskLoaderNotifier? TaskLoaderNotifier
    {
        get => (ITaskLoaderNotifier?)GetValue(TaskLoaderNotifierProperty);
        set => SetValue(TaskLoaderNotifierProperty, value);
    }

    public ControlTemplate ResultControlTemplate
    {
        get => (ControlTemplate)GetValue(ResultControlTemplateProperty);
        set => SetValue(ResultControlTemplateProperty, value);
    }

    public ControlTemplate? LoadingControlTemplate
    {
        get => (ControlTemplate?)GetValue(LoadingControlTemplateProperty);
        set => SetValue(LoadingControlTemplateProperty, value);
    }

    public ControlTemplate? ErrorControlTemplate
    {
        get => (ControlTemplate?)GetValue(ErrorControlTemplateProperty);
        set => SetValue(ErrorControlTemplateProperty, value);
    }

    public ControlTemplate? EmptyControlTemplate
    {
        get => (ControlTemplate?)GetValue(EmptyControlTemplateProperty);
        set => SetValue(EmptyControlTemplateProperty, value);
    }

    public new View GetTemplateChild(string name)
    {
        return (View)base.GetTemplateChild(name);
    }

    private static void TaskLoaderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var taskLoader = (TemplatedTaskLoader)bindable;
        taskLoader.SubscribeToNotifier(oldValue, newValue);
        taskLoader.Initialize();
    }

    private void SubscribeToNotifier(object oldValue, object newValue)
    {
        if (oldValue is ITaskLoaderNotifier oldNotifier)
        {
            oldNotifier.PropertyChanged -= NotifierPropertyChanged;
        }

        if (newValue is ITaskLoaderNotifier notifier)
        {
            notifier.PropertyChanged += NotifierPropertyChanged;
        }
    }

    private void Initialize()
    {
        if (TaskLoaderNotifier == null)
        {
            return;
        }

        if (TaskLoaderNotifier.ShowResult)
        {
            SetTemplateAndScheduleEvent(ResultControlTemplate, ResultControlTemplateLoaded);
            return;
        }

        if (TaskLoaderNotifier.ShowError)
        {
            SetTemplateAndScheduleEvent(ErrorControlTemplate ?? ResultControlTemplate, ErrorControlTemplateLoaded);
            return;
        }

        if (TaskLoaderNotifier.ShowEmptyState)
        {
            SetTemplateAndScheduleEvent(EmptyControlTemplate ?? ResultControlTemplate, EmptyControlTemplateLoaded);
            return;
        }

        if (TaskLoaderNotifier.ShowLoader)
        {
            SetTemplateAndScheduleEvent(LoadingControlTemplate ?? ResultControlTemplate, LoadingControlTemplateLoaded);
        }
    }

    private void NotifierPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ITaskLoaderNotifier.ShowResult) when TaskLoaderNotifier!.ShowResult:
                SetTemplateAndScheduleEvent(ResultControlTemplate, ResultControlTemplateLoaded);
                break;

            case nameof(ITaskLoaderNotifier.ShowError) when TaskLoaderNotifier!.ShowError:
                SetTemplateAndScheduleEvent(ErrorControlTemplate, ErrorControlTemplateLoaded);
                break;

            case nameof(ITaskLoaderNotifier.ShowLoader) when TaskLoaderNotifier!.ShowLoader:
                SetTemplateAndScheduleEvent(LoadingControlTemplate, LoadingControlTemplateLoaded);
                break;

            case nameof(ITaskLoaderNotifier.ShowEmptyState) when TaskLoaderNotifier!.ShowEmptyState:
                SetTemplateAndScheduleEvent(EmptyControlTemplate, EmptyControlTemplateLoaded);
                break;
        }
    }

    private void SetTemplateAndScheduleEvent(ControlTemplate? template, EventHandler<ControlTemplateLoadedEventArgs>? eventHandler)
    {
        _pendingEventHandler = eventHandler;
        ControlTemplate = template;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // TemplateRoot is set by MAUI after the template is applied
        // Use reflection to access the internal TemplateRoot property
        var templateRoot = GetTemplateRootViaReflection();
        _pendingEventHandler?.Invoke(this, new ControlTemplateLoadedEventArgs(templateRoot));
        _pendingEventHandler = null;
    }

    private View? GetTemplateRootViaReflection()
    {
        try
        {
            // Access the internal TemplateRoot property via reflection
            var templateRootProperty = typeof(TemplatedView).GetProperty(
                "Microsoft.Maui.Controls.IControlTemplated.TemplateRoot",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            return templateRootProperty?.GetValue(this) as View;
        }
        catch
        {
            // If reflection fails, return null
            return null;
        }
    }
}
