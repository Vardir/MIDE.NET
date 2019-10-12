using System;
using System.Reflection;
using System.ComponentModel;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Converters;

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

        private bool suppressHandler;
        private INotifyPropertyChanged source;
        private INotifyPropertyChanged destination;
        private PropertyInfo sourceProperty;
        private PropertyInfo destinationProperty;

        /// <summary>
        /// The first one element in binding that is usually interpreted as the source
        /// </summary>
        public INotifyPropertyChanged Source
        {
            get => source;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (source.HasValue())
                {    
                    source.PropertyChanged -= Source_PropertyChanged;
                }

                source = value;
                source.PropertyChanged += Source_PropertyChanged;
            }
        }
        /// <summary>
        /// The second one element in binding that is usually interpreted as the destination
        /// </summary>
        public INotifyPropertyChanged Destination
        {
            get => destination;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (destination != null)
                {    
                    destination.PropertyChanged -= Destination_PropertyChanged;
                }

                destination = value;
                destination.PropertyChanged += Destination_PropertyChanged;
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
            sourceProperty = source.GetType().GetProperty(sourceProp);
            destinationProperty = destination.GetType().GetProperty(destinationProp);

            if (sourceProperty == null)
                throw new ArgumentException($"Type [{source.GetType()}]  does not have declaration for property '{sourceProp}' or it is inaccessible");
            
            if (destinationProperty == null)
                throw new ArgumentException($"Type [{destination.GetType()}] does not have declaration for property '{destinationProp}' or it is inaccessible");
        }

        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (suppressHandler)
                    return;

            if (e.PropertyName == sourceProperty.Name)
            {
                suppressHandler = true;
                switch (BindingKind)
                {
                    case BindingKind.TwoWay:
                    case BindingKind.OneWay:
                        var value = Converter.Convert(sourceProperty.GetValue(source));
                        destinationProperty.SetValue(destination, value);
                        break;
                }

                suppressHandler = false;
            }
        }
        private void Destination_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (suppressHandler)
                return;

            if (e.PropertyName == destinationProperty.Name)
            {
                suppressHandler = true;
                switch (BindingKind)
                {
                    case BindingKind.TwoWay:
                    case BindingKind.OneWayToSource:
                        var value = Converter.ConvertBack(destinationProperty.GetValue(destination));
                        sourceProperty.SetValue(source, value);
                        break;
                }
                
                suppressHandler = false;
            }
        }
    }
}