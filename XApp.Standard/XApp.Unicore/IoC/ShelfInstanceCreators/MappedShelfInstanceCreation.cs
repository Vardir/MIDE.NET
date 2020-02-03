namespace Vardirsoft.XApp.IoC
{
    public class MappedShelfInstanceCreation<TPrimaryKey, TSecondaryKey> : IShelfInstanceCreation
    {
        internal MappedShelfInstanceCreation()
        {
            
        }

        public MappedShelfBuilderFinalizer<TPrimaryKey, TSecondaryKey, TPrimaryKey> ForCurrent() => new MappedShelfBuilderFinalizer<TPrimaryKey, TSecondaryKey, TPrimaryKey>();
        
        public MappedShelfBuilderFinalizer<TPrimaryKey, TSecondaryKey, TInstance> For<TInstance>() => new MappedShelfBuilderFinalizer<TPrimaryKey, TSecondaryKey, TInstance>();
    }
}