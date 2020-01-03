using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Vardirsoft.XApp.API.Validations;

namespace Vardirsoft.XApp.Components.PropertyEditors
{
    public abstract class BasePropertyEditor : LayoutComponent
    {
        private bool _hasErrors;
        private string _propName;

        public bool HasErrors
        {
            [DebuggerStepThrough]
            get => _hasErrors;
            
            [DebuggerStepThrough]
            protected set => SetWithNotify(ref _hasErrors, value);
        }
        
        public string PropertyName
        {
            [DebuggerStepThrough]
            get => _propName;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _propName, value, true);
        }

        public BasePropertyEditor(string id) : base(id)
        {

        }

        public abstract void AttachTo(object obj, string propertyName);
        public abstract void AttachTo(INotifyPropertyChanged notifyPropertyChanged, string propertyName);
    }

    /// <summary>
    /// A base class to create a single object editor that can be attached to an object's property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BasePropertyEditor<T> : BasePropertyEditor
    {
        private bool _isReadonly;
        private object _attachedObject;
        private PropertyInfo _property;
        private readonly Type _expectedValueType = typeof(T);
        private INotifyPropertyChanged _attachedNotifyObject;

        /// <summary>
        /// Indicates that editor does not support changing property value
        /// </summary>
        public bool IsReadonly
        {
            [DebuggerStepThrough]
            get => _isReadonly;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _isReadonly, value);
        }
        
        /// <summary>
        /// Gets or sets value of attached object's property
        /// </summary>
        public virtual T Value
        {
            [DebuggerStepThrough]
            get => (T)_property.GetValue(_attachedObject);
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

                _property.SetValue(_attachedObject, value);
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
            _attachedNotifyObject = null;
            _attachedObject = obj ?? throw new ArgumentNullException(nameof(obj));
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            
            var type = _attachedObject.GetType();
            _property = type.GetProperty(propertyName);

            if (_property is null)
                throw new ArgumentException($"Can not find public property '{propertyName}' on type [{type}] with accessible get method");

            if (!_property.CanRead || !_property.GetMethod.IsPublic)
                throw new ArgumentException($"Can not attach editor to property without public 'get' method");

            if (_property.PropertyType != _expectedValueType)
                throw new ArgumentException($"Can not attach editor of type [{_expectedValueType}] to property of type [{_property.PropertyType}]");

            if (!_property.CanWrite || !_property.SetMethod.IsPublic)
                IsReadonly = true;
        }
        
        /// <summary>
        /// Attaches editor to any object that implements <seealso cref="INotifyPropertyChanged"/> interface
        /// </summary>
        /// <param name="notifyPropertyChanged"></param>
        /// <param name="propertyName"></param>
        public override void AttachTo(INotifyPropertyChanged notifyPropertyChanged, string propertyName)
        {
            AttachTo((object)notifyPropertyChanged, propertyName);
            _attachedNotifyObject = notifyPropertyChanged;            
            _attachedNotifyObject.PropertyChanged += AttachedNotifyObject_PropertyChanged;
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = Create(id, _isReadonly);
            clone.Validations.AddRange(Validations);

            return clone;
        }

        protected abstract BasePropertyEditor<T> Create(string id, bool isReadonly);
        
        private void AttachedNotifyObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
            {
                NotifyPropertyChanged(nameof(Value));
            }
        }
    }
}