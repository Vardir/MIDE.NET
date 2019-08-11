using System;

namespace MIDE.Commands
{
    public class RelayParametrizedCommand : BaseCommand
    {
        private Action<object> action;
        private Func<object, bool> condition;

        public RelayParametrizedCommand(Action<object> action, Func<object, bool> condition)
        {
            if (action == null)
                throw new NullReferenceException(nameof(action));

            this.action = action;
            this.condition = condition;
        }

        public override bool CanExecute(object param) => condition?.Invoke(param) ?? true;

        public override void Execute(object param)
        {
            if (CanExecute(param))
                action(param);
        }
    }
}