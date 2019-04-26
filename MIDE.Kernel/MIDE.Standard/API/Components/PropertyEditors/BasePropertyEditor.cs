using System;
using System.Reflection;
using MIDE.API.Validations;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MIDE.API.Components.PropertyEditors
{
    /// <summary>
    /// A base class to create a single object editor that can be attached to an object's property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BasePropertyEditor<T> : LayoutComponent
    {
        private bool hasErrors;
        private bool isReadonly;
        private string propName;
        private object attachedObject;
        private PropertyInfo property;
        private readonly Type expectedValueType = typeof(T);
        private INotifyPropertyChanged attachedNotifyObject;

        public bool HasErrors
        {
            get => hasErrors;
            private set
            {
                if (value == hasErrors)
                    return;
                hasErrors = value;
                OnPropertyChanged(nameof(HasErrors));
            }
        }
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
                HasErrors = false;
                ValidationErrors.Clear();
                Validations.ForEach(v => v.Validate(value, nameof(Value), ValidationErrors));
                if (ValidationErrors.Count != 0)
                {
                    HasErrors = true;
                    return;
                }
                property.SetValue(attachedObject, value);
            }
        }
        public List<ValueValidation<T>> Validations { get; }
        public ObservableCollection<ValidationError> ValidationErrors { get; }

        public BasePropertyEditor(string id, bool isReadonly = false) : base(id)
        {
            IsReadonly = isReadonly;
            Validations = new List<ValueValidation<T>>();
            ValidationErrors = new ObservableCollection<ValidationError>();
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
            property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property == null)
                throw new ArgumentException($"Can not find public property '{propertyName}' on type [{type}] with accessible get method");
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

        protected override LayoutComponent CloneInternal(string id)
        {
            BasePropertyEditor<T> clone = Create(id, isReadonly);
            clone.Validations.AddRange(Validations);
            return clone;
        }

        protected abstract BasePropertyEditor<T> Create(string id, bool isReadonly);

        private void AttachedNotifyObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
                OnPropertyChanged(nameof(Value));
        }
    }
}