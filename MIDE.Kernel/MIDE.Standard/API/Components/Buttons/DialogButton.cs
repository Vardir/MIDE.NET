using MIDE.API.Components;

namespace MIDE.API.Components
{
    public class DialogButton : Button
    {
        public DialogResult DialogResult { get; }

        public DialogButton(string id, DialogResult dialogResult) : base(id)
        {
            DialogResult = dialogResult;
        }
    }
}