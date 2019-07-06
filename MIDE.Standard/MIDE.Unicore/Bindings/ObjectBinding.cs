using System;
using MIDE.API;
using MIDE.Converters;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MIDE.Bindings
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
        private bool suppressHandler;
        private TSource source;
        private TDestination destination;
        private readonly Type sourceType;
        private readonly Type destinationType;
        private PropertyInfo sourceProperty;
        private PropertyInfo destinationProperty;
        private IValueConverter converter;

        /// <summary>
        /// The first one element in binding that is usually interpreted as the source
        /// </summary>
        public TSource Source
        {
            get => source;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (source != null)
                    source.PropertyChanged -= Source_PropertyChanged;
                source = value;
                source.PropertyChanged += Source_PropertyChanged;
            }
        }
        /// <summary>
        /// The second one element in binding that is usually interpreted as the destination
        /// </summary>
        public TDestination Destination
        {
            get => destination;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (destination != null)
                    destination.PropertyChanged -= Destination_PropertyChanged;
                destination = value;
                destination.PropertyChanged += Destination_PropertyChanged;
            }
        }
        public BindingKind BindingKind { get; set; }

        public ObjectBinding()
        {
            BindingKind = BindingKind.TwoWay;
            sourceType = typeof(TSource);
            destinationType = typeof(TDestination);
        }
        public ObjectBinding(TSource source, TDestination destination) : this()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
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
        /// <param name="backConvertFunc">Converter function that is used when destination value is being passed to source</param>
        public void Bind<T, Y>(Expression<Func<TSource, T>> sourceExpr, Expression<Func<TDestination, Y>> destinationExpr,
                               Func<T, Y> convertFunc, Func<Y, T> convertBackFunc, Defaults<T,Y> defaults = default)
        {
            sourceProperty = sourceType.GetProperty(GetMember(sourceExpr.Body));
            destinationProperty = destinationType.GetProperty(GetMember(destinationExpr.Body));

            if (convertFunc != null && convertBackFunc != null)
                converter = new ValueConverter<T, Y>(convertFunc, convertBackFunc);
            else if (convertFunc == null)
                converter = new ValueConverter<T, Y>(convertBackFunc);
            else if (convertBackFunc == null)
                converter = new ValueConverter<T, Y>(convertFunc);
            else
                throw new ArgumentNullException("At least one of the converters must be set");

            if (defaults.sourceIsSet && sourceProperty.CanWrite)
                sourceProperty.SetValue(source, defaults.source);
            if (defaults.destinationIsSet && destinationProperty.CanWrite)
                destinationProperty.SetValue(destination, defaults.destination);
        }
        /// <summary>
        /// Sets up the property binding for the objects this instance is attached to
        /// </summary>
        /// <typeparam name="T">Source property type</typeparam>
        /// <typeparam name="Y">Destination property type</typeparam>
        /// <param name="sourceExpr">Expression to extract a property from the source</param>
        /// <param name="destinationExpr">Expression to extract a property from the destination</param>
        /// <param name="converter">The converter that is used when value is being passed to source/destination property</param>
        public void Bind<T, Y>(Expression<Func<TSource, T>> sourceExpr, Expression<Func<TDestination, Y>> destinationExpr, 
                               ValueConverter<T, Y> converter, Defaults<T,Y> defaults = default)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            sourceProperty = sourceType.GetProperty(GetMember(sourceExpr.Body));
            destinationProperty = destinationType.GetProperty(GetMember(destinationExpr.Body));
            if (defaults.sourceIsSet && sourceProperty.CanWrite)
                sourceProperty.SetValue(source, defaults.source);
            if (defaults.destinationIsSet && destinationProperty.CanWrite)
                destinationProperty.SetValue(destination, defaults.destination);
        }

        private string GetMember(Expression expr)
        {
            if (expr.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("Expected a member access expression");
            var member = (expr as MemberExpression).Member;
            return member.Name;
        }

        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (suppressHandler)
                return;
            if (e.PropertyName != sourceProperty.Name)
                return;
            suppressHandler = true;
            switch (BindingKind)
            {
                case BindingKind.OneWay:
                case BindingKind.TwoWay:
                    object value = converter.Convert(sourceProperty.GetValue(source));
                    destinationProperty.SetValue(destination, value);
                    break;
            }
            suppressHandler = false;
        }
        private void Destination_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (suppressHandler)
                return;
            if (e.PropertyName != destinationProperty.Name)
                return;
            suppressHandler = true;
            switch (BindingKind)
            {
                case BindingKind.OneWayToSource:
                case BindingKind.TwoWay:
                    object value = converter.ConvertBack(destinationProperty.GetValue(destination));
                    sourceProperty.SetValue(source, value);
                    break;
            }
            suppressHandler = false;
        }

    }

    public struct Defaults<T, Y>
    {
        public readonly bool sourceIsSet;
        public readonly bool destinationIsSet;
        public readonly T source;
        public readonly Y destination;

        public Defaults(T source)
        {
            this.source = source;
            destination = default;
            sourceIsSet = true;
            destinationIsSet = false;
        }
        public Defaults(Y destination)
        {
            this.destination = destination;
            source = default;
            sourceIsSet = false;
            destinationIsSet = true;
        }
        public Defaults(T source, Y destination)
        {
            this.source = source;
            this.destination = destination;
            sourceIsSet = true;
            destinationIsSet = true;
        }
    }
}