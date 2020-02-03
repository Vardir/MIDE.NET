using System;

namespace Vardirsoft.XApp.IoC
{
    internal sealed class ObjectShelf : Shelf<object>
    {
        public ObjectShelf(Func<object> builder, CreationMode mode) : base(builder, mode)
        {
            
        }
    }
}