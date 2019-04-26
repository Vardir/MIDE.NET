using System;

namespace MIDE.API.Components.PropertyEditors
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
            DateTimePropertyEditor clone = new DateTimePropertyEditor(id, isReadonly);
            return clone;
        }
    }
}
