using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

using XApp.Helpers;

namespace XApp.API.Validations
{
    public abstract class PropertyAttachedValidation<T> : Validation, INotifyPropertyChanged
    {
        protected const BindingFlags PROP_FLAGS = BindingFlags.GetProperty | BindingFlags.SetProperty | 
                                                  BindingFlags.Public | BindingFlags.Instance;

        private IValidate attachedObject;
        private List<PropertyInfo> supportedProps;
        private readonly Type supportedPropertyType;
        private readonly List<ValidationError> validationMessages;

        public override bool HasErrors => validationMessages.Count > 0;
        public bool RaiseExceptionOnError { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public PropertyAttachedValidation(bool raiseExceptionOnError)
        {
            validationMessages = new List<ValidationError>();
            RaiseExceptionOnError = raiseExceptionOnError;
            supportedPropertyType = typeof(T);
        }

        /// <summary>
        /// Attaches validation to an object. All properties of the required type will be validated on change
        /// <para>If property names to observe are supplied, only those will be validated</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AttachTo(IValidate obj, params string[] propertiesToObserve)
        {
            if (attachedObject != null)
            {
                attachedObject.Validations.Remove(this);
                attachedObject.PropertyChanged -= Obj_PropertyChanging;
            }

            attachedObject = obj ?? throw new ArgumentNullException(nameof(obj));
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
            if (propertyName == null)
                return validationMessages;

            return validationMessages.Where(v => v.PropertyName == propertyName);
        }
        public IEnumerable<ValidationError> GetAllErrors(string propertyName = null)
        {
            if (propertyName == null)
                return validationMessages;

            return validationMessages.Where(v => v.PropertyName == propertyName);
        }
        
        protected abstract void Validate(string property, T value);

        protected void AddError(string property, string message, object value)
        {
            validationMessages.Add(new ValidationError(property, message, value));
            attachedObject.NotifyError();

            OnPropertyChanged(nameof(HasErrors));
        }
        protected void ClearErrors()
        {
            validationMessages.Clear();
            attachedObject.NotifyError();

            OnPropertyChanged(nameof(HasErrors));
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void InvalidateAll()
        {
            foreach (var prop in supportedProps)
            {
                Obj_PropertyChanging(attachedObject, new PropertyChangedEventArgs(prop.Name));
            }
        }
        private void LoadSupportedProperties(Type type, string[] propsToObserve)
        {
            var selected = type.GetProperties(PROP_FLAGS).Where(p => p.PropertyType == supportedPropertyType);
            supportedProps = new List<PropertyInfo>(selected);

            if (propsToObserve != null && propsToObserve.Length != 0)
            {
                for (int i = 0; i < supportedProps.Count; i++)
                {
                    if (!propsToObserve.Contains(supportedProps[i].Name))
                    {    
                        supportedProps.RemoveAt(i--);
                    }
                }

                if (supportedProps.AnyNotIn(p => p.Name, propsToObserve))
                    throw new ArgumentException($"Type [{type}] contains not all the properties that were asked");
            }

            if (supportedProps.Count == 0)
                throw new ArgumentException($"Type [{type}] has no available properties of type [{typeof(T)}] to validate");
        }
        private void Obj_PropertyChanging(object sender, PropertyChangedEventArgs e)
        {
            var property = supportedProps.Find(p => p.Name == e.PropertyName);
            if (property == null)
                return;

            Validate(e.PropertyName, (T)property.GetValue(attachedObject));

            if (validationMessages.Count > 0)
            {
                if (RaiseExceptionOnError)
                    throw new FormatException($"Property [{e.PropertyName}]: Value has invalid format");
                    
                InvokeErrorsChanged(new DataErrorsChangedEventArgs(e.PropertyName));
            }
        }
    }
}