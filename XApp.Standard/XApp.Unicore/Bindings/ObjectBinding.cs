using System;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Converters;

namespace Vardirsoft.XApp.Bindings
{
    /// <summary>
    /// A generic-typed binding class that is used to connect two objects via property binding.
    /// Use this binding if type checking is necessary
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    //TODO: review naming
    public class ObjectBinding<TSource, TDestination>
        where TSource : INotifyPropertyChanged
        where TDestination : INotifyPropertyChanged
    {
        /// <summary>
        /// A flag to prevent indirect recursive calls of property changed event handlers
        /// </summary>
        private bool _suppressHandler;
        private TSource _source;
        private TDestination _destination;
        private readonly Type _sourceType;
        private readonly Type _destinationType;
        private PropertyInfo _sourceProperty;
        private PropertyInfo _destinationProperty;
        private IValueConverter _converter;

        /// <summary>
        /// The first one element in binding that is usually interpreted as the source
        /// </summary>
        public TSource Source
        {
            [DebuggerStepThrough]
            get => _source;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));

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
        public TDestination Destination
        {
            [DebuggerStepThrough]
            get => _destination;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                if (_destination.HasValue())
                {   
                    _destination.PropertyChanged -= Destination_PropertyChanged;
                }

                _destination = value;
                _destination.PropertyChanged += Destination_PropertyChanged;
            }
        }
        
        public BindingKind BindingKind { get; set; }

        public ObjectBinding()
        {
            BindingKind = BindingKind.TwoWay;
            _sourceType = typeof(TSource);
            _destinationType = typeof(TDestination);
        }
        public ObjectBinding(TSource source, TDestination destination) : this()
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            Source = source;
            Destination = destination;            
        }

        /// <summary>
        /// Sets up the property binding for the objects this instance is attached to
        /// </summary>
        /// <typeparam name="T">Source property type</typeparam>
        /// <typeparam name="Y">Destination property type</typeparam>
        /// <param name="sourceExpr">Expression to extract a property from the source</param>
        /// <param name="destinationExpr">Expression to extract a property from the destination</param>
        /// <param name="convertFunc">Converter function that is used when source value is being passed to destination</param>
        /// <param name="convertBackFunc">Converter function that is used when destination value is being passed to source</param>
        /// <param name="defaults"></param>
        public void Bind<T, Y>(Expression<Func<TSource, T>> sourceExpr, Expression<Func<TDestination, Y>> destinationExpr,
                               Func<T, Y> convertFunc, Func<Y, T> convertBackFunc, Defaults<T,Y> defaults = default)
        {
            _sourceProperty = _sourceType.GetProperty(GetMember(sourceExpr.Body));
            _destinationProperty = _destinationType.GetProperty(GetMember(destinationExpr.Body));

            if (convertFunc.HasValue() && convertBackFunc.HasValue())
            {    
                _converter = new ValueConverter<T, Y>(convertFunc, convertBackFunc);
            }
            else if (convertFunc is null)
            {    
                _converter = new ValueConverter<T, Y>(convertBackFunc);
            }
            else if (convertBackFunc is null)
            {    
                _converter = new ValueConverter<T, Y>(convertFunc);
            }
            else
            {    
                throw new NullReferenceException("At least one of the converters must be set");
            }

            if (defaults.SourceIsSet && _sourceProperty.CanWrite)
            {    
                _sourceProperty.SetValue(_source, defaults.Source);
            }
            if (defaults.DestinationIsSet && _destinationProperty.CanWrite)
            {  
                _destinationProperty.SetValue(_destination, defaults.Destination);
            }
        }

        /// <summary>
        /// Sets up the property binding for the objects this instance is attached to
        /// </summary>
        /// <typeparam name="T">Source property type</typeparam>
        /// <typeparam name="Y">Destination property type</typeparam>
        /// <param name="sourceExpr">Expression to extract a property from the source</param>
        /// <param name="destinationExpr">Expression to extract a property from the destination</param>
        /// <param name="converter">The converter that is used when value is being passed to source/destination property</param>
        /// <param name="defaults"></param>
        public void Bind<T, Y>(Expression<Func<TSource, T>> sourceExpr, Expression<Func<TDestination, Y>> destinationExpr, 
                               ValueConverter<T, Y> converter, Defaults<T,Y> defaults = default)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _sourceProperty = _sourceType.GetProperty(GetMember(sourceExpr.Body));
            _destinationProperty = _destinationType.GetProperty(GetMember(destinationExpr.Body));

            if (defaults.SourceIsSet && _sourceProperty.CanWrite)
            {    
                _sourceProperty.SetValue(_source, defaults.Source);
            }

            if (defaults.DestinationIsSet && _destinationProperty.CanWrite)
            {    
                _destinationProperty.SetValue(_destination, defaults.Destination);
            }
        }

        private string GetMember(Expression expr)
        {
            if (expr.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("Expected a member access expression");

            var member = (expr as MemberExpression)?.Member;

            return member?.Name;
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
                    case BindingKind.OneWay:
                    case BindingKind.TwoWay:
                        var value = _converter.Convert(_sourceProperty.GetValue(_source));
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
                    case BindingKind.OneWayToSource:
                    case BindingKind.TwoWay:
                        var value = _converter.ConvertBack(_destinationProperty.GetValue(_destination));
                        _sourceProperty.SetValue(_source, value);
                        break;
                }

                _suppressHandler = false;
            }
        }
    }

    public struct Defaults<T, Y>
    {
        public readonly bool SourceIsSet;
        public readonly bool DestinationIsSet;
        public readonly T Source;
        public readonly Y Destination;

        public Defaults(T source)
        {
            Source = source;
            Destination = default;
            SourceIsSet = true;
            DestinationIsSet = false;
        }
        public Defaults(Y destination)
        {
            Destination = destination;
            Source = default;
            SourceIsSet = false;
            DestinationIsSet = true;
        }
        public Defaults(T source, Y destination)
        {
            Source = source;
            Destination = destination;
            SourceIsSet = true;
            DestinationIsSet = true;
        }
    }
}