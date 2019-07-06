using System;

namespace MIDE.Components.PropertyEditors
{
    /// <summary>
    /// A basic enum property editor
    /// </summary>
    public abstract class EnumPropertyEditor<TEnum> : BasePropertyEditor<TEnum>
        where TEnum : Enum
    {
        public TEnum[] EnumFields { get; }

        public EnumPropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {
            EnumFields = GetValues();
        }

        protected abstract TEnum[] GetValues();
    }
}
