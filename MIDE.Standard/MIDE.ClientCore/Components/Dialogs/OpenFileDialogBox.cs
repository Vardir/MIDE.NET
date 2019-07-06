using MIDE.Bindings;
using MIDE.FileSystem;
using MIDE.Components.Complex;
using System.Collections.Generic;

namespace MIDE.Components
{
    /// <summary>
    /// Allows selecting a file
    /// </summary>
    public sealed class OpenFileDialogBox : BaseDialogBox<string>
    {
        private string directory;
        private DialogResult[] validationIgnored = new[] { DialogResult.Cancel };

        public string Filter { get; }

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
        public TextBox SelectedFile { get; private set; }
        public DialogButton OkButton { get; private set; }
        public DialogButton CancelButton { get; private set; }
        public FileSystemTreeView FileSystemView { get; private set; }

        public OpenFileDialogBox(string title, string filter) : base(title)
        {
            Filter = filter;
            InitializeComponents();
            Directory = @"\";
        }

        private void InitializeComponents()
        {
            FileSystemView = new FileSystemTreeView("file-system-view");
            FileSystemView.Generator = (directoryItem) => new FileSystemTreeViewItem(directoryItem);
            SelectedFile = new TextBox("selected-file");
            OkButton = new DialogButton(this, DialogResult.Ok);
            CancelButton = new DialogButton(this, DialogResult.Cancel);

            var selectedFileBinding = new ObjectBinding<FileSystemTreeView, TextBox>(FileSystemView, SelectedFile);
            selectedFileBinding.BindingKind = BindingKind.OneWay;
            selectedFileBinding.Bind(fsv => fsv.SelectedItem, tb => tb.Text,
                                     item => {
                                         if (item == null || !((FileSystemTreeViewItem)item).ObjectClass.IsFile)
                                             return null;
                                         return ((FileSystemTreeViewItem)item).FullPath;
                                     }, null);
        }

        public override string GetData() => SelectedFile.Text;
        protected override bool Validate() => FileManager.FileExists(SelectedFile.Text);
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}