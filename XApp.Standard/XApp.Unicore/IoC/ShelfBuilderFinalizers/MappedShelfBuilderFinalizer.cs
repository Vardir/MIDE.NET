namespace Vardirsoft.XApp.IoC
{
    public class MappedShelfBuilderFinalizer<TPrimaryKey, TSecondaryKey, TInstance> : BaseShelfBuilderFinalizer<TPrimaryKey>
    {
        internal MappedShelfBuilderFinalizer()
        {
            
        }

        internal override (OperationResult, IShelf) CreateShelf()
        {
            return (OperationResult.SuccessfulResult(), new MappedShelf<TSecondaryKey, TInstance>());
        }
    }
}