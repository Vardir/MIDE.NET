using System;

namespace Vardirsoft.XApp.Components.PropertyEditors
{
    /// <summary>
    /// A basic date time property editor
    /// </summary>
    public class DateTimePropertyEditor : BasePropertyEditor<DateTime>
    {
        public DateTimePropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {

        }

        protected override BasePropertyEditor<DateTime> Create(string id, bool isReadonly)
        {
            var clone = new DateTimePropertyEditor(id, isReadonly);

            return clone;
        }
    }
}
