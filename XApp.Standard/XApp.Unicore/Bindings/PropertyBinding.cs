using System;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Converters;
using Vardirsoft.XApp.Helpers;

namespace Vardirsoft.XApp.Bindings
{
    /// <summary>
    /// A simple binding class that is used to connect two objects via property binding.
    /// Use this binding if type checking is not necessary
    /// </summary>
    public class PropertyBinding
    {
        protected const BindingFlags GET_PROP_FLAGS = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;
        protected const BindingFlags SET_PROP_FLAGS = BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public;
        protected const BindingFlags PROP_FLAGS = BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public;

        private bool _suppressHandler;
        private INotifyPropertyChanged _source;
        private INotifyPropertyChanged _destination;
        private PropertyInfo _sourceProperty;
        private PropertyInfo _destinationProperty;

        /// <summary>
        /// The first one element in binding that is usually interpreted as the source
        /// </summary>
        public INotifyPropertyChanged Source
        {
            [DebuggerStepThrough]
            get => _source;
            set
            {
                Guard.EnsureNotNull(value, typeof(ArgumentNullException));

                if (_source.HasValue())
                {    
                    _source.PropertyChanged -= Source_PropertyChanged;
                }

                _source = value;
                _source.PropertyChanged += Source_PropertyChanged;
            }
        }
        
        /// <summary>
        /// The second one element in binding that is usually interpreted as the destination
        /// </summary>
        public INotifyPropertyChanged Destination
        {
            [DebuggerStepThrough]
            get => _destination;
            set
            {
                Guard.EnsureNotNull(value, typeof(ArgumentNullException));

                if (_destination.HasValue())
                {    
                    _destination.PropertyChanged -= Destination_PropertyChanged;
                }

                _destination = value;
                _destination.PropertyChanged += Destination_PropertyChanged;
            }
        }
        public BindingKind BindingKind { get; set; }
        /// <summary>
        /// Converter that is used to convert value in both directions
        /// </summary>
        public IValueConverter Converter { get; set; }

        public PropertyBinding()
        {
            BindingKind = BindingKind.TwoWay;
            Converter = new DefaultConverter();
        }

        /// <summary>
        /// Sets up binding between two objects on the given properties
        /// </summary>
        /// <param name="source">The object that goes as the first one in the binding</param>
        /// <param name="sourceProp">The property of the source object to listen to changing event of</param>
        /// <param name="destination">The object that goes as the second one in the binding</param>
        /// <param name="destinationProp">The property of the destination object to listen to changing event of</param>
        public void Bind(INotifyPropertyChanged source, string sourceProp, 
                         INotifyPropertyChanged destination, string destinationProp)
        {
            Source = source;
            Destination = destination;
            Bind(sourceProp, destinationProp);
        }
        /// <summary>
        /// Change properties that are used in binding
        /// </summary>
        /// <param name="sourceProp">The property of the source object to listen to changing event of</param>
        /// <param name="destinationProp">The property of the destination object to listen to changing event of</param>
        public void Bind(string sourceProp, string destinationProp)
        {
            _sourceProperty = _source.GetType().GetProperty(sourceProp);
            _destinationProperty = _destination.GetType().GetProperty(destinationProp);

            Guard.EnsureNotNull(_sourceProperty, typeof(ArgumentException), $"Type [{_source.GetType()}] does not have declaration for property '{sourceProp}' or it is inaccessible");
            Guard.EnsureNotNull(_sourceProperty, typeof(ArgumentException), $"Type [{_destination.GetType()}] does not have declaration for property '{destinationProp}' or it is inaccessible");
        }

        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_suppressHandler)
                    return;

            if (e.PropertyName == _sourceProperty.Name)
            {
                _suppressHandler = true;
                switch (BindingKind)
                {
                    case BindingKind.TwoWay:
                    case BindingKind.OneWay:
                        var value = Converter.Convert(_sourceProperty.GetValue(_source));
                        _destinationProperty.SetValue(_destination, value);
                        break;
                }

                _suppressHandler = false;
            }
        }
        private void Destination_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_suppressHandler)
                return;

            if (e.PropertyName == _destinationProperty.Name)
            {
                _suppressHandler = true;
                switch (BindingKind)
                {
                    case BindingKind.TwoWay:
                    case BindingKind.OneWayToSource:
                        var value = Converter.ConvertBack(_destinationProperty.GetValue(_destination));
                        _sourceProperty.SetValue(_source, value);
                        break;
                }
                
                _suppressHandler = false;
            }
        }
    }
}