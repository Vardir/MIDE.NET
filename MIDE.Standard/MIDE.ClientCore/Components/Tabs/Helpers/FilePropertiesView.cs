using System;
using MIDE.API;
using MIDE.IoC;

namespace MIDE.Components
{    
    public class FilePropertiesView : PropertiesView
    {
        public FilePropertiesView(string id) : base(id)
        {
            IsSealed = true;
            InitializeComponents();
        }

        public override void ShowFor(object context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context is string path)
                ShowProperties(path);
            //TODO: add context types
            throw new ArgumentException($"Invalid context given: [{context.GetType()}] ^{context.GetHashCode()}");
        }
        public void ShowProperties(string path)
        {
            if (IoCContainer.Resolve<IFileManager>().Exists(path))
            {
                //var array = FileManager.ExtractProperties(path).ToArray();
                //EnsureRowCount(array.Length + 1);
                //for (int i = 0; i < array.Length; i++)
                //{
                //    int index = i + 1;
                //    AddChild(new Label($"lb-prop{index}-caption", array[i].prop), index, 0);
                //    AddChild(new Label($"lb-prop{index}", array[i].val), index, 1);
                //}
                //(Children[1].Component as Label).Text = FileManager.ExtractName(path);
            }

            throw new ArgumentException("Invalid path given", nameof(path));            
        }

        protected override void InitializeComponents()
        {
            //NewRowTemplate = new GridRow(new GridLength("auto"));

            //Columns.Add(new GridColumn(new GridLength("auto")));
            //Columns.Add(new GridColumn(new GridLength("*")));

            //AddRow(new LayoutComponent[] { new Label("prop-capt", "File properties"), new Label("file-name", "") });
        }

        public override bool Contains(string id)
        {
            throw new NotImplementedException();
        }

        public override LayoutComponent Find(string id)
        {
            throw new NotImplementedException();
        }

        protected override void AddChild_Impl(LayoutComponent component)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveChild_Impl(string id)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveChild_Impl(LayoutComponent component)
        {
            throw new NotImplementedException();
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            throw new NotImplementedException();
        }
    }
}