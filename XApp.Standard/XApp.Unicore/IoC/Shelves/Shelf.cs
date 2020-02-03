using System;

namespace Vardirsoft.XApp.IoC
{
    internal interface IShelf
    {
        object Extract();
    }
    
    internal class Shelf<T> : IShelf
    {
        private bool _onceCreated;
        private T _instance;
        private readonly Func<T> _builder;
        private readonly CreationMode _creationMode;

        public Shelf(Func<T> builder, CreationMode mode)
        {
            _creationMode = mode;
            _builder = builder;

            if (mode == CreationMode.CreateOnceOnRegister)
            {
                _instance = builder();
            }
        }

        public T Extract()
        {
            if (_creationMode == CreationMode.CreateOnceOnExctraction)
            {
                var result = _onceCreated ? _instance : (_instance = _builder());
                
                _onceCreated = true;
                
                return result;
            }
                
            return _creationMode == CreationMode.CreatePerCall ? _builder() : _instance;
        }

        object IShelf.Extract() => Extract();
    }
}