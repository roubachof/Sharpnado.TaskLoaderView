
using System.ComponentModel;
using System.Windows.Input;
using Sharpnado.Tasks;

namespace Sharpnado.TaskLoaderView
{
    public class NotStartedTaskLoaderNotifier : ITaskLoaderNotifier
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand ResetCommand => DoNothingCommand;

        public ICommand ReloadCommand
        {
            get => DoNothingCommand;
            set => throw new NotSupportedException();
        }

        public ICommand RefreshCommand
        {
            get => DoNothingCommand;
            set => throw new NotSupportedException();
        }

        public bool IsCompleted { get; }

        public bool IsNotStarted { get; } = true;

        public bool IsRunningOrSuccessfullyCompleted { get; }

        public bool IsNotCompleted { get; } = true;

        public bool IsSuccessfullyCompleted { get; }

        public bool IsCanceled { get; }

        public bool IsFaulted { get; }

        public Exception? Error { get; }

        public bool ShowLoader { get; }

        public bool ShowRefresher { get; }

        public bool ShowResult { get; }

        public bool ShowError { get; }

        public bool ShowEmptyState { get; }

        public bool ShowErrorNotification { get; }

        public ITaskMonitor CurrentLoadingTask { get; } = TaskMonitor.NotStartedTask;

        public bool DisableEmptyState { get; }

        public TimeSpan AutoResetDelay { get; } = TimeSpan.Zero;

        private static ICommand DoNothingCommand { get; } = new Command(DoNothing);

        public void Load(bool isRefreshing = false)
        {
        }

        public void Reset()
        {
        }

        public void OnTaskOverloaded()
        {
        }

        private static void DoNothing()
        {
        }
    }
}