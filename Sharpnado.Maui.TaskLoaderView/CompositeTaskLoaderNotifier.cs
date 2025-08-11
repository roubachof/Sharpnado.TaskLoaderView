using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;
using Xamarin.Forms;

namespace Sharpnado.TaskLoaderView
{
    public enum ShowRefresherFromChildrenStrategy
    {
        Never = 0,
        WhenAll,
        WhenAny,
    }

    public partial class CompositeTaskLoaderNotifier : ITaskLoaderNotifier
    {
        protected const string Tag = "CompositeNotifier";

        private readonly ShowRefresherFromChildrenStrategy _showRefresherFromChildrenStrategy;

        private readonly ITaskLoaderNotifier[] _loaders;

        private readonly ITaskLoaderNotifier[] _mainLoaders = Array.Empty<ITaskLoaderNotifier>();

        private bool _showLoader;
        private bool _showRefresher;
        private bool _showResult;
        private bool _showError;
        private bool _showEmptyState;
        private bool _showErrorNotification;
        private bool _showLastError;

        private bool _isRunningOrSuccessfullyCompleted;

        private Exception _error;

        private Exception _lastError;

        public CompositeTaskLoaderNotifier(params ITaskLoaderNotifier[] taskLoaderNotifiers)
            : this(ShowRefresherFromChildrenStrategy.Never, taskLoaderNotifiers)
        {
        }

        public CompositeTaskLoaderNotifier(
            ShowRefresherFromChildrenStrategy showRefresherFromChildrenStrategy,
            params ITaskLoaderNotifier[] taskLoaderNotifiers)
        {
            _showRefresherFromChildrenStrategy = showRefresherFromChildrenStrategy;
            _loaders = taskLoaderNotifiers;

            Subscribe();

            ResetCommand = new Command(Reset);
            ReloadCommand = new Command(() => Load(isRefreshing: false));
            RefreshCommand = new Command(() => Load(isRefreshing: true));
        }

        private CompositeTaskLoaderNotifier(
            ITaskLoaderNotifier[] mainLoaders,
            ITaskLoaderNotifier[] taskLoaderNotifiers)
            : this(mainLoaders.Concat(taskLoaderNotifiers).ToArray())
        {
            _mainLoaders = mainLoaders;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ResetCommand { get; }

        public ICommand ReloadCommand { get; set; }

        public ICommand RefreshCommand { get; set; }

        public bool IsCompleted => _loaders.All(l => l.IsCompleted);

        public bool IsNotStarted => _loaders.All(l => l.IsNotStarted);

        public bool IsNotCompleted => _loaders.Any(l => l.IsNotCompleted);

        public bool IsSuccessfullyCompleted => _loaders.All(l => l.IsSuccessfullyCompleted);

        public bool IsCanceled => _loaders.Any(l => l.IsCanceled);

        public bool IsFaulted => _loaders.Any(l => l.IsFaulted);

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

        public bool ShowError
        {
            get => _showError;
            set
            {
                if (SetAndRaise(ref _showError, value))
                {
                    InternalLogger.Debug(Tag, () => $"ShowError: {_showError}");
                }

                if (value)
                {
                    ShowLastError = ShowError;
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

                    if (value)
                    {
                        ShowLastError = ShowErrorNotification;
                    }
                }
            }
        }

        public bool ShowLastError
        {
            get => _showLastError;
            set
            {
                if (SetAndRaise(ref _showLastError, value))
                {
                    InternalLogger.Debug(Tag, () => $"ShowAnyError: {_showLastError} (notif: {_showErrorNotification}, error: {_showError})");
                }
            }
        }

        public Exception Error
        {
            get => _error;
            set => SetAndRaise(ref _error, value);
        }

        public Exception LastError
        {
            get => _lastError;
            set
            {
                if (SetAndRaise(ref _lastError, value))
                {
                    InternalLogger.Debug(Tag, () => $"LastError: {_lastError}");
                }
            }

        }

        public ITaskMonitor CurrentLoadingTask { get; } = null;

        public bool DisableEmptyState { get; } = false;

        public TimeSpan AutoResetDelay { get; } = TimeSpan.Zero;

        public static Builder ForCommands()
        {
            return new Builder();
        }

        public void OnTaskOverloaded()
        {
            InternalLogger.Debug(Tag, () => $"OnTaskOverloaded() : let the child tasks handle their states");
        }

        public void Load(bool isRefreshing = false)
        {
            InternalLogger.Debug(Tag, () => $"Load()");

            foreach (var loader in _loaders)
            {
                loader.Load(isRefreshing);
            }

            if (!isRefreshing)
            {
                Error = null;
                ShowErrorNotification = ShowError = ShowResult = ShowEmptyState = false;
            }

            IsRunningOrSuccessfullyCompleted = true;
            ShowLoader = !isRefreshing;
            ShowRefresher = isRefreshing;

            TaskMonitor.Create(
                async () =>
                {
                    await Task.WhenAll(_loaders.Select(loader => loader.CurrentLoadingTask.TaskCompleted));
                    ShowRefresher = false;
                });

            RaisePropertyChanged(nameof(IsNotStarted));
            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsNotCompleted));
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            RaisePropertyChanged(nameof(IsFaulted));
        }

