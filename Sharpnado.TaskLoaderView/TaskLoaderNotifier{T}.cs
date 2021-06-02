using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;

using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms
{
    public class TaskLoaderNotifier<TData> : TaskLoaderNotifierBase
    {
        private Func<bool, Task<TData>> _loadingTaskSource;

        private TData _result;

        public TaskLoaderNotifier(bool disableEmptyState = false, string tag = null)
            : this(null, disableEmptyState, TimeSpan.Zero, tag ?? $"TaskLoaderNotifier<{typeof(TData).GetTypeInfo().Name}>")
        {
        }

        public TaskLoaderNotifier(TimeSpan autoResetDelay, string tag = null)
            : this(null, false, autoResetDelay, tag ?? $"TaskLoaderNotifier<{typeof(TData).GetTypeInfo().Name}>")
        {
        }

        public TaskLoaderNotifier(Func<bool, Task<TData>> loadingTaskSource, string tag = null)
            : this(loadingTaskSource, false, TimeSpan.Zero, tag ?? $"TaskLoaderNotifier<{typeof(TData).GetTypeInfo().Name}>")
        {
        }

        public TaskLoaderNotifier(
            Func<bool, Task<TData>> loadingTaskSource,
            bool disableEmptyState,
            TimeSpan autoResetDelay,
            string tag = nameof(TaskLoaderNotifier<TData>))
            : base(autoResetDelay, disableEmptyState, tag)
        {
            CurrentLoadingTask = TaskMonitor<TData>.NotStartedTask;
            ReloadCommand = new Command(() => Load(_loadingTaskSource));
            RefreshCommand = new Command(() => Load(_loadingTaskSource, true));

            _loadingTaskSource = loadingTaskSource;
        }

        public override bool IsNotStarted => CurrentLoadingTask == TaskMonitor<TData>.NotStartedTask;

        public TData Result
        {
            get => _result;
            set => SetAndRaise(ref _result, value);
        }

        /// <summary>
        ///     Load a task previously set.
        /// </summary>
        public override void Load(bool isRefreshing = false)
        {
            Load(_loadingTaskSource, isRefreshing);
        }

        public void UpdateLoadingTaskSource(Func<bool, Task<TData>> loadingTaskSource)
        {
            InternalLogger.Debug(Tag, () => $"UpdateLoadingTaskSource()");
            _loadingTaskSource = loadingTaskSource;
        }

        public void Load(Func<bool, Task<TData>> loadingTaskSource, bool isRefreshing = false)
        {
            InternalLogger.Debug(Tag, () => $"Load( isRefreshing: {isRefreshing} )");
            lock (SyncRoot)
            {
                if (CurrentLoadingTask != TaskMonitor<TData>.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                {
                    InternalLogger.Warn("A loading task is currently running: discarding previous call");
                    OnTaskOverloaded();
                }

                if (CurrentLoadingTask == TaskMonitor<TData>.NotStartedTask && loadingTaskSource == null)
                {
                    // Log.Warn("Refresh requested while not loaded yet, aborting...");
                    return;
                }

                _loadingTaskSource = loadingTaskSource;

                CurrentLoadingTask = null;
                CurrentLoadingTask = new TaskMonitor<TData>.Builder(() => _loadingTaskSource(isRefreshing))
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
            Result = default;

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
            InternalLogger.Debug(Tag, () => "OnTaskSuccessfullyCompleted()");
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));

            if (!DisableEmptyState && (Result == null || (Result is ICollection collection && collection.Count == 0)))
            {
                InternalLogger.Debug(Tag, () => "Showing empty state");
                ShowEmptyState = true;
                IsRunningOrSuccessfullyCompleted = false;
                return;
            }

            ShowResult = true;
        }
    }
}