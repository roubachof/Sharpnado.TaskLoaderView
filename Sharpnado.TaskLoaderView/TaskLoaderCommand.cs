using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sharpnado.Presentation.Forms
{
    public abstract class TaskLoaderCommandBase : INotifyPropertyChanged, ICommand
    {
        private readonly Func<object, bool> _canExecute;

        private bool _canBeExecuted;

        protected TaskLoaderCommandBase(ITaskLoaderNotifier notifier, Func<object, bool> canExecute = null)
        {
            Notifier = notifier;
            _canExecute = canExecute;

            if (_canExecute != null)
            {
                CanBeExecuted = CanExecute(null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler CanExecuteChanged;

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

        public bool IsExecuting => !Notifier.IsNotStarted && Notifier.IsNotCompleted;

        public abstract void Execute(object parameter);

        public bool CanExecute(object parameter)
        {
            return !IsExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public void RaiseCanExecuteChanged()
        {
            CanBeExecuted = CanExecute(null);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

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
        public TaskLoaderCommand(Func<Task> taskSource, TimeSpan autoResetDelay, Func<object, bool> canExecute = null)
            : base(new TaskLoaderNotifier(autoResetDelay), canExecute)
        {
            _taskSource = taskSource;
        }

        /// <summary>
        /// Create a TaskLoaderCommand with a TaskLoaderNotifier that will be started when the Execute method will
        /// be called.
        /// </summary>
        /// <param name="taskSource">The Task to be executed.</param>
        /// <param name="canExecute">The function determining if the command can be executed.</param>
        public TaskLoaderCommand(Func<Task> taskSource, Func<object, bool> canExecute = null)
            : this(taskSource, TimeSpan.Zero, canExecute)
        {
        }

        public new TaskLoaderNotifier Notifier => (TaskLoaderNotifier)base.Notifier;

        public override void Execute(object parameter)
        {
            if (IsExecuting)
            {
                return;
            }

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
        public TaskLoaderCommand(Func<T, Task> taskSource, TimeSpan autoResetDelay, Func<object, bool> canExecute = null)
            : base(new TaskLoaderNotifier(autoResetDelay), canExecute)
        {
            _taskSource = taskSource;
        }

        /// <summary>
        /// Create a TaskLoaderCommand with a TaskLoaderNotifier that will be started when the Execute method will
        /// be called.
        /// </summary>
        /// <param name="taskSource">The Task to be executed.</param>
        /// <param name="canExecute">The function determining if the command can be executed.</param>
        public TaskLoaderCommand(Func<T, Task> taskSource, Func<object, bool> canExecute = null)
            : this(taskSource, TimeSpan.Zero, canExecute)
        {
        }

        public new TaskLoaderNotifier Notifier => (TaskLoaderNotifier)base.Notifier;

        public override void Execute(object parameter)
        {
            if (IsExecuting)
            {
                return;
            }

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
        public TaskLoaderCommand(Func<TParam, Task<TTask>> taskSource, TimeSpan autoResetDelay, Func<object, bool> canExecute = null)
            : base(new TaskLoaderNotifier<TTask>(autoResetDelay), canExecute)
        {
            _taskSource = taskSource;
        }

        /// <summary>
        /// Create a TaskLoaderCommand with a TaskLoaderNotifier that will be started when the Execute method will
        /// be called.
        /// </summary>
        /// <param name="taskSource">The Task to be executed.</param>
        /// <param name="canExecute">The function determining if the command can be executed.</param>
        public TaskLoaderCommand(Func<TParam, Task<TTask>> taskSource, Func<object, bool> canExecute = null)
            : this(taskSource, TimeSpan.Zero, canExecute)
        {
        }

        public new TaskLoaderNotifier<TTask> Notifier => (TaskLoaderNotifier<TTask>)base.Notifier;

        public override void Execute(object parameter)
        {
            if (IsExecuting)
            {
                return;
            }

            Notifier.Load(() => _taskSource((TParam)parameter));
        }
    }
}
