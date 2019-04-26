namespace MIDE.API.Components.PropertyEditors
{
    /// <summary>
    /// A basic double floating point number property editor
    /// </summary>
    public class DoublePropertyEditor : BasePropertyEditor<double>
    {
        public DoublePropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {

        }

        protected override BasePropertyEditor<double> Create(string id, bool isReadonly)
        {
            DoublePropertyEditor clone = new DoublePropertyEditor(id, isReadonly);
            return clone;
        }
    }
}
