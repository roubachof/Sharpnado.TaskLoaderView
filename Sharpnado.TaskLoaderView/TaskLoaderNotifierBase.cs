using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms
{
    public abstract class TaskLoaderNotifierBase : ITaskLoaderNotifier
    {
        protected const string Tag = "Notifier";

        private bool _showLoader;
        private bool _showRefresher;
        private bool _showResult;
        private bool _showError;
        private bool _showEmptyState;
        private bool _showErrorNotification;

        private bool _isRunningOrSuccessfullyCompleted;

        private Exception _error;

        protected TaskLoaderNotifierBase(bool disableEmptyState = false)
            : this(TimeSpan.Zero, disableEmptyState)
        {
        }

        protected TaskLoaderNotifierBase(TimeSpan autoResetDelay, bool disableEmptyState = false)
        {
            AutoResetDelay = autoResetDelay;
            DisableEmptyState = disableEmptyState;

            ResetCommand = new Command(Reset);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ResetCommand { get; }

        public ICommand ReloadCommand { get; set; }

        public ICommand RefreshCommand { get; set; }

        public bool IsCompleted => CurrentLoadingTask.IsCompleted;

        public abstract bool IsNotStarted { get; }

        public bool IsNotCompleted => CurrentLoadingTask.IsNotCompleted;

        public bool IsSuccessfullyCompleted => CurrentLoadingTask.IsSuccessfullyCompleted;

        public bool IsCanceled => CurrentLoadingTask.IsCanceled;

        public bool IsFaulted => CurrentLoadingTask.IsFaulted;

        public bool IsRunningOrSuccessfullyCompleted
        {
            get => _isRunningOrSuccessfullyCompleted;
            set
            {
                if (SetAndRaise(ref _isRunningOrSuccessfullyCompleted, value))
                {
                    InternalLogger.Debug(Tag, () => $"IsRunningOrSuccessfullyCompleted: {_isRunningOrSuccessfullyCompleted}");
                }
            }
        }

        public bool ShowLoader
        {
            get => _showLoader;
            set
            {
                if (SetAndRaise(ref _showLoader, value))
                {
                    InternalLogger.Debug(Tag, () => $"ShowLoader: {_showLoader}");
                }
            }
        }

        public bool ShowRefresher
        {
            get => _showRefresher;
            set
            {
                if (SetAndRaise(ref _showRefresher, value))
                {
                    InternalLogger.Debug(Tag, () => $"ShowRefresher: {_showRefresher}");
                }
            }
        }

        public bool ShowResult
        {
            get => _showResult;
            set
            {
                if (SetAndRaise(ref _showResult, value))
                {
                    InternalLogger.Debug(Tag, () => $"ShowResult: {_showResult}");
                }
            }
        }

        public bool ShowError
        {
            get => _showError;
            set
            {
                if (SetAndRaise(ref _showError, value))
                {
                    InternalLogger.Debug(Tag, () => $"ShowError: {_showError}");
                }
            }
        }

        public bool ShowEmptyState
        {
            get => _showEmptyState;
            set
            {
                if (SetAndRaise(ref _showEmptyState, value))
                {
                    InternalLogger.Debug(Tag, () => $"ShowEmptyState: {_showEmptyState}");
                }
            }
        }

        public bool ShowErrorNotification
        {
            get => _showErrorNotification;
            set
            {
                if (SetAndRaise(ref _showErrorNotification, value))
                {
                    InternalLogger.Debug(Tag, () => $"ShowErrorNotification: {_showErrorNotification}");
                }
            }
        }

        public Exception Error
        {
            get => _error;
            set => SetAndRaise(ref _error, value);
        }

        public ITaskMonitor CurrentLoadingTask { get; protected set; }

        public bool DisableEmptyState { get; }

        public TimeSpan AutoResetDelay { get; }

        protected object SyncRoot { get; } = new object();

        public abstract void Load();

        public virtual void Reset()
        {
            InternalLogger.Debug(Tag, () => $"Reset()");
            IsRunningOrSuccessfullyCompleted = ShowError = ShowResult = ShowEmptyState = ShowLoader = ShowRefresher = false;
            Error = null;

            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsNotCompleted));
            RaisePropertyChanged(nameof(IsNotStarted));
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            RaisePropertyChanged(nameof(IsFaulted));
        }

        protected void OnTaskCompleted(ITaskMonitor task)
        {
            InternalLogger.Debug(Tag, () => $"OnTaskCompleted()");
            ShowRefresher = ShowLoader = false;

            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsNotCompleted));
            RaisePropertyChanged(nameof(IsNotStarted));

            if (AutoResetDelay > TimeSpan.Zero)
            {
                TaskMonitor.Create(
                    async () =>
                    {
                        await Task.Delay(AutoResetDelay);
                        Reset();
                    });
            }
        }

        protected void OnTaskFaulted(ITaskMonitor faultedTask, bool isRefreshing)
        {
            InternalLogger.Debug(Tag, () => $"OnTaskFaulted()");
            RaisePropertyChanged(nameof(IsFaulted));

            IsRunningOrSuccessfullyCompleted = false;
            ShowError = !isRefreshing;
            ShowErrorNotification = isRefreshing;
            Error = faultedTask.InnerException;
        }

        protected virtual void OnTaskSuccessfullyCompleted(ITaskMonitor task)
        {
            InternalLogger.Debug(Tag, () => $"OnTaskSuccessfullyCompleted()");
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));

            ShowResult = true;
        }

        protected virtual void Start(bool isRefreshing)
        {
            InternalLogger.Debug(Tag, () => $"Start( isRefreshing: {isRefreshing} )");

            IsRunningOrSuccessfullyCompleted = ShowLoader = !isRefreshing;
            ShowRefresher = isRefreshing;

            if (!isRefreshing)
            {
                Error = null;
                ShowError = ShowResult = ShowEmptyState = false;
            }

            RaisePropertyChanged(nameof(IsNotStarted));
            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsNotCompleted));
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            RaisePropertyChanged(nameof(IsFaulted));
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetAndRaise<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(property, value))
            {
                return false;
            }

            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        private static string DefaultErrorHandler(Exception exception)
        {
            return "An unknown error occured";
        }
    }
}
