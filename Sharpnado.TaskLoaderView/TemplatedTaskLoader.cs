using System.ComponentModel;
using Xamarin.Forms;

namespace Sharpnado.TaskLoaderView
{
    public class TemplatedTaskLoader : ContentView
    {
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
                ControlTemplate = ResultControlTemplate;
                return;
            }

            if (TaskLoaderNotifier.ShowError)
            {
                ControlTemplate = ErrorControlTemplate ?? ResultControlTemplate;
                return;
            }

            if (TaskLoaderNotifier.ShowEmptyState)
            {
                ControlTemplate = EmptyControlTemplate ?? ResultControlTemplate;
                return;
            }

            if (TaskLoaderNotifier.ShowLoader)
            {
                ControlTemplate = LoadingControlTemplate ?? ResultControlTemplate;
            }
        }

        private void NotifierPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ITaskLoaderNotifier.ShowResult) when TaskLoaderNotifier!.ShowResult:
                    ControlTemplate = ResultControlTemplate;
                    break;

                case nameof(ITaskLoaderNotifier.ShowError) when TaskLoaderNotifier!.ShowError:
                    ControlTemplate = ErrorControlTemplate;
                    break;

                case nameof(ITaskLoaderNotifier.ShowLoader) when TaskLoaderNotifier!.ShowLoader:
                    ControlTemplate = LoadingControlTemplate;
                    break;

                case nameof(ITaskLoaderNotifier.ShowEmptyState) when TaskLoaderNotifier!.ShowEmptyState:
                    ControlTemplate = EmptyControlTemplate;
                    break;
            }
        }
    }
}
