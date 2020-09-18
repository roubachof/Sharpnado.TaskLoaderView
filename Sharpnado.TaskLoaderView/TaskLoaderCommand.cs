using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms
{
    public abstract class TaskLoaderCommandBase : INotifyPropertyChanged, ICommand
    {
        private readonly Func<object, bool> _canExecute;

        private readonly WeakEventManager _weakEventManager = new WeakEventManager();

        private bool _canBeExecuted;

        protected TaskLoaderCommandBase(ITaskLoaderNotifier notifier, Func<object, bool> canExecute = null, bool autoRaiseCanExecuteChange = false)
        {
            Notifier = notifier;
            _canExecute = canExecute;

            AutoRaiseCanExecuteChange = autoRaiseCanExecuteChange;

            if (_canExecute != null)
            {
                CanBeExecuted = CanExecute(null);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => _weakEventManager.AddEventHandler(value);
            remove => _weakEventManager.RemoveEventHandler(value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ITaskLoaderNotifier Notifier { get; }

        public bool CanBeExecuted
        {
            get => _canBeExecuted;
            set
            {
                if (_canBeExecuted != value)
                {
                    _canBeExecuted = value;
                    OnPropertyChanged(nameof(CanBeExecuted));
                }
            }
        }

        public bool IsExecuting { get; private set; }

        protected bool AutoRaiseCanExecuteChange { get; }

        public async void Execute(object parameter)
        {
            if (IsExecuting)
            {
                return;
            }

            try
            {
                IsExecuting = true;

                if (AutoRaiseCanExecuteChange)
                {
                    RaiseCanExecuteChanged();
                }

                ExecuteInternal(parameter);

                // TaskCompleted special task never raise an exception, so we are safe with async void ;)
                await Notifier.CurrentLoadingTask.TaskCompleted;
            }
            finally
            {
                IsExecuting = false;
                if (AutoRaiseCanExecuteChange)
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? !IsExecuting;
        }

        public void RaiseCanExecuteChanged()
        {
            CanBeExecuted = CanExecute(null);
            _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
        }

        protected abstract void ExecuteInternal(object parameter);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TaskLoaderCommand : TaskLoaderCommandBase
    {
        private readonly Func<Task> _taskSource;

        /// <summary>
        /// Create a TaskLoaderCommand with a TaskLoaderNotifier that will be started when the Execute method will
        /// be called.
        /// </summary>
        /// <param name="taskSource">The Task to be executed.</param>
        /// <param name="autoResetDelay">If > TimeSpan.Zero, upon completion and after expiration
        /// of the autoResetDelay the TaskLoaderNotifier will be Reset.</param>
        /// <param name="canExecute">The function determining if the command can be executed.</param>
        /// <param name="autoRaiseCanExecuteChange">If true, will raise automatically can execute before the task is running and after it is completed.</param>
        public TaskLoaderCommand(Func<Task> taskSource, TimeSpan autoResetDelay, Func<object, bool> canExecute = null, bool autoRaiseCanExecuteChange = false)
            : base(new TaskLoaderNotifier(autoResetDelay), canExecute, autoRaiseCanExecuteChange)
        {
            _taskSource = taskSource;
        }

        /// <summary>
        /// Create a TaskLoaderCommand with a TaskLoaderNotifier that will be started when the Execute method will
        /// be called.
        /// </summary>
        /// <param name="taskSource">The Task to be executed.</param>
        /// <param name="canExecute">The function determining if the command can be executed.</param>
        /// <param name="autoRaiseCanExecuteChange">If true, will raise automatically can execute before the task is running and after it is completed.</param>
        public TaskLoaderCommand(Func<Task> taskSource, Func<object, bool> canExecute = null, bool autoRaiseCanExecuteChange = false)
            : this(taskSource, TimeSpan.Zero, canExecute, autoRaiseCanExecuteChange)
        {
        }

        public new TaskLoaderNotifier Notifier => (TaskLoaderNotifier)base.Notifier;

        protected override void ExecuteInternal(object parameter)
        {
            Notifier.Load(_taskSource);
        }
    }

    public class TaskLoaderCommand<T> : TaskLoaderCommandBase
    {
        private readonly Func<T, Task> _taskSource;

        /// <summary>
        /// Create a TaskLoaderCommand with a TaskLoaderNotifier that will be started when the Execute method will
        /// be called.
        /// </summary>
        /// <param name="taskSource">The Task to be executed.</param>
        /// <param name="autoResetDelay">If > TimeSpan.Zero, upon completion and after expiration
        /// of the autoResetDelay the TaskLoaderNotifier will be Reset.</param>
        /// <param name="canExecute">The function determining if the command can be executed.</param>
        /// <param name="autoRaiseCanExecuteChange">If true, will raise automatically can execute before the task is running and after it is completed.</param>
        public TaskLoaderCommand(Func<T, Task> taskSource, TimeSpan autoResetDelay, Func<object, bool> canExecute = null, bool autoRaiseCanExecuteChange = false)
            : base(new TaskLoaderNotifier(autoResetDelay), canExecute, autoRaiseCanExecuteChange)
        {
            _taskSource = taskSource;
        }

        /// <summary>
        /// Create a TaskLoaderCommand with a TaskLoaderNotifier that will be started when the Execute method will
        /// be called.
        /// </summary>
        /// <param name="taskSource">The Task to be executed.</param>
        /// <param name="canExecute">The function determining if the command can be executed.</param>
        /// <param name="autoRaiseCanExecuteChange">If true, will raise automatically can execute before the task is running and after it is completed.</param>
        public TaskLoaderCommand(Func<T, Task> taskSource, Func<object, bool> canExecute = null, bool autoRaiseCanExecuteChange = true)
            : this(taskSource, TimeSpan.Zero, canExecute, autoRaiseCanExecuteChange)
        {
        }

        public new TaskLoaderNotifier Notifier => (TaskLoaderNotifier)base.Notifier;

        protected override void ExecuteInternal(object parameter)
        {
            Notifier.Load(() => _taskSource((T)parameter));
        }
    }

    public class TaskLoaderCommand<TParam, TTask> : TaskLoaderCommandBase
    {
        private readonly Func<TParam, Task<TTask>> _taskSource;

        /// <summary>
        /// Create a TaskLoaderCommand with a TaskLoaderNotifier that will be started when the Execute method will
        /// be called.
        /// </summary>
        /// <param name="taskSource">The Task to be executed.</param>
        /// <param name="autoResetDelay">If > TimeSpan.Zero, upon completion and after expiration
        /// of the autoResetDelay the TaskLoaderNotifier will be Reset.</param>
        /// <param name="canExecute">The function determining if the command can be executed.</param>
        /// <param name="autoRaiseCanExecuteChange">If true, will raise automatically can execute before the task is running and after it is completed.</param>
        public TaskLoaderCommand(Func<TParam, Task<TTask>> taskSource, TimeSpan autoResetDelay, Func<object, bool> canExecute = null, bool autoRaiseCanExecuteChange = true)
            : base(new TaskLoaderNotifier<TTask>(autoResetDelay), canExecute, autoRaiseCanExecuteChange)
        {
            _taskSource = taskSource;
        }

        /// <summary>
        /// Create a TaskLoaderCommand with a TaskLoaderNotifier that will be started when the Execute method will
        /// be called.
        /// </summary>
        /// <param name="taskSource">The Task to be executed.</param>
        /// <param name="canExecute">The function determining if the command can be executed.</param>
        /// <param name="autoRaiseCanExecuteChange">If true, will raise automatically can execute before the task is running and after it is completed.</param>
        public TaskLoaderCommand(Func<TParam, Task<TTask>> taskSource, Func<object, bool> canExecute = null, bool autoRaiseCanExecuteChange = true)
            : this(taskSource, TimeSpan.Zero, canExecute, autoRaiseCanExecuteChange)
        {
        }

        public new TaskLoaderNotifier<TTask> Notifier => (TaskLoaderNotifier<TTask>)base.Notifier;

        protected override void ExecuteInternal(object parameter)
        {
            Notifier.Load(() => _taskSource((TParam)parameter));
        }
    }
}
