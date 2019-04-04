using System;
using System.Linq;
using MIDE.Helpers;
using System.Drawing;
using MIDE.FileSystem;
using MIDE.Application;
using MIDE.API.Bindings;
using MIDE.API.Commands;
using MIDE.API.Validation;
using MIDE.API.Measurements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IO = System.IO;

namespace MIDE.API.Components
{
    public class FileExplorer : Tab
    {
        private bool _pushHistory;
        private int _currentHistoryIndex;
        private Label errorMessage;
        private ActionTextBox searchBox;
        private PathValidator pathValidator;
        private ObservableCollection<string> browseHistory;

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
        public TreeView TreeView { get; private set; }

        public FileExplorer(string id) : base(id)
        {
            IsSealed = true;
            _pushHistory = true;
            browseHistory = new ObservableCollection<string>();
            
            InitializeComponents();
        }

        public void Show(string path)
        {
            searchBox.Text = path;
            ShowCurrent();
        }

        protected override void InitializeComponents()
        {
            GridLayout grid = new GridLayout("container");
            grid.Rows.AddRange(new[]{
                new GridRow(new GridLength("auto")),
                new GridRow(new GridLength("auto")),
                new GridRow(new GridLength("*"))
                });
            grid.Columns.Add(new GridColumn(new GridLength("*")));
            searchBox = new ActionTextBox("search");
            searchBox.ActionButton.Caption = null;
            searchBox.ActionButton.ButtonGlyph = new Visuals.Glyph("\uf002") { AlternateColor = Color.White };
            searchBox.ActionButton.PressCommand = new RelayCommand(ShowCurrent);
            searchBox.Margin = new BoundingBox(0, 0, 0, 5);
            pathValidator = new PathValidator(false);
            pathValidator.AttachTo(searchBox, "Text");
            TreeView = new TreeView("files-view");
            errorMessage = new Label("error-box", null);
            errorMessage.Visibility = Visibility.Collapsed;
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

            var backButtonBinding1 = new ObjectBinding<ObservableCollection<string>, ToolbarButton>(browseHistory, backButton);
            backButtonBinding1.BindingKind = BindingKind.OneWay;
            backButtonBinding1.Bind(items => items.Count, button => button.IsEnabled,
                                    count => count > 0, null);
            var backButtonBinding2 = new ObjectBinding<FileExplorer, ToolbarButton>(this, backButton);
            backButtonBinding2.BindingKind = BindingKind.OneWay;
            backButtonBinding2.Bind(ex => ex.CurrentHistoryIndex, button => button.IsEnabled,
                                    index => index > 0, null,
                                    new Defaults<int, bool>(false));

            var forwardButtonBinding1 = new ObjectBinding<ObservableCollection<string>, ToolbarButton>(browseHistory, forwardButton);
            forwardButtonBinding1.BindingKind = BindingKind.OneWay;
            forwardButtonBinding1.Bind(coll => coll.Count, button => button.IsEnabled,
                                       count => count > 0, null);
            var forwardButtonBinding2 = new ObjectBinding<FileExplorer, ToolbarButton>(this, forwardButton);
            forwardButtonBinding2.BindingKind = BindingKind.OneWay;
            forwardButtonBinding2.Bind(ex => ex.CurrentHistoryIndex, button => button.IsEnabled,
                                       index => index < browseHistory.Count - 1, null,
                                       new Defaults<int, bool>(false));

            var errorBinding = new ObjectBinding<PathValidator, Label>(pathValidator, errorMessage);
            errorBinding.BindingKind = BindingKind.OneWay;
            errorBinding.Bind(pv => pv.HasErrors, em => em.Visibility,
                              hasErrors =>
                              {
                                  errorMessage.Text = string.Join("\n", pathValidator.GetAllErrors().Select(e => e.ValidationMessage));
                                  return hasErrors ? Visibility.Visible : Visibility.Collapsed;
                              }, null,
                              new Defaults<bool, Visibility>(Visibility.Collapsed));

            TabToolbar.AddChild(backButton);
            TabToolbar.AddChild(forwardButton);
            TabToolbar.AddChild(homeButton);
            ContentContainer = grid;
            grid.AddChild(searchBox, 0, 0);
            grid.AddChild(errorMessage, 1, 0);
            grid.AddChild(TreeView, 2, 0);
        }

