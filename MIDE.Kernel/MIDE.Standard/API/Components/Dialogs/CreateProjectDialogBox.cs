using System.Collections.Generic;

namespace MIDE.API.Components
{
    public sealed class CreateProjectDialogBox : BaseDialogBox<(string template, string path)>
    {
        private TextBox searchBox;
        private TextBox projectName;
        private TextBox projectPath;
        private ListBox templatesList;
        private DialogResult[] results = new[] { DialogResult.Accept, DialogResult.Cancel };
        private DialogResult[] validationIgnored = new[] { DialogResult.Cancel };

        public override IEnumerable<DialogResult> Results => results;

        public CreateProjectDialogBox(string title) : base(title)
        {
            MinWidth = 250;
            MinHeight = 250;
            InitializeComponents();
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
            templatesList.Add("item");
        }

        public override (string template, string path) GetData() => (projectName.Text, projectPath.Text);

        protected override void InitializeComponents()
        {
            Button browse = new Button("browse");
            Label projectNameLabel = new Label("project-name-label", "Project name:");
            Label projectPathLabel = new Label("project-path-label", "Project location:");
            searchBox     = new TextBox("search-box");
            projectName   = new TextBox("project-name");
            projectPath   = new TextBox("project-path");
            templatesList = new ListBox("templates-list");
            templatesList.Padding = "5 0";

            RowMargin = 10;
            body.Rows.Add(new GridRow("auto"));
            body.Rows.Add(new GridRow("*"));
            body.Rows.Add(new GridRow("auto"));
            body.Rows.Add(new GridRow("auto"));
            body.Columns.Add(new GridColumn("auto"));
            body.Columns.Add(new GridColumn(5));
            body.Columns.Add(new GridColumn("*"));
            body.Columns.Add(new GridColumn(5));
            body.Columns.Add(new GridColumn("auto"));

            body.AddChild(searchBox, 0, 0, 1, 3);
            body.AddChild(templatesList, 2, 0, 1, 5);
            body.AddChild(projectNameLabel, 4, 0);
            body.AddChild(projectName, 4, 2);
            body.AddChild(projectPathLabel, 6, 0);
            body.AddChild(projectPath, 6, 2);
            body.AddChild(browse, 6, 4);
        }

        protected override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(projectName.Text))
                return false;
            if (string.IsNullOrWhiteSpace(projectPath.Text))
                return false;
            return true;
        }
        protected override GridLayout GenerateButtonsGrid(string id, IEnumerable<DialogButton> buttons)
        {
            return GetGridButtonsCentered(id, buttons);
        }
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}