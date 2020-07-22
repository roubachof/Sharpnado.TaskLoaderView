using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sharpnado.Presentation.Forms
{
    public abstract class TaskLoaderCommandBase : ICommand
    {
        private readonly Func<object, bool> _canExecute;

        protected TaskLoaderCommandBase(Func<object, bool> canExecute = null)
        {
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        protected abstract bool IsExecuting { get; }

        public abstract void Execute(object parameter);

        public bool CanExecute(object parameter)
        {
            return !IsExecuting && (_canExecute?.Invoke(null) ?? true);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class TaskLoaderCommand : TaskLoaderCommandBase
    {
        private readonly Func<Task> _taskSource;

        public TaskLoaderCommand(Func<Task> taskSource, Func<object, bool> canExecute = null)
            : base(canExecute)
        {
            _taskSource = taskSource;
            Notifier = new TaskLoaderNotifier();
        }

        public TaskLoaderNotifier Notifier { get; }

        protected override bool IsExecuting => !Notifier.IsNotStarted && Notifier.IsNotCompleted;

        public override void Execute(object parameter)
        {
            Notifier.Load(_taskSource);
        }
    }

    public class TaskLoaderCommand<T> : TaskLoaderCommandBase
    {
        private readonly Func<T, Task> _taskSource;

        public TaskLoaderCommand(Func<T, Task> taskSource, Func<object, bool> canExecute = null)
            : base(canExecute)
        {
            _taskSource = taskSource;
            Notifier = new TaskLoaderNotifier();
        }

        public TaskLoaderNotifier Notifier { get; }

        protected override bool IsExecuting => !Notifier.IsNotStarted && Notifier.IsNotCompleted;

        public override void Execute(object parameter)
        {
            Notifier.Load(() => _taskSource((T)parameter));
        }
    }

    public class TaskLoaderCommand<TParam, TTask> : TaskLoaderCommandBase
    {
        private readonly Func<TParam, Task<TTask>> _taskSource;

        public TaskLoaderCommand(Func<TParam, Task<TTask>> taskSource, Func<object, bool> canExecute = null)
            : base(canExecute)
        {
            _taskSource = taskSource;
            Notifier = new TaskLoaderNotifier<TTask>();
        }

        public TaskLoaderNotifier<TTask> Notifier { get; }

        protected override bool IsExecuting => !Notifier.IsNotStarted && Notifier.IsNotCompleted;

        public override void Execute(object parameter)
        {
            Notifier.Load(() => _taskSource((TParam)parameter));
        }
    }
}
