using System;
using System.Linq;

using Vardirsoft.Shared.Helpers;

namespace Vardirsoft.XApp.IoC
{
    public sealed class SingularShelfInstanceCreation<TPrimaryKey, TInstance> : IShelfInstanceCreation
    {
        internal SingularShelfInstanceCreation()
        {
            
        }
        
        public SingularShelfBuilderFinalizer<TPrimaryKey, TInstance> WithPerCallCreation()
        {
            var canCreate = CanCreateViaActivator(typeof(TInstance));
            
            return WithPerCallCreation(() => (TInstance)Activator.CreateInstance(typeof(TInstance)), canCreate ? null : $"Can't create new instances of type: {typeof(TInstance)}");
        }

        public SingularShelfBuilderFinalizer<TPrimaryKey, TInstance> WithDelayedCreation()
        {
            var canCreate = CanCreateViaActivator(typeof(TInstance));
            
            return WithDelayedCreation(InstanceCreation, canCreate ? null : $"Can't create new instances of type: {typeof(TInstance)}");
        }

        public SingularShelfBuilderFinalizer<TPrimaryKey, TInstance> WithInstantCreation()
        {
            var canCreate = CanCreateViaActivator(typeof(TInstance));
            
            return WithInstantCreation(InstanceCreation, canCreate ? null : $"Can't create new instances of type: {typeof(TInstance)}");
        }

        public SingularShelfBuilderFinalizer<TPrimaryKey, TInstance> WithPerCallCreation(Func<TInstance> instanceBuilder, string errorMessage)
        {
            return WithBuilder(instanceBuilder, CreationMode.CreatePerCall, errorMessage.HasValue() ? OperationResult.SuccessfulResult() : OperationResult.FailWith(errorMessage));
        }

        public SingularShelfBuilderFinalizer<TPrimaryKey, TInstance> WithDelayedCreation(Func<TInstance> instanceBuilder, string errorMessage)
        {
            return WithBuilder(instanceBuilder, CreationMode.CreateOnceOnExctraction, errorMessage.HasValue() ? OperationResult.SuccessfulResult() : OperationResult.FailWith(errorMessage));
        }

        public SingularShelfBuilderFinalizer<TPrimaryKey, TInstance> WithInstantCreation(Func<TInstance> instanceBuilder, string errorMessage = null)
        {
            return WithBuilder(instanceBuilder, CreationMode.CreateOnceOnRegister, errorMessage.HasValue() ? OperationResult.SuccessfulResult() : OperationResult.FailWith(errorMessage));
        }

        private static SingularShelfBuilderFinalizer<TPrimaryKey, TInstance> WithBuilder(Func<TInstance> instanceBuilder, CreationMode creationMode, OperationResult errorMessage)
        {
            return new SingularShelfBuilderFinalizer<TPrimaryKey, TInstance>(instanceBuilder, creationMode, errorMessage);
        }

        private static bool CanCreateViaActivator(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(Type));

            if (type.IsAbstract || type.IsInterface || type.IsPrimitive || type.IsNotPublic || type.IsCOMObject)
                return false;

            return type.GetConstructors().Any(x => x.IsPublic && !x.GetParameters().HasItems());
        }
        
        private static TInstance InstanceCreation() => (TInstance)Activator.CreateInstance(typeof(TInstance));
    }
}