using System.Linq;
using MIDE.Helpers;
using MIDE.FileSystem;
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
        private TreeView treeView;
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
            StackPanel stack = new StackPanel("container");
            stack.Orientation = StackOrientation.Vertical;
            searchBox = new ActionTextBox("search");
            searchBox.ActionButton.Caption = "@";
            searchBox.ActionButton.PressCommand = new RelayCommand(ShowCurrent);
            searchBox.Margin = new BoundingBox(0, 0, 0, 5);
            pathValidator = new PathValidator(false);
            pathValidator.AttachTo(searchBox, "Text");
            treeView = new TreeView("files-view");
            errorMessage = new Label("error-box", null);
            errorMessage.Visibility = Visibility.Collapsed;
            ToolbarButton homeButton = new ToolbarButton("home");
            homeButton.Order = 99;
            homeButton.PressCommand = new RelayCommand(GoHome);
            ToolbarButton backButton = new ToolbarButton("back");
            backButton.Order = 101;
            backButton.PressCommand = new RelayCommand(GoBack);
            ToolbarButton forwardButton = new ToolbarButton("forward");
            forwardButton.Order = 100;
            forwardButton.PressCommand = new RelayCommand(GoForward);

            var backButtonBinding1 = new ObjectBinding<ObservableCollection<string>, ToolbarButton>(browseHistory, backButton);
            backButtonBinding1.BindingKind = BindingKind.OneWay;
            backButtonBinding1.Bind(items => items.Count, button => button.IsEnabled,
                                    count => count > 0, b => 0);
            var backButtonBinding2 = new ObjectBinding<FileExplorer, ToolbarButton>(this, backButton);
            backButtonBinding2.BindingKind = BindingKind.OneWay;
            backButtonBinding2.Bind(ex => ex.CurrentHistoryIndex, button => button.IsEnabled,
                                    index => index > 0, b => 0,
                                    new Defaults<int, bool>(false));

            var forwardButtonBinding1 = new ObjectBinding<ObservableCollection<string>, ToolbarButton>(browseHistory, forwardButton);
            forwardButtonBinding1.BindingKind = BindingKind.OneWay;
            forwardButtonBinding1.Bind(coll => coll.Count, button => button.IsEnabled,
                                       count => count > 0, b => 0);
            var forwardButtonBinding2 = new ObjectBinding<FileExplorer, ToolbarButton>(this, forwardButton);
            forwardButtonBinding2.BindingKind = BindingKind.OneWay;
            forwardButtonBinding2.Bind(ex => ex.CurrentHistoryIndex, button => button.IsEnabled,
                                       index => index > 0, b => 0,
                                       new Defaults<int, bool>(false));

            var errorBinding = new ObjectBinding<PathValidator, Label>(pathValidator, errorMessage);
            errorBinding.BindingKind = BindingKind.OneWay;
            errorBinding.Bind(pv => pv.HasErrors, em => em.Visibility,
                              hasErrors =>
                              {
                                  errorMessage.Text = string.Join("\n", pathValidator.GetAllErrors().Select(e => e.ValidationMessage));
                                  return hasErrors ? Visibility.Visible : Visibility.Collapsed;
                              }, c => false,
                              new Defaults<bool, Visibility>(Visibility.Collapsed));

            TabToolbar.AddChild(backButton);
            TabToolbar.AddChild(forwardButton);
            TabToolbar.AddChild(homeButton);
            ContentContainer = stack;
            stack.AddChild(searchBox);
            stack.AddChild(errorMessage);
            stack.AddChild(treeView);
        }

        private void GoHome()
        {
            searchBox.Text = null;
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
            //if (pathValidator.HasErrors)
            //{
            //    errorMessage.Text = string.Join("\n", pathValidator.GetAllErrors().Select(e => e.ValidationMessage));
            //    errorMessage.Visibility = Visibility.Visible;
            //    return;
            //}
            //else
            //    errorMessage.Visibility = Visibility.Collapsed;

            treeView.Items.Clear();
            var items = Enumerable.Empty<DirectoryItem>();
            if (string.IsNullOrEmpty(searchBox.Text))
                items = FileSystemInfo.GetLogicalDrives();
            else
            {
                items = FileSystemInfo.GetDirectoryContents(searchBox.Text);
            }
            treeView.Items.AddRange(items
                    .Select(item => new FileExplorerItem(item.name, item.fullPath, item.itemClass)));

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
                if (string.IsNullOrEmpty(value))
                    return;
                if (!IO.Directory.Exists(value) && !IO.File.Exists(value))
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
        private bool isExpanded;
        private string fullPath;
        private FSObjectClass fsObjectClass;

        public override bool CanExpand => !ObjectClass.IsFile;
        public override bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (value)
                    Expand();
                isExpanded = value;
            }
        }
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
}