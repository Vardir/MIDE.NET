using System;
using System.Linq;
using MIDE.Helpers;
using System.Drawing;
using MIDE.FileSystem;
using MIDE.Application;
using MIDE.API.Bindings;
using MIDE.API.Commands;
using MIDE.API.Validations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IO = System.IO;

namespace MIDE.API.Components
{
    public class FileExplorer : Tab
    {
        private bool _pushHistory;
        private int _currentHistoryIndex;

        public int CurrentHistoryIndex
        {
            get => _currentHistoryIndex;
            private set
            {
                if (_currentHistoryIndex == value)
                    return;
                _currentHistoryIndex = value;
                OnPropertyChanged(nameof(CurrentHistoryIndex));
            }
        }
        public ActionTextBox SearchBox { get; private set; }
        public TreeView TreeView { get; private set; }
        public ObservableCollection<string> BrowseHistory { get; }

        public FileExplorer(string id) : base(id)
        {
            _pushHistory = true;
            BrowseHistory = new ObservableCollection<string>();
            
            InitializeComponents();
            IsSealed = true;
        }

        public void Show(string path)
        {
            SearchBox.Text = path;
            ShowCurrent();
        }

        protected override void InitializeComponents()
        {            
            SearchBox = new ActionTextBox("search");
            SearchBox.ActionButton.Caption = null;
            SearchBox.ActionButton.ButtonGlyph = new Visuals.Glyph("\uf002") { AlternateColor = Color.White };
            SearchBox.ActionButton.PressCommand = new RelayCommand(ShowCurrent);
            SearchBox.Validations.Add(new PathValidation());

            TreeView = new TreeView("files-view");
            ToolbarButton homeButton = new ToolbarButton("home");
            homeButton.Caption = null;
            homeButton.Order = 99;
            homeButton.ButtonGlyph = new Visuals.Glyph("\uf015") { AlternateColor = Color.White };
            homeButton.PressCommand = new RelayCommand(GoHome);
            ToolbarButton backButton = new ToolbarButton("back");
            backButton.Caption = null;
            backButton.Order = 101;
            backButton.ButtonGlyph = new Visuals.Glyph("\uf048") { AlternateColor = Color.White };
            backButton.PressCommand = new RelayCommand(GoBack);
            ToolbarButton forwardButton = new ToolbarButton("forward");
            forwardButton.Caption = null;
            forwardButton.Order = 100;
            forwardButton.ButtonGlyph = new Visuals.Glyph("\uf051") { AlternateColor = Color.White };
            forwardButton.PressCommand = new RelayCommand(GoForward);

            var backButtonBinding1 = new ObjectBinding<ObservableCollection<string>, ToolbarButton>(BrowseHistory, backButton);
            backButtonBinding1.BindingKind = BindingKind.OneWay;
            backButtonBinding1.Bind(items => items.Count, button => button.IsEnabled,
                                    count => count > 0, null);
            var backButtonBinding2 = new ObjectBinding<FileExplorer, ToolbarButton>(this, backButton);
            backButtonBinding2.BindingKind = BindingKind.OneWay;
            backButtonBinding2.Bind(ex => ex.CurrentHistoryIndex, button => button.IsEnabled,
                                    index => index > 0, null,
                                    new Defaults<int, bool>(false));

            var forwardButtonBinding1 = new ObjectBinding<ObservableCollection<string>, ToolbarButton>(BrowseHistory, forwardButton);
            forwardButtonBinding1.BindingKind = BindingKind.OneWay;
            forwardButtonBinding1.Bind(coll => coll.Count, button => button.IsEnabled,
                                       count => count > 0, null);
            var forwardButtonBinding2 = new ObjectBinding<FileExplorer, ToolbarButton>(this, forwardButton);
            forwardButtonBinding2.BindingKind = BindingKind.OneWay;
            forwardButtonBinding2.Bind(ex => ex.CurrentHistoryIndex, button => button.IsEnabled,
                                       index => index < BrowseHistory.Count - 1, null,
                                       new Defaults<int, bool>(false));

            TabToolbar.AddChild(backButton);
            TabToolbar.AddChild(forwardButton);
            TabToolbar.AddChild(homeButton);

            AddChild(SearchBox);
            AddChild(TreeView);
        }

        protected override Tab Create(string id, Toolbar toolbar, bool allowDuplicates)
        {
            FileExplorer clone = new FileExplorer(id);
            clone.SearchBox = SearchBox.Clone() as ActionTextBox;
            clone.TreeView = TreeView.Clone() as TreeView;
            return clone;
        }

