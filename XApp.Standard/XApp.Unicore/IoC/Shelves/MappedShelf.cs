using System;
using System.Collections.Generic;

namespace Vardirsoft.XApp.IoC
{
    internal abstract class MappedShelf : IShelf
    {
        public Type KeyType { get; }

        public MappedShelf(Type keyType)
        {
            KeyType = keyType;
        }

        public virtual object Extract() => throw new NotImplementedException();
    }
    
    internal sealed class MappedShelf<TKey, TInstance> : MappedShelf
    {
        private readonly Dictionary<TKey, Shelf<TInstance>> _singletons;

        public MappedShelf() : base(typeof(TKey))
        {
            _singletons = new Dictionary<TKey, Shelf<TInstance>>();
        }

        public void Register<TK, TI>(TK key, TI instance)
            where TK : TKey
            where TI : TInstance
        {
            if (_singletons.ContainsKey(key))
                throw new InvalidOperationException("Duplicate key entry given for mapped shelf");
            
            _singletons.Add(key, new Shelf<TInstance>(() => instance, CreationMode.CreateOnceOnRegister));
        }
        
        public TInstance Extract<T>(T key) where T: TKey => _singletons[key].Extract();
    }
}