using System;
using System.Reflection;
using System.ComponentModel;

namespace MIDE.API.Components.PropertyEditors
{
    /// <summary>
    /// A base class to create a single object editor that can be attached to an object's property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BasePropertyEditor<T> : LayoutComponent
    {
        private bool isReadonly;
        private string propName;
        private object attachedObject;
        private PropertyInfo property;
        private readonly Type expectedValueType = typeof(T);
        private INotifyPropertyChanged attachedNotifyObject;

        public bool IsReadonly
        {
            get => isReadonly;
            set
            {
                if (value == isReadonly)
                    return;
                isReadonly = value;
                OnPropertyChanged(nameof(IsReadonly));
            }
        }
        public string PropertyName
        {
            get => propName;
            set
            {
                if (value == propName)
                    return;
                propName = value;
                OnPropertyChanged(nameof(PropertyName));
            }
        }
        public virtual T Value
        {
            get => (T)property.GetValue(attachedObject);
            set
            {
                if (IsReadonly)
                    return;
                property.SetValue(attachedObject, value);
            }
        }

        public BasePropertyEditor(string id, bool isReadonly = false) : base(id)
        {
            IsReadonly = isReadonly;
        }

        public void AttachTo(object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            attachedObject = obj;
            PropertyName = propertyName;
            attachedNotifyObject = null;
            Type type = attachedObject.GetType();
            property = type.GetProperty(propertyName, BindingFlags.GetProperty | BindingFlags.Public);
            if (property == null)
                throw new ArgumentException($"Can not find public property '{property}' on type [{type}] with accessible get method");
            if (!property.PropertyType.Equals(expectedValueType))
                throw new ArgumentException($"Can not attach editor of type [{expectedValueType}] to property of type [{property.PropertyType}]");
            if (property.SetMethod == null || !property.SetMethod.IsPublic)
                IsReadonly = true;
        }
        public void AttachTo(INotifyPropertyChanged obj, string propertyName)
        {
            //TODO: requires refactoring with previous method
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            attachedObject = obj;
            attachedNotifyObject = obj;
            PropertyName = propertyName;
            Type type = attachedObject.GetType();
            property = type.GetProperty(propertyName, BindingFlags.GetProperty | BindingFlags.Public);
            if (property == null)
                throw new ArgumentException($"Can not find public property '{property}' on type [{type}] with accessible get method");
            if (!property.PropertyType.Equals(expectedValueType))
                throw new ArgumentException($"Can not attach editor of type [{expectedValueType}] to property of type [{property.PropertyType}]");
            if (property.SetMethod == null || !property.SetMethod.IsPublic)
                IsReadonly = true;
            attachedNotifyObject.PropertyChanged += AttachedNotifyObject_PropertyChanged;
        }

        private void AttachedNotifyObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
                OnPropertyChanged(nameof(Value));
        }
    }

    /// <summary>
    /// A basic int32 property editor
    /// </summary>
    public class Int32PropertyEditor : BasePropertyEditor<int>
    {
        public Int32PropertyEditor(string id, bool isReadonly = false) : base(id, isReadonly)
        {

        }

        protected override LayoutComponent CloneInternal(string id)
        {
            throw new NotImplementedException();
        }
    }
}
