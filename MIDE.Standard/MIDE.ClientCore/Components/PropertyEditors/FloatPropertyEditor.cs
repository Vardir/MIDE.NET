namespace MIDE.Components.PropertyEditors
{
    /// <summary>
    /// A basic single floating point number property editor
    /// </summary>
    public class FloatPropertyEditor : BasePropertyEditor<float>
    {
        public FloatPropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {

        }

        protected override BasePropertyEditor<float> Create(string id, bool isReadonly)
        {
            FloatPropertyEditor clone = new FloatPropertyEditor(id, isReadonly);
            return clone;
        }
    }
}
