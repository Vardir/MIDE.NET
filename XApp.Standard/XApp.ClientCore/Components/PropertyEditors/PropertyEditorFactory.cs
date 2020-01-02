using System;
using System.ComponentModel;
using System.Collections.Generic;

using Vardirsoft.Shared.Helpers;

namespace Vardirsoft.XApp.Components.PropertyEditors
{
    public static class PropertyEditorFactory
    {
        public static Dictionary<string, Func<string, BasePropertyEditor>> Editors { get; }

        static PropertyEditorFactory()
        {
            Editors = new Dictionary<string, Func<string, BasePropertyEditor>> 
            {
                ["Boolean"] = name => new BoolPropertyEditor(name),
                ["Byte"] = name => new BytePropertyEditor(name),
                ["Int16"] = name => new Int16PropertyEditor(name),
                ["Int32"] = name => new Int32PropertyEditor(name),
                ["Int64"] = name => new Int64PropertyEditor(name),
                ["Single"] = name => new FloatPropertyEditor(name),
                ["Double"] = name => new DoublePropertyEditor(name),
                ["ConsoleColor"] = name => new ConsoleColorPropertyEditor(name),
                ["DateTime"] = name => new DateTimePropertyEditor(name),
                ["Object"] = name => new ObjectPropertyEditor(name),
                ["String"] = name => new StringPropertyEditor(name),
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
            var type = typeof(T);
            var editors = new List<BasePropertyEditor>();
            var notifyPropertyChanged = obj as INotifyPropertyChanged;
            var propertyInfos = type.GetProperties();

            foreach (var property in propertyInfos)
            {
                if (property.CanRead || property.GetMethod.IsPublic)
                {
                    var editor = GetEditorFor(property.PropertyType, property.Name);
                    if (notifyPropertyChanged.HasValue())
                    { 
                        editor.AttachTo(notifyPropertyChanged, property.Name);
                    }
                    else
                    { 
                        editor.AttachTo(obj, property.Name);
                    }
                    
                    editors.Add(editor);
                }
            }
            
            return editors.ToArray();
        }

        private static BasePropertyEditor GetEditorFor(Type propertyType, string propertyName)
        {
            var editorName = $"{ApplicationComponent.ToSafeId(propertyName)}-editor";
            BasePropertyEditor editor = null;

            if (Editors.TryGetValue(propertyType.Name, out var func))
            {
                editor = func(editorName);
            }

            return editor;
        }
    }
}