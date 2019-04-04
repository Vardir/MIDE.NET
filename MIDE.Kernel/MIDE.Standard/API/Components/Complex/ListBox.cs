using System;
using MIDE.API.ViewModels;

namespace MIDE.API.Components
{
    public class ListBox : LayoutContainer
    {


        public ListBox(string id) : base(id)
        {

        }

        public void AddChild(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

        }
        public void AddChild(BaseViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

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
    }

    public abstract class ListBoxItem : GridLayout
    {
        public ListBoxItem(string id) : base(id)
        {

        }
    }
}