        private void GoHome()
        {
            searchBox.Text = @"\";
            ShowCurrent();
        }
        private void GoBack()
        {
            if (browseHistory.Count == 0)
                return;
            _pushHistory = false;
            CurrentHistoryIndex = _currentHistoryIndex < 1 ? 0 : _currentHistoryIndex - 1;            
            Show(browseHistory[_currentHistoryIndex]);
            _pushHistory = true;
        }
        private void GoForward()
        {
            if (browseHistory.Count == 0)
                return;
            _pushHistory = false;
            CurrentHistoryIndex = _currentHistoryIndex > browseHistory.Count - 2 ? browseHistory.Count - 1 : _currentHistoryIndex + 1;
            Show(browseHistory[_currentHistoryIndex]);
            _pushHistory = true;
        }
        private void ShowCurrent()
        {
            if (pathValidator.HasErrors)
                return;

            TreeView.Items.Clear();
            var items = Enumerable.Empty<DirectoryItem>();
            if (searchBox.Text == @"\")
                items = FileSystemInfo.GetLogicalDrives();
            else
                items = FileSystemInfo.GetDirectoryContents(searchBox.Text);
            
            TreeView.Items.AddRange(items.Select(item => {
                var fei = new FileExplorerItem(item.name, item.fullPath, item.itemClass);
                fei.ContextMenu = FileExplorerContextMenu.Instance.Select(fei);
                return fei;
            }));

            if (_pushHistory)
            {
                browseHistory.Add(searchBox.Text);
                CurrentHistoryIndex = browseHistory.Count - 1;
            }
        }
        
        protected class PathValidator : PropertyValidator<string>
        {
            public PathValidator(bool raiseExceptionOnError) : base(raiseExceptionOnError) { }

            protected override void Validate(string property, string value)
            {
                if (!IO.Directory.Exists(value) && !IO.File.Exists(value) && value != @"\")
                {
                    AddError(property, "The specified path does not exist", value);
                    return;
                }
                ClearErrors();
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

        public FileExplorerItem(string caption, string fullPath, FSObjectClass fsObjectClass) : base()
        {
            ExpandCommand = new RelayCommand(Expand);

            Caption = caption;
            FullPath = fullPath;
            ObjectClass = fsObjectClass;
            
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
                   .Select(item => new FileExplorerItem(item.name, item.fullPath, item.itemClass));
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
                ["folder"] = new ContextMenu("folder-contextmenu")
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
            if (contextMenuSchemes.TryGetValue(item.ItemClass, out ContextMenu scheme)) { }

            return scheme ?? contextMenuSchemes["__default"];
        }

        private void InitializeSchemes()
        {
            var flscheme = contextMenuSchemes["file"];
            flscheme.AddItem(new MenuButton("open", -99));
            flscheme.AddItem(new MenuSplitter("splitter-1", -90));
            flscheme.AddItem(new MenuButton("properties", 0)
            {
                PressCommand = new RelayCommand(FileProperties)
            });

            var fdscheme = contextMenuSchemes["folder"];
            var addbtn = new MenuButton("add", -99);
            addbtn.Add(new MenuButton("new-empty-file", -99) { Caption = "New empty file..." }, null);
            addbtn.Add(new MenuButton("new-folder", -98), null);
            fdscheme.AddItem(addbtn);
        }

        public static void FileProperties()
        {
            var tab = AppKernel.Instance.UIManager.GetTab<FileExplorer>();
            var fei = tab.TreeView
                .SelectedItems.FirstWith(item => (item as FileExplorerItem).ItemClass != "folder", item => item as FileExplorerItem);

        }
    }
}