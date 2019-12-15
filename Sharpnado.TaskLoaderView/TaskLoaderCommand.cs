using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Sharpnado.Presentation.Forms;

namespace Sharpnado.TaskLoaderView
{
    public abstract class TaskLoaderCommandBase : ICommand
    {
        private readonly Func<object, bool> _canExecute;

        protected TaskLoaderCommandBase(Func<object, bool> canExecute = null)
        {
            _canExecute = _canExecute;
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
        private readonly Func<Task<T>> _taskSource;

        public TaskLoaderCommand(Func<Task<T>> taskSource, Func<object, bool> canExecute = null)
            : base(canExecute)
        {
            _taskSource = taskSource;
            Notifier = new TaskLoaderNotifier<T>();
        }

        public TaskLoaderNotifier<T> Notifier { get; }

        protected override bool IsExecuting => !Notifier.IsNotStarted && Notifier.IsNotCompleted;

        public override void Execute(object parameter)
        {
            Notifier.Load(_taskSource);
        }
    }
}
