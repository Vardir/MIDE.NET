namespace Vardirsoft.XApp.IoC
{
    public class MappedShelfBuilder<TPrimaryKey> : IShelfBuilder
    {
        internal MappedShelfBuilder()
        {
            
        }

        public MappedShelfInstanceCreation<TPrimaryKey, TPrimaryKey> ByCurrentKey() => new MappedShelfInstanceCreation<TPrimaryKey, TPrimaryKey>();
        
        public MappedShelfInstanceCreation<TPrimaryKey, TKey> ByKeyOf<TKey>() => new MappedShelfInstanceCreation<TPrimaryKey, TKey>();
    }
}