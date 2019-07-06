namespace MIDE.Components.PropertyEditors
{
    /// <summary>
    /// A basic int64 property editor
    /// </summary>
    public class Int64PropertyEditor : BasePropertyEditor<long>
    {
        public Int64PropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {

        }

        protected override BasePropertyEditor<long> Create(string id, bool isReadonly)
        {
            Int64PropertyEditor clone = new Int64PropertyEditor(id, isReadonly);
            return clone;
        }
    }
}
