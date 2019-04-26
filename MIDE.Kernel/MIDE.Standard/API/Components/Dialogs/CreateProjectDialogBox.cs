using System;
using System.Linq;
using Newtonsoft.Json;
using MIDE.FileSystem;
using MIDE.Application;
using MIDE.Schemes.JSON;
using MIDE.API.Validations;
using System.Collections.Generic;

namespace MIDE.API.Components
{
    public sealed class CreateProjectDialogBox : BaseDialogBox<ProjectCreationArgs>
    {
        private FileManager fileManager;
        private DialogResult[] validationIgnored = new[] { DialogResult.Cancel };

        public Button BrowseButton { get; private set; }
        public Label ProjectNameLabel { get; private set; }
        public Label ProjectPathLabel { get; private set; }
        public TextBox SearchBox { get; private set; }
        public TextBox ProjectName { get; private set; }
        public TextBox ProjectPath { get; private set; }
        public ListBox TemplatesList { get; private set; }
        public DialogButton AcceptButton { get; private set; }
        public DialogButton CancelButton { get; private set; }
        
        public CreateProjectDialogBox(string title) : base(title)
        {
            fileManager = AppKernel.Instance.FileManager;
            InitializeComponents();
            LoadTemplates();
        }

        public override ProjectCreationArgs GetData()
        {
            string path = ProjectPath.Text;
            string name = ProjectName.Text;
            ProjectScheme scheme = TemplatesList.SelectedItems[0].Value as ProjectScheme;
            return new ProjectCreationArgs(path, name, scheme);
        }

        protected override bool Validate()
        {
            bool result = true;
            result &= TemplatesList.SelectedItems.Count == 1;
            result &= ProjectName.Validations.All(v => !v.HasErrors);
            result &= ProjectPath.Validations.All(v => !v.HasErrors);
            return result;
        }
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;

        private void LoadTemplates()
        {
            string folder = fileManager.GetPath(ApplicationPath.Templates);
            folder = $"{folder}\\projects\\";
            if (!fileManager.Exists(folder))
                return;
            IEnumerable<string> files = fileManager.EnumerateFiles(folder, "*-proj.json");
            foreach (var def in files)
            {
                ProjectScheme scheme = null;
                try
                {
                    scheme = JsonConvert.DeserializeObject<ProjectScheme>(fileManager.ReadOrCreate(def));
                }
                catch (Exception ex)
                {
                    AppKernel.Instance.AppLogger.PushError(ex, this, "Can not read project template");
                    continue;
                }
                if (scheme.Icon == null)
                    scheme.Icon = $"{folder}icon.png";
                scheme.SchemePath = $"{folder}\\{def}";
                TemplatesList.Add(scheme);
            }
        }
        private void InitializeComponents()
        {
            BrowseButton = new Button("browse");
            SearchBox = new TextBox("search-box");
            ProjectName = new TextBox("project-name", "MyProject");
            ProjectPath = new TextBox("project-path", fileManager.GetPath(ApplicationPath.DefaultForProjects));
            TemplatesList = new ListBox("templates-list");
            ProjectNameLabel = new Label("project-name-label", "Project name:");
            ProjectPathLabel = new Label("project-path-label", "Project location:");
            AcceptButton = new DialogButton(this, DialogResult.Accept);
            CancelButton = new DialogButton(this, DialogResult.Cancel);

            TemplatesList.IsMultiselect = false;

            var pathValidation = new DefaultPathValidation();
            var nameValidation = new DefaultStringValidation(false, @"^[a-zA-Z0-9_]+[a-zA-Z0-9\.]*$");
            ProjectName.Validations.Add(nameValidation);
            ProjectPath.Validations.Add(pathValidation);
        }
    }

    public struct ProjectCreationArgs
    {
        public readonly string path;
        public readonly string name;
        public readonly ProjectScheme projectScheme;

        public ProjectCreationArgs(string path, string name, ProjectScheme scheme)
        {
            this.path = path;
            this.name = name;
            projectScheme = scheme;
        }
    }
}