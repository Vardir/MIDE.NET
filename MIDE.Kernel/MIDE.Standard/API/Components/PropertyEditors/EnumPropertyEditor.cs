using System;

namespace MIDE.API.Components.PropertyEditors
{
    /// <summary>
    /// A basic enum property editor
    /// </summary>
    public abstract class EnumPropertyEditor<TEnum> : BasePropertyEditor<TEnum>
        where TEnum : Enum
    {
        public Array EnumFields { get; }

        public EnumPropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {
            EnumFields = GetValues();
        }

        protected abstract Array GetValues();
    }
}
