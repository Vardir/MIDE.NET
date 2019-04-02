using System;
using MIDE.API.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    public abstract class BaseDialogWindow<T>
    {
        public DialogResult Results { get; }
        public DialogResult SelectedResult { get; private set; }
        public string Title { get; }
        public LayoutContainer Body { get; protected set; }
        public DialogButton[] DialogButtons { get; }
        public ObservableCollection<string> ValidationErrors { get; }

        public event Action ResultSelected;

        public BaseDialogWindow(DialogResult dialogResults, string title)
        {
            Title = title;
            Results = dialogResults;
            DialogButtons = GetButtons();
            Console.WriteLine("wnd");
            ValidationErrors = new ObservableCollection<string>();
        }

        public abstract T GetData();

        protected abstract void Validate();

        private DialogButton[] GetButtons()
        {
            List<DialogButton> buttons = new List<DialogButton>(2);
            if (Results.HasFlag(DialogResult.Yes))
                buttons.Add(GetButton("yes", DialogResult.Yes));
            if (Results.HasFlag(DialogResult.No))
                buttons.Add(GetButton("no", DialogResult.No));
            if (Results.HasFlag(DialogResult.Ok))
                buttons.Add(GetButton("ok", DialogResult.Ok));
            if (Results.HasFlag(DialogResult.Cancel))
                buttons.Add(GetButton("cancel", DialogResult.Cancel));
            if (Results.HasFlag(DialogResult.Accept))
                buttons.Add(GetButton("accept", DialogResult.Accept));
            if (Results.HasFlag(DialogResult.Abort))
                buttons.Add(GetButton("abort", DialogResult.Abort));
            if (Results.HasFlag(DialogResult.Skip))
                buttons.Add(GetButton("skip", DialogResult.Skip));
            if (Results.HasFlag(DialogResult.Retry))
                buttons.Add(GetButton("retry", DialogResult.Retry));
            return buttons.ToArray();
        }
        private DialogButton GetButton(string id, DialogResult result)
        {
            DialogButton button = new DialogButton(id, result);
            button.Padding = new Measurements.BoundingBox(40, 3);
            button.PressCommand = new RelayCommand(() =>
            {
                SelectedResult = result;
                Validate();
                if (ValidationErrors.Count == 0)
                {
                    ResultSelected?.Invoke();
                }
            });
            return button;
        }
    }

    [Flags]
    public enum DialogResult
    {
        None   = 0,
        Yes    = 1,  No     = 2,
        Ok     = 4,  Cancel = 8,
        Accept = 16, Abort  = 32,
        Skip   = 64, Retry  = 128
    }
}