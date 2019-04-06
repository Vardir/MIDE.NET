using System;
using System.Linq;
using MIDE.Helpers;
using MIDE.API.Measurements;
using System.Collections.Generic;

namespace MIDE.API.Components
{
    /// <summary>
    /// Top base class for dialog boxes
    /// </summary>
    public abstract class BaseDialogBox : GridLayout
    {
        private readonly HashSet<DialogResult> validationIgnoredResults;

        protected readonly GridLayout body;

        public DialogResult SelectedResult { get; set; }
        public string Title { get; }
        public abstract IEnumerable<DialogResult> Results { get; }

        public event Action ResultSelected;

        public BaseDialogBox(string title) : base("dialog-box")
        {
            Title = title;
            validationIgnoredResults = new HashSet<DialogResult>();

            body = new GridLayout("body");
            InitializeComponents();
        }

        /// <summary>
        /// Validates data model of the dialog box and closes it if there are no errors occurred
        /// </summary>
        public void ValidateAndAccept()
        {
            if (SelectedResult == DialogResult.None)
                return;
            bool close = true;
            if(!validationIgnoredResults.Contains(SelectedResult))
                close = Validate();
            if (close)
            {
                ResultSelected?.Invoke();
            }
        }

        protected abstract bool Validate();
        protected abstract GridLayout GenerateButtonsGrid(string id, IEnumerable<DialogButton> buttons);
        protected abstract IEnumerable<DialogResult> GetValidationIgnoredResults();

        protected override void InitializeComponents()
        {
            MinWidth = 300;
            MinHeight = 200;
            MaxWidth = 640;
            MaxHeight = 640;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            RowMargin = 10;
            Rows.Add(new GridRow("*"));
            Rows.Add(new GridRow("auto"));
            AddChild(body, 0, 0);

            SetDialogResults(Results);
            validationIgnoredResults.AddRange(GetValidationIgnoredResults());
            IsSealed = true;
        }

        private void SetDialogResults(IEnumerable<DialogResult> dialogResults)
        {
            int count = dialogResults.Count();
            List<DialogResult> results = new List<DialogResult>(count);
            List<DialogButton> buttons = new List<DialogButton>(count);
            foreach (var result in dialogResults)
            {
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
            AddChild(GenerateButtonsGrid("buttons", buttons), 1, 0);
        }

        protected static GridLayout GetGridButtonsCentered(string id, IEnumerable<DialogButton> buttons)
        {
            GridLayout grid = new GridLayout(id);
            grid.ColumnMargin = new GridLength(10);
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