using System;
using System.Threading.Tasks;
using Sharpnado.TaskLoaderView;
using Sharpnado.Tasks;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms
{
    public class TaskLoaderNotifier : TaskLoaderNotifierBase
    {
        private Func<Task> _loadingTaskSource;

        public TaskLoaderNotifier()
            : this(TimeSpan.Zero, null)
        {
        }

        public TaskLoaderNotifier(Func<Task> loadingTaskSource)
            : this(TimeSpan.Zero, loadingTaskSource)
        {
        }

        public TaskLoaderNotifier(TimeSpan autoResetDelay, Func<Task> loadingTaskSource = null)
            : base(autoResetDelay)
        {
            _loadingTaskSource = loadingTaskSource;

            CurrentLoadingTask = TaskMonitor.NotStartedTask;
            ReloadCommand = new Command(() => Load(_loadingTaskSource));
            RefreshCommand = new Command(() => Load(_loadingTaskSource, isRefreshing: true));
        }

        public override bool IsNotStarted => CurrentLoadingTask == TaskMonitor.NotStartedTask;

        /// <summary>
        /// Load a task previously set.
        /// </summary>
        public override void Load()
        {
            Load(_loadingTaskSource);
        }

        public void Load(Func<Task> loadingTaskSource, bool isRefreshing = false)
        {
            InternalLogger.Debug(Tag, () => $"Load( isRefreshing: {isRefreshing} )");

            lock (SyncRoot)
            {
                if (CurrentLoadingTask != TaskMonitor.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                {
                    // Log.Warn("A loading task is currently running: discarding this call");
                    return;
                }

                _loadingTaskSource = loadingTaskSource;

                CurrentLoadingTask = null;
                CurrentLoadingTask = new TaskMonitor.Builder(_loadingTaskSource)
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