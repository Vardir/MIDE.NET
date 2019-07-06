namespace MIDE.Components.PropertyEditors
{
    /// <summary>
    /// A basic boolean property editor
    /// </summary>
    public class BoolPropertyEditor : BasePropertyEditor<bool>
    {
        public BoolPropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {

        }

        protected override BasePropertyEditor<bool> Create(string id, bool isReadonly)
        {
            BoolPropertyEditor clone = new BoolPropertyEditor(id, isReadonly);
            return clone;
        }
    }
}
