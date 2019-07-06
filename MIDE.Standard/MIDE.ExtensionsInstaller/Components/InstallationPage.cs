using System;
using MIDE.Components;
using System.Collections.ObjectModel;
using MIDE.ExtensionsInstaller.ViewModels;

namespace MIDE.ExtensionsInstaller.Components
{
    public class InstallationPage : Tab
    {
        public ObservableCollection<InstallationActionViewModel> Actions { get; }

        public InstallationPage() : base("installation-page", false)
        {
            Actions = new ObservableCollection<InstallationActionViewModel>();
        }

        protected override Tab Create(string id, Toolbar toolbar, bool allowDuplicates)
        {
            throw new NotImplementedException();
        }
    }
}