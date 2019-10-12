namespace Vardirsoft.XApp.Components.PropertyEditors
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
            var clone = new Int16PropertyEditor(id, isReadonly);

            return clone;
        }
    }
}
