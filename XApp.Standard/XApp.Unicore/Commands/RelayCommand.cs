using System;

namespace XApp.Commands
{
    public class RelayCommand : BaseCommand
    {
        private Action action;
        private Func<bool> condition;
        
        public RelayCommand(Action action, Func<bool> condition)
        {
            if (action == null)
                throw new NullReferenceException(nameof(action));

            this.action = action;
            this.condition = condition;
        }

        public override bool CanExecute(object _) => condition?.Invoke() ?? true;

        public override void Execute(object _)
        {
            if (CanExecute(_))
            {    
                action();
            }
        }
    }

    public class RelayCommand<T> : BaseCommand
    {
        private Action<T> action;
        private Func<T, bool> condition;

        public RelayCommand(Action<T> action, Func<T, bool> condition)
        {
            if (action == null)
                throw new NullReferenceException(nameof(action));

            this.action = action;
            this.condition = condition;
        }

        public override bool CanExecute(object param) => condition?.Invoke((T)param) ?? true;

        public override void Execute(object param)
        {
            if (CanExecute(param))
            {    
                action((T)param);
            }
        }
    }
}