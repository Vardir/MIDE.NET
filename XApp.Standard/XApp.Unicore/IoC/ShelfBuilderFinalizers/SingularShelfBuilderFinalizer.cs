using System;

namespace Vardirsoft.XApp.IoC
{
    public class SingularShelfBuilderFinalizer<TPrimaryKey, TInstance> : BaseShelfBuilderFinalizer<TPrimaryKey>
    {
        private readonly Func<TInstance> _instanceBuilder;
        private readonly CreationMode _creationMode;
        private readonly OperationResult _previousOperationResult;
        
        internal SingularShelfBuilderFinalizer(Func<TInstance> instanceBuilder, CreationMode creationMode, OperationResult previousOperationResult = null)
        {
            _creationMode = creationMode;
            _instanceBuilder = instanceBuilder;
            _previousOperationResult = previousOperationResult ?? OperationResult.SuccessfulResult();
        }

        internal override (OperationResult, IShelf) CreateShelf()
        {
            if (_previousOperationResult.IsFailure)
                return (OperationResult.FailWith(((OperationResult.Failure)_previousOperationResult).Message), null);
            
            if (_instanceBuilder is null)
                return (OperationResult.FailWith("Shelf instance builder is null"), null);
            
            return (OperationResult.SuccessfulResult(), new Shelf<TInstance>(_instanceBuilder, _creationMode));
        }
    }
}