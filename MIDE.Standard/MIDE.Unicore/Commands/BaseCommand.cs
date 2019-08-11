namespace MIDE.Commands
{
    public abstract class BaseCommand
    {
        public virtual bool CanExecute(object param) => true;

        public abstract void Execute(object param);
    }
}