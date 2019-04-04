using MIDE.API.Measurements;
using System.Collections.Generic;

namespace MIDE.API.Components
{
    public class CreateProjectDialogBox : BaseDialogBox<(string template, string path)>
    {
        private readonly TextBox searchBox;
        private readonly TextBox projectName;
        private readonly TextBox projectPath;

        public CreateProjectDialogBox(string title) : base(title)
        {
            searchBox = new TextBox("search-box");
            projectName = new TextBox("project-name");
            projectPath = new TextBox("project-path");
            BuildBody();
        }

        public override (string template, string path) GetData() => (projectName.Text, projectPath.Text);

        protected override void Validate()
        {
            ValidationErrors.Clear();
            if (SelectedResult == DialogResult.Cancel)
                return;
            if (string.IsNullOrWhiteSpace(projectName.Text))
                ValidationErrors.Add("Project name is empty!");
            if (string.IsNullOrWhiteSpace(projectPath.Text))
                ValidationErrors.Add("Project path is empty!");
        }
        protected override GridLayout GenerateGrid(string id, IEnumerable<DialogButton> buttons)
        {
            return GetGridButtonsCentered(id, buttons);
        }

        private void BuildBody()
        {
            Rows.Add(new GridRow(new GridLength("auto")));
            Rows.Add(new GridRow(new GridLength(10)));
            Rows.Add(new GridRow(new GridLength("*")));
            Rows.Add(new GridRow(new GridLength(10)));
            Rows.Add(new GridRow(new GridLength("auto")));
            Rows.Add(new GridRow(new GridLength(10)));
            Rows.Add(new GridRow(new GridLength("auto")));
            Columns.Add(new GridColumn(new GridLength("auto")));
            Columns.Add(new GridColumn(new GridLength(5)));
            Columns.Add(new GridColumn(new GridLength("*")));
            Columns.Add(new GridColumn(new GridLength(5)));
            Columns.Add(new GridColumn(new GridLength("auto")));
            AddChild(searchBox, 0, 0, 1, 3);
        }
    }
}

/* <Grid>
        <TextBox Text="..." Grid.ColumnSpan="3"/>
        <ListBox Grid.ColumnSpan="5" Grid.Row="2"
                 Padding="5 0"
                 Background="{StaticResource DataGrid_Background_Brush}">
            <ListBoxItem Margin="5 5">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="auto"/>
                        <RowDefinition Height="5"/>
                        <RowDefinition Height="auto"/>
                    </Grid.RowDefinitions>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="auto"/>
                        <ColumnDefinition Width="15"/>
                        <ColumnDefinition/>
                    </Grid.ColumnDefinitions>

                    <TextBlock Text="X" FontSize="35" Foreground="White"
                               Grid.RowSpan="3"/>
                    <TextBlock Text="Project Template" Foreground="White"
                               FontSize="18"
                               Grid.Column="2"/>
                    <TextBlock Text="Just a test project template"
                               Foreground="Silver" FontSize="16"
                               Grid.Column="2" Grid.Row="2"/>
                </Grid>
            </ListBoxItem>
            <ListBoxItem Margin="5 5">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="auto"/>
                        <RowDefinition Height="5"/>
                        <RowDefinition Height="auto"/>
                    </Grid.RowDefinitions>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="auto"/>
                        <ColumnDefinition Width="15"/>
                        <ColumnDefinition/>
                    </Grid.ColumnDefinitions>

                    <TextBlock Text="X" FontSize="35" Foreground="White"
                               Grid.RowSpan="3"/>
                    <TextBlock Text="Project Template" Foreground="White"
                               FontSize="18"
                               Grid.Column="2"/>
                    <TextBlock Text="Just a test project template"
                               Foreground="Silver" FontSize="16"
                               Grid.Column="2" Grid.Row="2"/>
                </Grid>
            </ListBoxItem>
            <ListBoxItem Margin="5 5">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="auto"/>
                        <RowDefinition Height="5"/>
                        <RowDefinition Height="auto"/>
                    </Grid.RowDefinitions>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="auto"/>
                        <ColumnDefinition Width="15"/>
                        <ColumnDefinition/>
                    </Grid.ColumnDefinitions>

                    <TextBlock Text="X" FontSize="35" Foreground="White"
                               Grid.RowSpan="3"/>
                    <TextBlock Text="Project Template" Foreground="White"
                               FontSize="18"
                               Grid.Column="2"/>
                    <TextBlock Text="Just a test project template"
                               Foreground="Silver" FontSize="16"
                               Grid.Column="2" Grid.Row="2"/>
                </Grid>
            </ListBoxItem>
        </ListBox>

        <TextBlock Text="Project name:"
                   Grid.Row="4" Foreground="White"/>
        <TextBox Text="MyProject"
                 Grid.Row="4" Grid.Column="2"/>
        
        <TextBlock Text="Project location:"
                   Grid.Row="6" Foreground="White"/>
        <TextBox Text="F:\Projects\"
                 Grid.Row="6" Grid.Column="2"/>
        <Button Content="Browse"
                Grid.Row="6" Grid.Column="4"/>        
    </Grid> */