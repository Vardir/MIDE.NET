using System;
using MIDE.Helpers;
using System.Collections.Generic;
using MIDE.Application.Localization;

namespace MIDE.API.Components
{
    /// <summary>
    /// Top base class for dialog boxes
    /// </summary>
    public abstract class BaseDialogBox
    {
        private readonly HashSet<DialogResult> validationIgnoredResults;

        protected static readonly LocalizationProvider localization = LocalizationProvider.Instance;

        public DialogResult SelectedResult { get; set; }
        public string Title { get; }

        public event Action ResultSelected;

        public BaseDialogBox(string title)
        {
            Title = localization[title];
            validationIgnoredResults = new HashSet<DialogResult>();
            validationIgnoredResults.AddRange(GetValidationIgnoredResults());
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
        protected abstract IEnumerable<DialogResult> GetValidationIgnoredResults();
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