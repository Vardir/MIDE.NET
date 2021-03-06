﻿using System;

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

        protected override ConsoleColor[] GetValues()
        {
            Array array = Enum.GetValues(typeof(ConsoleColor));
            ConsoleColor[] result = new ConsoleColor[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = (ConsoleColor)array.GetValue(i);
            }
            return result;
        }
        protected override BasePropertyEditor<ConsoleColor> Create(string id, bool isReadonly)
        {
            ConsoleColorPropertyEditor clone = new ConsoleColorPropertyEditor(id, isReadonly);
            return clone;
        }
    }
}
