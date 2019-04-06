using System.Linq;
using MIDE.API.Validations;
using System.Collections.Generic;

namespace MIDE.API.Components
{
    public sealed class CreateProjectDialogBox : BaseDialogBox<(string template, string path)>
    {        
        private DialogResult[] results = new[] { DialogResult.Accept, DialogResult.Cancel };
        private DialogResult[] validationIgnored = new[] { DialogResult.Cancel };

        public Button BrowseButton { get; private set; }
        public Label ProjectNameLabel { get; private set; }
        public Label ProjectPathLabel { get; private set; }
        public TextBox SearchBox { get; private set; }
        public TextBox ProjectName { get; private set; }
        public TextBox ProjectPath { get; private set; }
        public ListBox TemplatesList { get; private set; }

        public override IEnumerable<DialogResult> Results => results;

        public CreateProjectDialogBox(string title) : base(title)
        {
            InitializeComponents();
            #region test
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item");
            TemplatesList.Add("item"); 
            #endregion
        }

        public override (string template, string path) GetData() => (ProjectName.Text, ProjectPath.Text);

        private void InitializeComponents()
        {
            BrowseButton     = new Button("browse");            
            SearchBox        = new TextBox("search-box");
            ProjectName      = new TextBox("project-name");
            ProjectPath      = new TextBox("project-path");
            TemplatesList    = new ListBox("templates-list");
            ProjectNameLabel = new Label("project-name-label", "Project name:");
            ProjectPathLabel = new Label("project-path-label", "Project location:");

            var nameValidation = new DefaultStringPropertyValidation(false, false, @"^[a-zA-Z0-9_]+[a-zA-Z0-9\.]*$");
            var pathValidation1 = new DefaultStringPropertyValidation(false, false);
            var pathValidation2 = new PathPropertyValidation(false);
            nameValidation.AttachTo(ProjectName, "Text");
            pathValidation1.AttachTo(ProjectPath, "Text");
            pathValidation2.AttachTo(ProjectPath, "Text");
        }

        protected override bool Validate()
        {
            bool result = true;
            result &= ProjectName.Validations.All(v => !v.HasErrors);
            result &= ProjectPath.Validations.All(v => !v.HasErrors);
            return result;
        }
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}