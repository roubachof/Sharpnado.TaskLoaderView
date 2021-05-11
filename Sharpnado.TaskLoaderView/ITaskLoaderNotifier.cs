using System;
using System.ComponentModel;
using System.Windows.Input;
using Sharpnado.Tasks;

namespace Sharpnado.Presentation.Forms
{
    public interface ITaskLoaderNotifier : INotifyPropertyChanged
    {
        ICommand ResetCommand { get; }

        ICommand ReloadCommand { get; set; }

        ICommand RefreshCommand { get; set; }

        bool IsCompleted { get; }

        bool IsNotStarted { get; }

        bool IsRunningOrSuccessfullyCompleted { get; }

        bool IsNotCompleted { get; }

        bool IsSuccessfullyCompleted { get; }

        bool IsCanceled { get; }

        bool IsFaulted { get; }

        Exception Error { get; }

        bool ShowLoader { get; }

        bool ShowRefresher { get; }

        bool ShowResult { get; }

        bool ShowError { get; }

        bool ShowEmptyState { get; }

        bool ShowErrorNotification { get; }

        ITaskMonitor CurrentLoadingTask { get; }

        bool DisableEmptyState { get; }

        TimeSpan AutoResetDelay { get; }

        void Load(bool isRefreshing = false);

        void Reset();

        void OnTaskOverloaded();
    }
}