using System;
using MIDE.API.Commands;

namespace MIDE.API.Components
{
    /// <summary>
    /// Provides basic interaction logic for dialog boxes
    /// </summary>
    public class DialogButton : Button
    {
        public DialogResult DialogResult { get; }
        public BaseDialogBox ParentBox { get; }

        public DialogButton(BaseDialogBox dialogBox, DialogResult dialogResult) 
            : base(ToId(dialogResult))
        {
            ParentBox = dialogBox;
            DialogResult = dialogResult;
            PressCommand = new RelayCommand(Press);
        }

        public void Press()
        {
            ParentBox.SelectedResult = DialogResult;
            ParentBox.ValidateAndAccept();
        }

        public static string ToId(DialogResult result)
        {
            switch (result)
            {
                case DialogResult.Ok: return "ok";
                case DialogResult.Yes: return "yes";
                case DialogResult.No: return "no";
                case DialogResult.Accept: return "accept";
                case DialogResult.Cancel: return "cancel";
                case DialogResult.Skip: return "skip";
                case DialogResult.Retry: return "retry";
                case DialogResult.Abort: return "abort";
                default: throw new ArgumentException("Not supported dialog result");
            }
        }
    }
}