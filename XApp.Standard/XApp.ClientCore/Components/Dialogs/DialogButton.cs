using Vardirsoft.Shared.MVVM;

namespace Vardirsoft.XApp.Components
{
    public class DialogButton : LayoutComponent
    {
        private BaseDialogBox parentBox;

        public DialogResult DialogResult { get; }

        public BaseCommand PressCommand { get; }

        public DialogButton(string id, BaseDialogBox box, DialogResult dialogResult) : base(id)
        {
            PressCommand = new RelayCommand(OnPress, null);            
        }
        public DialogButton(BaseDialogBox box, DialogResult dialogResult) 
            : this(ToSafeId(dialogResult.ToString()), box, dialogResult)
        {
            
        }

        public void Press() => PressCommand.Execute(null);

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = new DialogButton(parentBox, DialogResult);
            
            return clone;
        }

        private void OnPress()
        {
            parentBox.SelectedResult = DialogResult;
            parentBox.ValidateAndAccept();
        }
    }
}