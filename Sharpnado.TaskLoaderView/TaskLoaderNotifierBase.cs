using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms
{
    public abstract class TaskLoaderNotifierBase : ITaskLoaderNotifier
    {
        private Exception _error;

        private bool _isRunningOrSuccessfullyCompleted;

        private bool _showEmptyState;

        private bool _showError;

        private bool _showErrorNotification;

        private bool _showLoader;

        private bool _showRefresher;

        private bool _showResult;

        protected TaskLoaderNotifierBase(bool disableEmptyState = false, string tag = "TaskLoaderNotifier")
            : this(TimeSpan.Zero, disableEmptyState, tag)
        {
        }

        protected TaskLoaderNotifierBase(
            TimeSpan autoResetDelay,
            bool disableEmptyState = false,
            string tag = "TaskLoaderNotifier")
        {
            AutoResetDelay = autoResetDelay;
            DisableEmptyState = disableEmptyState;
            Tag = tag;

            ResetCommand = new Command(Reset);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Tag { get; }

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
                    InternalLogger.Debug(
                        Tag,
                        () => $"IsRunningOrSuccessfullyCompleted: {_isRunningOrSuccessfullyCompleted}");
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

        protected object SyncRoot { get; } = new();

        public abstract void Load(bool isRefreshing = false);

        public virtual void Reset()
        {
            InternalLogger.Debug(Tag, () => "Reset()");

            Error = null;
            IsRunningOrSuccessfullyCompleted =
                ShowError = ShowResult = ShowEmptyState = ShowLoader = ShowRefresher = ShowErrorNotification = false;

            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsNotCompleted));
            RaisePropertyChanged(nameof(IsNotStarted));
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            RaisePropertyChanged(nameof(IsFaulted));
        }

        /// <summary>
        /// We'll need to just cancel the current task and keep all the states cause a new task will be loaded 
        /// </summary>
        public void OnTaskOverloaded()
        {
            lock (SyncRoot)
            {
                if (!IsNotStarted)
                {
                    CurrentLoadingTask?.CancelCallbacks();
                }
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder($"{Tag} => ");
            if (ShowLoader)
            {
                builder.Append(nameof(ShowLoader));
                builder.Append(" / ");
            }

            if (ShowResult)
            {
                builder.Append(nameof(ShowResult));
                builder.Append(" / ");
            }

            if (ShowEmptyState)
            {
                builder.Append(nameof(ShowEmptyState));
                builder.Append(" / ");
            }

            if (ShowError)
            {
                builder.Append(nameof(ShowError));
                builder.Append(" / ");
            }

            if (ShowRefresher)
            {
                builder.Append(nameof(ShowRefresher));
                builder.Append(" / ");
            }

            if (ShowErrorNotification)
            {
                builder.Append(nameof(ShowErrorNotification));
                builder.Append(" / ");
            }

            return builder.ToString();
        }

        protected void OnTaskCompleted(ITaskMonitor task)
        {
            InternalLogger.Debug(Tag, () => "OnTaskCompleted()");
            if (CurrentLoadingTask != task && task.IsCanceled)
            {
                InternalLogger.Info("A previous task has been canceled: discarding the updates");
                return;
            }

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
            InternalLogger.Debug(Tag, () => "OnTaskFaulted()");
            RaisePropertyChanged(nameof(IsFaulted));

            var exception = faultedTask.InnerException;
            if (exception == null && faultedTask.IsCanceled)
            {
                exception = new TaskCanceledException("This task has been canceled.");
            }

            exception ??= new UnknownException("An unknown error has occurred");

            Error = exception;

            IsRunningOrSuccessfullyCompleted = false;
            ShowError = !isRefreshing;
            ShowErrorNotification = isRefreshing;

            RaisePropertyChanged(nameof(ShowErrorNotification));
        }

        protected virtual void OnTaskSuccessfullyCompleted(ITaskMonitor task)
        {
            InternalLogger.Debug(Tag, () => "OnTaskSuccessfullyCompleted()");
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));

            ShowResult = true;
        }

        protected virtual void Start(bool isRefreshing)
        {
            InternalLogger.Debug(Tag, () => $"Start( isRefreshing: {isRefreshing} )");

            if (!isRefreshing)
            {
                Error = null;
                ShowError = ShowResult = ShowEmptyState = false;
            }

            IsRunningOrSuccessfullyCompleted = ShowLoader = !isRefreshing;
            ShowRefresher = isRefreshing;

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

        private class UnknownException : Exception
        {
            public UnknownException(string message)
                : base(message)
            {
            }
        }
    }
}