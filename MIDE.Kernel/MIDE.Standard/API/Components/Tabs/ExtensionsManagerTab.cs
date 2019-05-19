using MIDE.Application;
using MIDE.API.Commands;
using MIDE.API.ViewModels;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    public class ExtensionsManagerTab : Tab
    {
        public Button Install { get; private set; }
        public Button RestoreSelected { get; private set; }
        public Button UninstallSelected { get; private set; }
        public ListBox Extensions { get; private set; }

        public ExtensionsManagerTab(string id) : base(id, false)
        {
            InitializeComponents();
            IsSealed = true;
        }

        protected override void InitializeComponents()
        {
            Extensions = new ListBox("extensions");
            Extensions.Parent = this;
            Extensions.IsMultiselect = false;
            Install = new Button("install");
            Install.PressCommand = new RelayCommand(InstallExtension);
            UninstallSelected = new Button("uninstall");
            UninstallSelected.PressCommand = new RelayCommand(UninstallSelectedExtension);
            RestoreSelected = new Button("restore");
            RestoreSelected.PressCommand = new RelayCommand(RestoreSelectedEtension);

            AddChild(Install);
            AddChild(Extensions);
            AddChild(RestoreSelected);
            AddChild(UninstallSelected);

            var entries = ExtensionsManager.Instance.ExtensionsEntries;
            foreach (var entry in entries)
            {
                var item = new ExtensionItemViewModel(entry);
                item.IsEnabled = entry.IsEnabled;
                Extensions.Add(item);
            }
        }

        protected override Tab Create(string id, Toolbar toolbar, bool allowDuplicates)
        {
            ExtensionsManagerTab clone = new ExtensionsManagerTab(id);
            return clone;
        }

        private void InstallExtension()
        {

        }
        private void UninstallSelectedExtension()
        {
            if (Extensions.SelectedItems.Count == 0)
                return;
            (Extensions.SelectedItems[0].Value as ExtensionItemViewModel).PendingUninstall = true;
        }
        private void RestoreSelectedEtension()
        {
            if (Extensions.SelectedItems.Count == 0)
                return;
            (Extensions.SelectedItems[0].Value as ExtensionItemViewModel).PendingUninstall = false;
        }
    }

    public class ExtensionItemViewModel : BaseViewModel
    {
        private AppExtensionEntry entry;

        private string PendingUninstallMessage => "Extension is going to be uninstalled";

        public bool IsEnabled
        {
            get => entry.IsEnabled;
            set
            {
                if (value == entry.IsEnabled)
                    return;
                entry.IsEnabled = value;
                UpdateMessages();
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        public bool PendingUninstall
        {
            get => entry.PendingUninstall;
            set
            {
                if (value == entry.PendingUninstall)
                    return;
                entry.PendingUninstall = value;
                UpdateMessages();
                OnPropertyChanged(nameof(PendingUninstall));
            }
        }
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public string[] Dependencies { get; }
        public ObservableCollection<string> Messages { get; }

        public ExtensionItemViewModel(AppExtensionEntry entry)
        {
            this.entry = entry;
            Name = entry.Id;
            Version = entry.Version;
            Description = entry.Description;
            Dependencies = entry.Dependencies;
            Messages = new ObservableCollection<string>();
            UpdateMessages();
        }

        private void UpdateMessages()
        {
            if (!entry.PendingUninstall)
                Messages.Remove(PendingUninstallMessage);
            else if (!Messages.Contains(PendingUninstallMessage))
                Messages.Add(PendingUninstallMessage);
        }
    }
}