namespace MIDE.Components.PropertyEditors
{
    /// <summary>
    /// A basic int16 property editor
    /// </summary>
    public class Int16PropertyEditor : BasePropertyEditor<short>
    {
        public Int16PropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {

        }

        protected override BasePropertyEditor<short> Create(string id, bool isReadonly)
        {
            Int16PropertyEditor clone = new Int16PropertyEditor(id, isReadonly);
            return clone;
        }
    }
}
