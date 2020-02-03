namespace Vardirsoft.XApp.IoC
{
    public class ShelfInitializer<T>
    {
        internal ShelfInitializer()
        {
            
        }
        
        public SingularShelfBuilder<T> AsSingular() => new SingularShelfBuilder<T>();
        
        public MappedShelfBuilder<T> AsMapped() => new MappedShelfBuilder<T>();
    }
}