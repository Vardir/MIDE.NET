using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Vardirsoft.XApp.IoC
{
    public static class IoCContainer
    {        
        private static readonly Dictionary<Type, Shelf> Shelves;
        
        static IoCContainer()
        {
            Shelves = new Dictionary<Type, Shelf>();
        }

        public static void Store(object instance)
        {
            var type = instance.GetType();

            if (Shelves.ContainsKey(type))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            Shelves.Add(type, new Shelf(() => instance, CreationMode.CreateOnceOnRegister));
        }
        
        [DebuggerStepThrough]
        public static void Store<TKey, TInstance>(TInstance instance) => Store(typeof(TKey), instance);
        
        public static void Store(Type key, object instance)
        {
            if (Shelves.ContainsKey(key))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            Shelves.Add(key, new Shelf(() => instance, CreationMode.CreateOnceOnRegister));
        }
        
        public static void Register<TKey, TInstance>(CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
            where TInstance: new()
        {
            var keyType = typeof(TKey);
            var instanceType = typeof(TInstance);

            Register(keyType, instanceType, creationMode);
        }
        
        public static void Register<TKey>(Func<object> builder, CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
        {
            var keyType = typeof(TKey);

            Register(keyType, builder, creationMode);
        }
        
        public static void Register(Type keyType, Type instanceType, CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
        {
            if (Shelves.ContainsKey(keyType))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            Shelves.Add(keyType, new Shelf(() => Activator.CreateInstance(instanceType), creationMode));
        }
        
        public static void Register(Type key, Func<object> builder, CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
        {
            if (Shelves.ContainsKey(key))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            Shelves.Add(key, new Shelf(builder, creationMode));
        }

        [DebuggerStepThrough]
        public static TKey Resolve<TKey>(bool throwIfCantCast = false, bool throwIfNotFound = false) => Extract<TKey, TKey>(throwIfCantCast, throwIfNotFound);
        
        [DebuggerStepThrough]
        public static object Extract<TKey>(bool throwIfNotFound = false) => Extract(typeof(TKey), throwIfNotFound);
        
        public static TInstance Extract<TKey, TInstance>(bool throwIfCantCast = false, bool throwIfNotFound = false)
        {
            var result = Extract(typeof(TKey), throwIfNotFound);
            try 
            {
                return (TInstance)result;
            }
            catch
            {
                if (throwIfCantCast) 
                    throw;
            }

            return default;
        }
        
        public static object Extract(Type type, bool throwIfNotFound = false)
        {
            try
            {
                return Shelves[type].Extract();
            }
            catch
            {
                if (throwIfNotFound) 
                    throw;
            }

            return null;
        }


        private class Shelf
        {
            private object _instance;
            private readonly Func<object> _builder;
            private readonly CreationMode _creationMode;

            public Shelf(Func<object> builder, CreationMode mode)
            {
                _creationMode = mode;
                _builder = builder;

                if (mode == CreationMode.CreateOnceOnRegister)
                {
                    _instance = builder();
                }
            }

            public object Extract()
            {
                if (_instance is null && _creationMode == CreationMode.CreateOnceOnExctraction)
                    return _instance = _builder();
                    
                return _creationMode == CreationMode.CreatePerCall ? _builder() : _instance;
            }
        }
    }
}