namespace Sharpnado.TaskLoaderView;

public interface ITimeVisibilityText
{
    string Text { get; }
}

public class TimedVisibilityBehavior : Behavior<View>
{
    public static readonly BindableProperty VisibilityInMillisecondsProperty = BindableProperty.Create(
        nameof(VisibilityInMilliseconds),
        typeof(int),
        typeof(TimedVisibilityBehavior),
        5000);

    private const string Tag = "TimedVisibility";

    private bool _lastVisibility;

    private string _lastViewText = string.Empty;

    private CancellationTokenSource _cancellationTokenSource;

    public int VisibilityInMilliseconds
    {
        get => (int)GetValue(VisibilityInMillisecondsProperty);
        set => SetValue(VisibilityInMillisecondsProperty, value);
    }

    protected override void OnAttachedTo(View bindable)
    {
        _lastVisibility = bindable.IsVisible;
        bindable.PropertyChanged += ViewPropertyChanged;
    }

    private bool ViewTextHasChanged(string propertyName, ITimeVisibilityText viewWithText)
    {
        bool hasChanged = propertyName == nameof(ITimeVisibilityText.Text) && viewWithText.Text != _lastViewText;
        if (hasChanged)
        {
            _lastViewText = viewWithText.Text;
        }

        return hasChanged;
    }

    private async void ViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var view = (View)sender;
        if (e.PropertyName != nameof(view.IsVisible)
            && !(view is ITimeVisibilityText snackbar && ViewTextHasChanged(e.PropertyName, snackbar)))
        {
            return;
        }

        InternalLogger.Debug(Tag, () => $"Canceling wait");
        _cancellationTokenSource?.Cancel();
        if (!_lastVisibility && view.IsVisible)
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await Wait(VisibilityInMilliseconds, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException exception)
            {
                InternalLogger.Debug(Tag, () => $"Wait has been cancelled");
                return;
            }

            InternalLogger.Debug(Tag, () => $"setting view visibility to false");
            view.IsVisible = false;
        }
        else
        {
            _lastVisibility = view.IsVisible;
        }
    }

    private Task Wait(int milliseconds, CancellationToken token)
    {
        InternalLogger.Debug(Tag, () => $"Wait: {milliseconds}ms");
        return Task.Delay(milliseconds, token);
    }
}