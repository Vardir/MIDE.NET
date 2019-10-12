namespace Vardirsoft.XApp.Bindings
{
    public enum BindingKind
    {
        /// <summary>
        /// From source to destination
        /// </summary>
        OneWay,
        /// <summary>
        /// From one object to another / vice versa
        /// </summary>
        TwoWay,
        /// <summary>
        /// From destination to source
        /// </summary>
        OneWayToSource
    }
}