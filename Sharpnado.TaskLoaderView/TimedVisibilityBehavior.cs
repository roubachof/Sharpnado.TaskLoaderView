using System.Threading.Tasks;

using Xamarin.Forms;

namespace Sharpnado.TaskLoaderView
{
    public class TimedVisibilityBehavior : Behavior<View>
    {
        public static readonly BindableProperty VisibilityInMillisecondsProperty = BindableProperty.Create(
            nameof(VisibilityInMilliseconds),
            typeof(int),
            typeof(TimedVisibilityBehavior),
            5000);

        private bool _lastVisibility;

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

        private async void ViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var view = (View)sender;
            if (e.PropertyName != nameof(view.IsVisible))
            {
                return;
            }

            if (!_lastVisibility && view.IsVisible)
            {
                await Task.Delay(VisibilityInMilliseconds);
                view.IsVisible = false;
            }
            else
            {
                _lastVisibility = view.IsVisible;
            }
        }
    }
}