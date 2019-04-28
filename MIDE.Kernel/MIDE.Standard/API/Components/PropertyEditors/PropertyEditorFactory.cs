using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

namespace MIDE.API.Components.PropertyEditors
{
    public static class PropertyEditorFactory
    {
        public static Dictionary<string, Func<string, BasePropertyEditor>> Editors { get; }

        static PropertyEditorFactory()
        {
            Editors = new Dictionary<string, Func<string, BasePropertyEditor>>()
            {
                ["Boolean"] = (name) => new BoolPropertyEditor(name),
                ["Byte"] = (name) => new BytePropertyEditor(name),
                ["Int16"] = (name) => new Int16PropertyEditor(name),
                ["Int32"] = (name) => new Int32PropertyEditor(name),
                ["Int64"] = (name) => new Int64PropertyEditor(name),
                ["Single"] = (name) => new FloatPropertyEditor(name),
                ["Double"] = (name) => new DoublePropertyEditor(name),
                ["ConsoleColor"] = (name) => new ConsoleColorPropertyEditor(name),
                ["DateTime"] = (name) => new DateTimePropertyEditor(name),
                ["Object"] = (name) => new ObjectPropertyEditor(name),
                ["String"] = (name) => new StringPropertyEditor(name),
            };
        }

        /// <summary>
        /// Inspects object's properties and generates editor for each known type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static BasePropertyEditor[] GetEditorsFor<T>(T obj)
        {
            Type type = typeof(T);
            List<BasePropertyEditor> editors = new List<BasePropertyEditor>();
            INotifyPropertyChanged notifyPropertyChanged = obj as INotifyPropertyChanged;
            PropertyInfo[] propertyInfos = type.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo property = propertyInfos[i];
                if (!property.CanRead || !property.GetMethod.IsPublic)
                    continue;
                BasePropertyEditor editor = GetEditorFor(property.PropertyType, property.Name);
                if (notifyPropertyChanged != null)
                    editor.AttachTo(notifyPropertyChanged, property.Name);
                else
                    editor.AttachTo(obj, property.Name);
                editors.Add(editor);
            }
            return editors.ToArray();
        }

        private static BasePropertyEditor GetEditorFor(Type propertyType, string propertyName)
        {
            string editorName = $"{ApplicationComponent.ToSafeId(propertyName)}-editor";
            BasePropertyEditor editor = null;
            if (Editors.TryGetValue(propertyType.Name, out var func))
                editor = func(editorName);
            return editor;
        }
    }
}