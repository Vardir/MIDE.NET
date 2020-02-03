namespace Vardirsoft.XApp.IoC
{
    public class SingularShelfBuilder<TPrimaryKey> : IShelfBuilder
    {
        internal SingularShelfBuilder()
        {
            
        }
        
        public SingularShelfInstanceCreation<TPrimaryKey, TPrimaryKey> OfCurrent() => new SingularShelfInstanceCreation<TPrimaryKey, TPrimaryKey>();

        public SingularShelfInstanceCreation<TPrimaryKey, TInstance> Of<TInstance>() => new SingularShelfInstanceCreation<TPrimaryKey, TInstance>();

        public SingularShelfBuilderFinalizer<TPrimaryKey, TInstance> With<TInstance>(TInstance instance) => new SingularShelfInstanceCreation<TPrimaryKey, TInstance>().WithInstantCreation(() => instance);
    }
}