using System.Collections.Generic;

using XApp.IoC;
using XApp.API;
using XApp.Bindings;
using XApp.Components.Complex;

namespace XApp.Components
{
    /// <summary>
    /// Allows selecting a directory
    /// </summary>
    public sealed class OpenDirectoryDialogBox : BaseDialogBox<string>
    {
        private string directory;
        private DialogResult[] validationIgnored = new[] { DialogResult.Cancel };
        
        public string Directory
        {
            get => directory;
            set
            {
                if (value == directory)
                    return;

                directory = value;
                FileSystemView.Show(directory);
            }
        }
        public TextBox SelectedDirectory { get; private set; }
        public DialogButton OkButton { get; private set; }
        public DialogButton CancelButton { get; private set; }
        public FileSystemTreeView FileSystemView { get; private set; }

        public OpenDirectoryDialogBox(string title) : base(title)
        {
            InitializeComponents();
            Directory = @"\";
        }

        private void InitializeComponents()
        {
            FileSystemView = new FileSystemTreeView("file-system-view");
            FileSystemView.Generator = (directoryItem) => new FileSystemTreeViewItem(directoryItem);
            SelectedDirectory = new TextBox("selected-directory");
            OkButton = new DialogButton(this, DialogResult.Ok);
            CancelButton = new DialogButton(this, DialogResult.Cancel);

            var selectedFileBinding = new ObjectBinding<FileSystemTreeView, TextBox>(FileSystemView, SelectedDirectory);
            selectedFileBinding.BindingKind = BindingKind.OneWay;
            selectedFileBinding.Bind(fsv => fsv.SelectedItem, tb => tb.Text,
                                     item => {
                                         if (item == null || !((FileSystemTreeViewItem)item).ObjectClass.IsFolder)
                                             return null;
                                         return ((FileSystemTreeViewItem)item).FullPath;
                                     }, null);
        }

        public override string GetData() => SelectedDirectory.Text;
        protected override bool Validate() => IoCContainer.Resolve<IFileManager>().DirectoryExists(SelectedDirectory.Text);
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}