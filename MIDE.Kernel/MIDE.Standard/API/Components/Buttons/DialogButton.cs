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

        public static string ToId(DialogResult result) => result switch
        {
            DialogResult.Ok => "ok",
            DialogResult.Yes => "yes",
            DialogResult.No => "no",
            DialogResult.Accept => "accept",
            DialogResult.Cancel => "cancel",
            DialogResult.Skip => "skip",
            DialogResult.Retry => "retry",
            DialogResult.Abort => "abort",
            _ => throw new ArgumentException("Not supported dialog result")
        };
    }
}