using System;
using System.Threading.Tasks;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;
using Xamarin.Forms;

namespace Sharpnado.TaskLoaderView
{
    public class TaskLoaderNotifier : TaskLoaderNotifierBase
    {
        private Func<bool, Task> _loadingTaskSource;

        public TaskLoaderNotifier()
            : this(TimeSpan.Zero, null)
        {
        }

        public TaskLoaderNotifier(Func<bool, Task> loadingTaskSource, string tag = "TaskLoaderNotifier")
            : this(TimeSpan.Zero, loadingTaskSource, tag)
        {
        }

        public TaskLoaderNotifier(TimeSpan autoResetDelay, Func<bool, Task> loadingTaskSource = null, string tag = "TaskLoaderNotifier")
            : base(autoResetDelay, false, tag)
        {
            _loadingTaskSource = loadingTaskSource;

            CurrentLoadingTask = TaskMonitor.NotStartedTask;
            ReloadCommand = new Command(() => Load(_loadingTaskSource));
            RefreshCommand = new Command(() => Load(_loadingTaskSource, isRefreshing: true));
        }

        public override bool IsNotStarted => CurrentLoadingTask == TaskMonitor.NotStartedTask;

        public void UpdateLoadingTaskSource(Func<bool, Task> loadingTaskSource)
        {
            InternalLogger.Debug(Tag, () => $"UpdateLoadingTaskSource()");
            _loadingTaskSource = loadingTaskSource;
        }

        /// <summary>
        /// Load a task previously set.
        /// </summary>
        public override void Load(bool isRefreshing = false)
        {
            Load(_loadingTaskSource, isRefreshing);
        }

        public void Load(Func<bool, Task> loadingTaskSource, bool isRefreshing = false)
        {
            InternalLogger.Debug(Tag, () => $"Load( isRefreshing: {isRefreshing} )");

            lock (SyncRoot)
            {
                if (CurrentLoadingTask != TaskMonitor.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                {
                    InternalLogger.Warn("A loading task is currently running: discarding previous call");
                    OnTaskOverloaded();
                }

                _loadingTaskSource = loadingTaskSource;

                CurrentLoadingTask = null;
                CurrentLoadingTask = new TaskMonitor.Builder(() => _loadingTaskSource(isRefreshing))
                    .WithName("TaskLoaderNotifier")
                    .WithWhenCompleted(OnTaskCompleted)
                    .WithWhenFaulted(faultedTask => OnTaskFaulted(faultedTask, isRefreshing))
                    .WithWhenSuccessfullyCompleted(OnTaskSuccessfullyCompleted)
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

            CurrentLoadingTask = TaskMonitor.NotStartedTask;

            base.Reset();
        }

        protected override void Start(bool isRefreshing)
        {
            base.Start(isRefreshing);

            CurrentLoadingTask.Start();
        }
    }
}
