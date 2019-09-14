using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using XApp.API.Validations;

namespace XApp.Components.PropertyEditors
{
    public abstract class BasePropertyEditor : LayoutComponent
    {
        private bool hasErrors;
        private string propName;

        public bool HasErrors
        {
            get => hasErrors;
            protected set => SetAndNotify(value, ref hasErrors);
        }
        public string PropertyName
        {
            get => propName;
            set => SetAndNotify(value, ref propName);
        }

        public BasePropertyEditor(string id) : base(id)
        {

        }

        public abstract void AttachTo(object obj, string propertyName);
        public abstract void AttachTo(INotifyPropertyChanged obj, string propertyName);
    }

    /// <summary>
    /// A base class to create a single object editor that can be attached to an object's property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BasePropertyEditor<T> : BasePropertyEditor
    {
        private bool isReadonly;
        private object attachedObject;
        private PropertyInfo property;
        private readonly Type expectedValueType = typeof(T);
        private INotifyPropertyChanged attachedNotifyObject;

        /// <summary>
        /// Indicates that editor does not support changing property value
        /// </summary>
        public bool IsReadonly
        {
            get => isReadonly;
            set => SetAndNotify(value, ref isReadonly);
        }
        /// <summary>
        /// Gets or sets value of attached object's property
        /// </summary>
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
        /// <summary>
        /// List of validations that are being applied before setting property's value
        /// </summary>
        public List<ValueValidation<T>> Validations { get; }
        /// <summary>
        /// A collection of validation errors that occurred on last value setting
        /// </summary>
        public ObservableCollection<ValidationError> ValidationErrors { get; }

        public BasePropertyEditor(string id, bool isReadonly = false) : base(id)
        {
            IsReadonly = isReadonly;
            Validations = new List<ValueValidation<T>>();
            ValidationErrors = new ObservableCollection<ValidationError>();
        }

        /// <summary>
        /// Attaches editor to any king of object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        public override void AttachTo(object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
                
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            attachedObject = obj;
            attachedNotifyObject = null;
            PropertyName = propertyName;
            
            var type = attachedObject.GetType();
            property = type.GetProperty(propertyName);

            if (property == null)
                throw new ArgumentException($"Can not find public property '{propertyName}' on type [{type}] with accessible get method");

            if (!property.CanRead || !property.GetMethod.IsPublic)
                throw new ArgumentException($"Can not attach editor to property without public 'get' method");

            if (!property.PropertyType.Equals(expectedValueType))
                throw new ArgumentException($"Can not attach editor of type [{expectedValueType}] to property of type [{property.PropertyType}]");

            if (!property.CanWrite || !property.SetMethod.IsPublic)
                IsReadonly = true;
        }
        /// <summary>
        /// Attaches editor to any object that implements <seealso cref="INotifyPropertyChanged"/> interface
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        public override void AttachTo(INotifyPropertyChanged obj, string propertyName)
        {
            AttachTo(obj, propertyName);
            attachedNotifyObject = obj;            
            attachedNotifyObject.PropertyChanged += AttachedNotifyObject_PropertyChanged;
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = Create(id, isReadonly);
            clone.Validations.AddRange(Validations);

            return clone;
        }

        protected abstract BasePropertyEditor<T> Create(string id, bool isReadonly);
        
        private void AttachedNotifyObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
            {
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}