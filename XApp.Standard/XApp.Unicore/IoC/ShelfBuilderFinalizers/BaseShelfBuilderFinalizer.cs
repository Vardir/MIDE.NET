namespace Vardirsoft.XApp.IoC
{
    public abstract class BaseShelfBuilderFinalizer<TPrimaryKey>
    {
        public OperationResult FinalizeBuilding()
        {
            OnBuildingFinishing();
            
            var (result, shelf) = CreateShelf();
            
            if (result.IsSuccess)
            {
                IoCContainer.Register<TPrimaryKey>(shelf);
            }
            
            OnBuildingFinished(result);
            
            return result;
        }

        protected virtual void OnBuildingFinishing() { }
        
        protected virtual void OnBuildingFinished(OperationResult result) { }

        internal abstract (OperationResult, IShelf) CreateShelf();
    }
}