using Vardirsoft.Shared.MVVM;

namespace Vardirsoft.XApp.Components
{
    public class DialogButton : ApplicationComponent
    {
        private readonly BaseDialogBox _parentBox;

        public DialogResult DialogResult { get; }

        public BaseCommand PressCommand { get; }

        public DialogButton(string id, BaseDialogBox box, DialogResult dialogResult) : base(id)
        {
            _parentBox = box;
            DialogResult = dialogResult;
            PressCommand = new RelayCommand(OnPress);
        }
        
        public DialogButton(BaseDialogBox box, DialogResult dialogResult) 
            : this(ToSafeId(dialogResult.ToString()), box, dialogResult)
        {
            
        }

        public void Press() => PressCommand.Execute(null);

        protected override ApplicationComponent CloneInternal(string id)
        {
            var clone = new DialogButton(_parentBox, DialogResult);
            
            return clone;
        }

        private void OnPress()
        {
            _parentBox.SelectedResult = DialogResult;
            _parentBox.ValidateAndAccept();
        }
    }
}