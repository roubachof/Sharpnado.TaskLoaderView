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

        protected TaskLoaderCommandBase(Func<object, bool> canExecute = null)
        {
            _canExecute = canExecute;

            if (_canExecute != null)
            {
                CanBeExecuted = CanExecute(null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler CanExecuteChanged;

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

        protected abstract bool IsExecuting { get; }

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
            if (!CanExecute(parameter))
            {
                return;
            }

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
            if (!CanExecute(parameter))
            {
                return;
            }

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
            if (!CanExecute(parameter))
            {
                return;
            }

            Notifier.Load(() => _taskSource((TParam)parameter));
        }
    }
}
