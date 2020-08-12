using System;
using System.Collections;
using System.Threading.Tasks;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms
{
    public class TaskLoaderNotifier<TData> : TaskLoaderNotifierBase
    {
        private Func<Task<TData>> _loadingTaskSource;

        private TData _result;

        public TaskLoaderNotifier(bool disableEmptyState = false)
            : this(null, disableEmptyState, TimeSpan.Zero)
        {
        }

        public TaskLoaderNotifier(TimeSpan autoResetDelay)
            : this(null, false, autoResetDelay)
        {
        }

        public TaskLoaderNotifier(Func<Task> loadingTaskSource)
            : this((Func<Task<TData>>)loadingTaskSource, false, TimeSpan.Zero)
        {
        }

        public TaskLoaderNotifier(Func<Task<TData>> loadingTaskSource, bool disableEmptyState, TimeSpan autoResetDelay)
            : base(autoResetDelay, disableEmptyState)
        {
            CurrentLoadingTask = TaskMonitor<TData>.NotStartedTask;
            ReloadCommand = new Command(() => Load(_loadingTaskSource));
            RefreshCommand = new Command(() => Load(_loadingTaskSource, isRefreshing: true));

            _loadingTaskSource = loadingTaskSource;
        }

        public override bool IsNotStarted => CurrentLoadingTask == TaskMonitor<TData>.NotStartedTask;

        public TData Result
        {
            get => _result;
            set => SetAndRaise(ref _result, value);
        }

        /// <summary>
        /// Load a task previously set.
        /// </summary>
        public override void Load()
        {
            Load(_loadingTaskSource);
        }

        public void Load(Func<Task<TData>> loadingTaskSource, bool isRefreshing = false)
        {
            InternalLogger.Debug(Tag, () => $"Load( isRefreshing: {isRefreshing} )");
            lock (SyncRoot)
            {
                if (CurrentLoadingTask != TaskMonitor<TData>.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                {
                    // Log.Warn("A loading task is currently running: discarding this call");
                    return;
                }

                if (CurrentLoadingTask == TaskMonitor<TData>.NotStartedTask && loadingTaskSource == null)
                {
                    // Log.Warn("Refresh requested while not loaded yet, aborting...");
                    return;
                }

                _loadingTaskSource = loadingTaskSource;

                CurrentLoadingTask = null;
                CurrentLoadingTask = new TaskMonitor<TData>.Builder(_loadingTaskSource)
                    .WithName($"TaskLoaderNotifier<{nameof(TData)}>")
                    .WithWhenCompleted(OnTaskCompleted)
                    .WithWhenFaulted(faultedTask => OnTaskFaulted(faultedTask, isRefreshing))
                    .WithWhenSuccessfullyCompleted(
                        (completedTask, result) =>
                        {
                            Result = result;
                            OnTaskSuccessfullyCompleted(completedTask);
                        })
                    .Build();
            }

            Start(isRefreshing);
        }

        public override void Reset()
        {
            lock (SyncRoot)
            {
                if (!IsNotStarted)
                {
                    CurrentLoadingTask?.CancelCallbacks();
                }
            }

            CurrentLoadingTask = TaskMonitor<TData>.NotStartedTask;
            Result = default(TData);

            base.Reset();

            RaisePropertyChanged(nameof(Result));
        }

        protected override void Start(bool isRefreshing)
        {
            base.Start(isRefreshing);

            RaisePropertyChanged(nameof(Result));

            CurrentLoadingTask.Start();
        }

        protected override void OnTaskSuccessfullyCompleted(ITaskMonitor task)
        {
            InternalLogger.Debug(Tag, () => $"OnTaskSuccessfullyCompleted()");
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));

            if (!DisableEmptyState && (Result == null || (Result is ICollection collection && collection.Count == 0)))
            {
                InternalLogger.Debug(Tag, () => $"Showing empty state");
                ShowEmptyState = true;
                IsRunningOrSuccessfullyCompleted = false;
                return;
            }

            ShowResult = true;
        }
    }
}