        private void GoHome()
        {
            SearchBox.Text = @"\";
            ShowCurrent();
        }
        private void GoBack()
        {
            if (BrowseHistory.Count == 0)
                return;
            _pushHistory = false;
            CurrentHistoryIndex = _currentHistoryIndex < 1 ? 0 : _currentHistoryIndex - 1;            
            Show(BrowseHistory[_currentHistoryIndex]);
            _pushHistory = true;
        }
        private void GoForward()
        {
            if (BrowseHistory.Count == 0)
                return;
            _pushHistory = false;
            CurrentHistoryIndex = _currentHistoryIndex > BrowseHistory.Count - 2 ? BrowseHistory.Count - 1 : _currentHistoryIndex + 1;
            Show(BrowseHistory[_currentHistoryIndex]);
            _pushHistory = true;
        }
        private void ShowCurrent()
        {
            if (SearchBox.ValidationErrors.Count > 0)
                return;

            TreeView.Items.Clear();
            var items = Enumerable.Empty<DirectoryItem>();
            if (SearchBox.Text == @"\")
                items = FileSystemInfo.GetLogicalDrives();
            else
                items = FileSystemInfo.GetDirectoryContents(SearchBox.Text);
            
            TreeView.Items.AddRange(items.Select(item => new FileExplorerItem(item)));

            if (_pushHistory)
            {
                BrowseHistory.Add(SearchBox.Text);
                CurrentHistoryIndex = BrowseHistory.Count - 1;
            }
        }

        protected class PathValidation : ValueValidation<string>
        {
            public PathValidation() { }

            protected override IEnumerable<(string, object)> Validate(string value)
            {
                if (!IO.Directory.Exists(value) && !IO.File.Exists(value) && value != @"\")
                {
                    yield return("The specified path does not exist", value);
                }
            }
        }
    }

    public class FileExplorerItem : TreeViewItem
    {
        private string fullPath;
        private FSObjectClass fsObjectClass;

        public override bool CanExpand => !ObjectClass.IsFile;
        public FSObjectClass ObjectClass
        {
            get => fsObjectClass;
            set
            {
                if (fsObjectClass == value)
                    return;
                fsObjectClass = value;
                ItemClass = fsObjectClass.Id;
                ItemGlyph = fsObjectClass.ObjectGlyph;
                OnPropertyChanged(nameof(ObjectClass));
            }
        }
        public string FullPath
        {
            get => fullPath;
            set
            {
                if (value == null || fullPath == value)
                    return;
                fullPath = value;
                OnPropertyChanged(nameof(FullPath));
            }
        }

        public FileExplorerItem(DirectoryItem directoryItem)
            : this(directoryItem.name, directoryItem.fullPath, directoryItem.itemClass)
        {
            ContextMenu = FileExplorerContextMenu.Instance.Select(this);
        }
        public FileExplorerItem(string caption, string fullPath, FSObjectClass fsObjectClass) : base()
        {
            ExpandCommand = new RelayCommand(Expand);

            Caption = caption;
            FullPath = fullPath;
            ObjectClass = fsObjectClass;

            ContextMenu = FileExplorerContextMenu.Instance.Select(this);

            ClearChildren();
        }
        
        protected override TreeViewItem CloneInternal()
        {
            FileExplorerItem clone = new FileExplorerItem(Caption, fullPath, fsObjectClass);
            return clone;
        }
        protected override IEnumerable<TreeViewItem> GetChildItems()
        {
            return FileSystemInfo.GetDirectoryContents(FullPath)
                   .Select(item => new FileExplorerItem(item));
        }

        protected override void OnChildrenCleared()
        {
            if (!ObjectClass.IsFile)
                Children.Add(null);
        }
    }

    public class FileExplorerContextMenu
    {
        private static FileExplorerContextMenu instance;
        public static FileExplorerContextMenu Instance => instance ?? (instance = new FileExplorerContextMenu());

        private Dictionary<string, ContextMenu> contextMenuSchemes;
        
        private FileExplorerContextMenu ()
        {
            contextMenuSchemes = new Dictionary<string, ContextMenu>()
            {
                ["__default"] = new ContextMenu("default-contextmenu"),
                ["file"] = new ContextMenu("file-contextmenu"),
                ["folder"] = new ContextMenu("folder-contextmenu"),
                ["drive"] = new ContextMenu("drive-contextmenu")
            };
            InitializeSchemes();
        }

        public void AddScheme(string key, ContextMenu menuScheme)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (menuScheme == null)
                throw new ArgumentNullException(nameof(menuScheme));
            if (contextMenuSchemes.ContainsKey(key))
                contextMenuSchemes[key] = menuScheme;
            else
                contextMenuSchemes.Add(key, menuScheme);
        }

        public bool HasMenuScheme(string key) => contextMenuSchemes.ContainsKey(key);
        public bool HasMenuScheme(Func<ContextMenu, bool> predicate)
        {
            foreach (var kvp in contextMenuSchemes)
            {
                if (predicate(kvp.Value))
                    return true;
            }
            return false;
        }
        public ContextMenu Select(TreeViewItem item)
        {
            if (!(item is FileExplorerItem fItem))
                throw new ArgumentException($"Expected [FileExplorerItem] but got [{item?.GetType()}]");
            return Select(fItem);
        }
        public ContextMenu Select(FileExplorerItem item)
        {
            contextMenuSchemes.TryGetValue(item.ItemClass, out ContextMenu scheme);
            if (scheme == null)
            {
                if (item.ObjectClass.IsFile)
                    scheme = contextMenuSchemes["file"];
                else if (item.ObjectClass.IsFolder)
                    scheme = contextMenuSchemes["folder"];
            }
            return scheme ?? contextMenuSchemes["__default"];
        }

        private void InitializeSchemes()
        {
            var flscheme = contextMenuSchemes["file"];
            flscheme.Add(new MenuButton("delete", -98)
            {
                PressCommand = new RelayCommand(Delete)
            }, null);

            var fdscheme = contextMenuSchemes["folder"];
            var addbtn = new MenuButton("add", -99);
            addbtn.Add(new MenuButton("new-empty-file", -99)
            {
                Caption = "New empty file...",
                PressCommand = new RelayCommand(NewEmptyFile)
            }, null);
            addbtn.Add(new MenuButton("new-folder", -98)
            {
                Caption = "New empty folder...",
                PressCommand = new RelayCommand(NewFolder)
            }, null);
            fdscheme.Add(addbtn, null);
            fdscheme.MergeWith(flscheme);
            
            flscheme.Add(new MenuButton("open", "common", -99), null);
            flscheme.Add(new MenuButton("properties", 0)
            {
                PressCommand = new RelayCommand(FileProperties)
            }, null);

            var dvscheme = contextMenuSchemes["drive"];
            dvscheme.MergeWith(fdscheme);
        }

        public static void NewEmptyFile()
        {
            var dialogBox = new TextBoxDialogBox("New file", "File name");
            var (dialogResult, name) = AppKernel.Instance.UIManager.OpenDialog(dialogBox);
            if (dialogResult == DialogResult.Cancel)
                return;

            var tab = AppKernel.Instance.UIManager.GetTab<FileExplorer>();
            var fei = tab.TreeView
                .SelectedItems.FirstWith(item => (item as FileExplorerItem).ItemClass == "folder", item => item as FileExplorerItem);
            if (fei == null)
                return;
            string file = FileManager.Instance.Combine(fei.FullPath, name);
            string template = ProjectManager.Instance.FindBy(IO.Path.GetExtension(name))?.ObjectTemplate;
            string message = FileManager.Instance.MakeFile(file, template);
            if (message != null)
            {
                var messageBox = new MessageDialogBox("Error", message);
                AppKernel.Instance.UIManager.OpenDialog(messageBox);
            }
            fei.IsExpanded = false;
            fei.IsExpanded = true;
        }
        public static void NewFolder()
        {
            var dialogBox = new TextBoxDialogBox("New folder", "Folder name");
            var (dialogResult, name) = AppKernel.Instance.UIManager.OpenDialog(dialogBox);
            if (dialogResult == DialogResult.Cancel)
                return;

            var tab = AppKernel.Instance.UIManager.GetTab<FileExplorer>();
            var fei = tab.TreeView
                .SelectedItems.FirstWith(item => (item as FileExplorerItem).ItemClass != "file", item => item as FileExplorerItem);
            if (fei == null)
                return;
            string folder = FileManager.Instance.Combine(fei.FullPath, name);
            string message = FileManager.Instance.MakeFolder(folder);
            if (message != null)
            {
                var messageBox = new MessageDialogBox("Error", message);
                AppKernel.Instance.UIManager.OpenDialog(messageBox);
            }
            fei.IsExpanded = false;
            fei.IsExpanded = true;
        }
        public static void Delete()
        {
            var tab = AppKernel.Instance.UIManager.GetTab<FileExplorer>();
            foreach (var item in tab.TreeView.SelectedItems)
            {
                if (!(item is FileExplorerItem fileExplorerItem))
                    continue;
                var parent = fileExplorerItem.Parent;
                string message = FileManager.Instance.Delete(fileExplorerItem.FullPath);
                if (message != null)
                {
                    var messageBox = new MessageDialogBox("Error", message);
                    AppKernel.Instance.UIManager.OpenDialog(messageBox);
                    return;
                }
                if (parent != null)
                    parent.Children.Remove(fileExplorerItem);
                else
                    tab.TreeView.Items.Remove(fileExplorerItem);
            }
        }
        public static void FileProperties()
        {
            var tab = AppKernel.Instance.UIManager.GetTab<FileExplorer>();
            var fei = tab.TreeView
                .SelectedItems.FirstWith(item => (item as FileExplorerItem).ItemClass != "folder", item => item as FileExplorerItem);

        }
    }
}