using System;

namespace MIDE.API.Components.PropertyEditors
{
    /// <summary>
    /// Test console color property editor
    /// </summary>
    public sealed class ConsoleColorPropertyEditor : EnumPropertyEditor<ConsoleColor>
    {
        public ConsoleColorPropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {

        }

        protected override Array GetValues() => Enum.GetValues(typeof(ConsoleColor));
        protected override BasePropertyEditor<ConsoleColor> Create(string id, bool isReadonly)
        {
            ConsoleColorPropertyEditor clone = new ConsoleColorPropertyEditor(id, isReadonly);
            return clone;
        }
    }
}
