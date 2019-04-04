using System.Collections.Generic;

namespace MIDE.API.Components
{
    /// <summary>
    /// Displays text information for user
    /// </summary>
    public class MessageDialogBox : BaseDialogBox<bool>
    {
        public MessageDialogBox(string title, string message) : base(title)
        {
            AddChild(new Label("message", message));

            SetDialogResults(DialogResult.Ok);
        }

        public override bool GetData() => true;

        protected override void Validate() { }
        protected override GridLayout GenerateGrid(string id, IEnumerable<DialogButton> buttons)
        {
            return GetGridButtonsCentered(id, buttons);
        }
    }
}