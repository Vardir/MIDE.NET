using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

using Vardirsoft.Shared.Helpers;
using Vardirsoft.XApp.Helpers;

namespace Vardirsoft.XApp.API.Validations
{
    public abstract class PropertyAttachedValidation<T> : Validation, INotifyPropertyChanged
    {
        protected const BindingFlags PROP_FLAGS = BindingFlags.GetProperty | BindingFlags.SetProperty | 
                                                  BindingFlags.Public | BindingFlags.Instance;

        private IValidate _attachedObject;
        private List<PropertyInfo> _supportedProps;
        private readonly Type _supportedPropertyType;
        private readonly List<ValidationError> _validationMessages;

        public override bool HasErrors => _validationMessages.Count > 0;
        public bool RaiseExceptionOnError { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public PropertyAttachedValidation(bool raiseExceptionOnError)
        {
            _validationMessages = new List<ValidationError>();
            RaiseExceptionOnError = raiseExceptionOnError;
            _supportedPropertyType = typeof(T);
        }

        /// <summary>
        /// Attaches validation to an object. All properties of the required type will be validated on change
        /// <para>If property names to observe are supplied, only those will be validated</para>
        /// </summary>
        public void AttachTo(IValidate obj, params string[] propertiesToObserve)
        {
            if (_attachedObject.HasValue())
            {
                _attachedObject.Validations.Remove(this);
                _attachedObject.PropertyChanged -= Obj_PropertyChanging;
            }
            
            Guard.EnsureNotNull(obj, typeof(ArgumentNullException));

            _attachedObject = obj;
            LoadSupportedProperties(obj.GetType(), propertiesToObserve);
            obj.PropertyChanged += Obj_PropertyChanging;
            obj.Validations.Add(this);

            InvalidateAll();
        }

        /// <summary>
        /// Receive all the validation errors for the current validation instance.
        /// <para>If property name supplied, returns errors only for the specified property</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public override IEnumerable GetErrors(string propertyName)
        {
            if (propertyName is null)
                return _validationMessages;

            return _validationMessages.Where(v => v.PropertyName == propertyName);
        }
        public IEnumerable<ValidationError> GetAllErrors(string propertyName = null)
        {
            if (propertyName is null)
                return _validationMessages;

            return _validationMessages.Where(v => v.PropertyName == propertyName);
        }
        
        protected abstract void Validate(string property, T value);

        protected void AddError(string property, string message, object value)
        {
            _validationMessages.Add(new ValidationError(property, message, value));
            _attachedObject.NotifyError();

            OnPropertyChanged(nameof(HasErrors));
        }
        protected void ClearErrors()
        {
            _validationMessages.Clear();
            _attachedObject.NotifyError();

            OnPropertyChanged(nameof(HasErrors));
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void InvalidateAll()
        {
            foreach (var prop in _supportedProps)
            {
                Obj_PropertyChanging(_attachedObject, new PropertyChangedEventArgs(prop.Name));
            }
        }
        private void LoadSupportedProperties(Type type, string[] propsToObserve)
        {
            var selected = type.GetProperties(PROP_FLAGS).Where(p => p.PropertyType == _supportedPropertyType);
            _supportedProps = new List<PropertyInfo>(selected);

            if (propsToObserve.HasValue() && propsToObserve.Length != 0)
            {
                for (var i = 0; i < _supportedProps.Count; i++)
                {
                    if (!propsToObserve.Contains(_supportedProps[i].Name))
                    {    
                        _supportedProps.RemoveAt(i--);
                    }
                }

                Guard.Ensure(_supportedProps.AnyNotIn(p => p.Name, propsToObserve), $"Type [{type}] contains not all the properties that were asked");
            }

            Guard.EnsureNonEmpty(_supportedProps, $"Type [{type}] has no available properties of type [{typeof(T)}] to validate");
        }
        private void Obj_PropertyChanging(object sender, PropertyChangedEventArgs e)
        {
            var property = _supportedProps.Find(p => p.Name == e.PropertyName);
            if (property is null)
                return;

            Validate(e.PropertyName, (T)property.GetValue(_attachedObject));

            if (_validationMessages.Count > 0)
            {
                Guard.EnsureNot(RaiseExceptionOnError, typeof(FormatException), $"Property [{e.PropertyName}]: Value has invalid format");
                    
                InvokeErrorsChanged(new DataErrorsChangedEventArgs(e.PropertyName));
            }
        }
    }
}