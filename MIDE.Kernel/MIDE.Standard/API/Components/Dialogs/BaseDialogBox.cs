using System;
using System.Linq;
using MIDE.API.Measurements;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    /// <summary>
    /// Top base class for dialog boxes
    /// </summary>
    public abstract class BaseDialogBox : GridLayout
    {
        public DialogResult SelectedResult { get; set; }
        public string Title { get; }
        public DialogResult[] Results { get; private set; }
        public LayoutContainer DialogButtons { get; private set; }
        public ObservableCollection<string> ValidationErrors { get; }

        public event Action ResultSelected;

        public BaseDialogBox(string title) : base("dialog-box")
        {
            MinWidth = 300;
            MinHeight = 200;
            MaxWidth = 1024;
            MaxHeight = 1024;

            Title = title;
            ValidationErrors = new ObservableCollection<string>();
        }

        /// <summary>
        /// Validates data model of the dialog box and closes it if there are no errors occurred
        /// </summary>
        public void ValidateAndAccept()
        {
            Validate();
            if (ValidationErrors.Count == 0)
            {
                ResultSelected?.Invoke();
            }
        }

        protected abstract void Validate();
        protected abstract GridLayout GenerateGrid(string id, IEnumerable<DialogButton> buttons);

        protected void SetDialogResults(params DialogResult[] dialogResults)
        {
            List<DialogResult> results = new List<DialogResult>(dialogResults.Length);
            List<DialogButton> buttons = new List<DialogButton>(dialogResults.Length);
            for (int i = 0; i < dialogResults.Length; i++)
            {
                DialogResult result = dialogResults[i];
                if (results.Contains(result))
                    continue;
                buttons.Add(new DialogButton(this, result)
                {
                    Padding = new BoundingBox(40, 3),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                results.Add(result);
            }
            Results = results.ToArray();
            DialogButtons = GenerateGrid("buttons", buttons);
        }

        protected static GridLayout GetGridButtonsCentered(string id, IEnumerable<DialogButton> buttons)
        {
            GridLayout grid = new GridLayout(id);
            for (int i = 0; i < buttons.Count(); i++)
            {
                grid.Columns.Add(new GridColumn(new GridLength("*")));
                grid.AddChild(buttons.ElementAt(i), 0, i);
            }            
            return grid;
        }
    }

    /// <summary>
    /// Base class for dialog boxes to implement
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class BaseDialogBox<TData> : BaseDialogBox
    {
        public BaseDialogBox(string title) : base(title) { }

        /// <summary>
        /// Returns data model of the dialog box
        /// </summary>
        /// <returns></returns>
        public abstract TData GetData();

        /// <summary>
        /// Validates dialog box data model and adds validation errors to collection
        /// </summary>
        protected abstract override void Validate();
    }

    public enum DialogResult
    {
        None   = 0,
        Yes    = 1,  No     = 2,
        Ok     = 4,  Cancel = 8,
        Accept = 16, Abort  = 32,
        Skip   = 64, Retry  = 128
    }
}