        public void Reset()
        {
            InternalLogger.Debug(Tag, () => $"Reset()");
            foreach (var loader in _loaders)
            {
                loader.Reset();
            }

            IsRunningOrSuccessfullyCompleted = ShowError =
                ShowErrorNotification = ShowResult = ShowEmptyState = ShowLoader = ShowRefresher = false;
            Error = null;
            LastError = null;

            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsNotCompleted));
            RaisePropertyChanged(nameof(IsNotStarted));
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            RaisePropertyChanged(nameof(IsFaulted));

            ResetCommand.Execute(null);
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

        private void Subscribe()
        {
            foreach (var loader in _loaders)
            {
                loader.PropertyChanged += LoaderOnPropertyChanged;
            }
        }

        private void LoaderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var taskNotifier = (ITaskLoaderNotifier)sender;

            switch (e.PropertyName)
            {
                case nameof(IsRunningOrSuccessfullyCompleted):
                    IsRunningOrSuccessfullyCompleted = _loaders.All(l => l.IsRunningOrSuccessfullyCompleted);
                    break;

                case nameof(ShowLoader):
                    ShowLoader = _loaders.Any(l => l.ShowLoader && !_mainLoaders.Contains(l));
                    break;

                case nameof(ShowRefresher):
                    switch (_showRefresherFromChildrenStrategy)
                    {
                        case ShowRefresherFromChildrenStrategy.WhenAny:
                            ShowLoader = _loaders.Any(l => l.ShowLoader);
                            break;
                        case ShowRefresherFromChildrenStrategy.WhenAll:
                            ShowLoader = _loaders.All(l => l.ShowLoader);
                            break;
                    }

                    break;

                case nameof(ShowResult):
                    ShowResult = _loaders.All(l => l.ShowResult);
                    break;

                case nameof(ShowEmptyState):
                    ShowEmptyState = _loaders.All(l => l.ShowEmptyState);
                    break;

                case nameof(ShowError):
                    if (_mainLoaders.Contains(taskNotifier))
                    {
                        return;
                    }

                    InternalLogger.Debug(Tag, () =>
                    {
                        // bool any = _loaders.Any(l => l.ShowError && !_mainLoaders.Contains(l));
                        return $"LoaderOnPropertyChanged() => setting ShowError, taskNotifier.ShowError = {taskNotifier.ShowError}";
                    });

                    ShowError = taskNotifier.ShowError;

                    LastError = taskNotifier.Error ?? LastError;
                    Error = IsFaulted
                        ? new AggregateException(
                            _loaders
                                .Where(l => l.Error != null && !_mainLoaders.Contains(l))
                                .Select(l => l.Error))
                        : null;
                    break;

                case nameof(ShowErrorNotification):

                    InternalLogger.Debug(Tag, () =>
                    {
                        // bool any = _loaders.Any(l => l.ShowErrorNotification);
                        return $"LoaderOnPropertyChanged() => setting ShowErrorNotification, taskNotifier.ShowErrorNotification = {taskNotifier.ShowErrorNotification}";
                    });

                    ShowErrorNotification = taskNotifier.ShowErrorNotification;

                    LastError = taskNotifier.Error ?? LastError;
                    Error = IsFaulted
                        ? new AggregateException(
                            _loaders
                                .Where(l => l.Error != null)
                                .Select(l => l.Error))
                        : null;
                    break;
            }
        }
    }
}
