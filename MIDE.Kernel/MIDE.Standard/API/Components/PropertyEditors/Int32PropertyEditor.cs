namespace MIDE.API.Components.PropertyEditors
{
    /// <summary>
    /// A basic int32 property editor
    /// </summary>
    public class Int32PropertyEditor : BasePropertyEditor<int>
    {
        public Int32PropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {

        }

        protected override BasePropertyEditor<int> Create(string id, bool isReadonly)
        {
            Int32PropertyEditor clone = new Int32PropertyEditor(id, isReadonly);
            return clone;
        }
    }
}