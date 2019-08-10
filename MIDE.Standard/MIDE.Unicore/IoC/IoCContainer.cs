using System;
using System.Collections.Generic;

namespace MIDE.IoC
{
    public static class IoCContainer
    {        
        private static Dictionary<Type, Shelf> shelves;
        
        static IoCContainer()
        {
            shelves = new Dictionary<Type, Shelf>();
        }

        public static void Store(object instance)
        {
            var type = instance.GetType();

            if (shelves.ContainsKey(type))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            shelves.Add(type, new Shelf(() => instance, CreationMode.CreateOnceOnRegister));
        }
        public static void Store<TKey, TInstance>(TInstance instance) => Store(typeof(TKey), instance);
        public static void Store(Type key, object instance)
        {
            if (shelves.ContainsKey(key))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            shelves.Add(key, new Shelf(() => instance, CreationMode.CreateOnceOnRegister));
        }
        public static void Registrate<TKey, TInstance>(CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
            where TInstance: new()
        {
            var keyType = typeof(TKey);
            var instanceType = typeof(TInstance);

            Registrate(keyType, instanceType, creationMode);
        }
        public static void Registrate<TKey, TInstance>(Func<object> builder,
                                                CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
        {
            var keyType = typeof(TKey);

            Registrate(keyType, builder, creationMode);
        }
        public static void Registrate(Type keyType, Type instanceType,
                               CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
        {
            if (shelves.ContainsKey(keyType))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            shelves.Add(keyType, new Shelf(() => Activator.CreateInstance(instanceType), creationMode));
        }
        public static void Registrate(Type key, Func<object> builder,
                               CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
        {
            if (shelves.ContainsKey(key))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            shelves.Add(key, new Shelf(builder, creationMode));
        }

        public static TKey Resolve<TKey>(bool throwIfCantCast = false, bool throwIfNotFound = false) => Extract<TKey, TKey>(throwIfCantCast, throwIfNotFound);
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
                if (throwIfCantCast) throw;
            }
            return default;
        }
        public static object Extract(Type type, bool throwIfNotFound = false)
        {
            try
            {
                return shelves[type].Extract();
            }
            catch
            {
                if (throwIfNotFound) throw;
            }
            return null;
        }


        private class Shelf
        {
            private bool onceCreated;
            private object instance;
            private Func<object> builder;
            private readonly CreationMode creationMode;

            public Shelf(Func<object> builder, CreationMode mode)
            {
                this.creationMode = mode;
                this.builder = builder;

                if (mode == CreationMode.CreateOnceOnExctraction)
                {
                    instance = builder();
                }
            }

            public object Extract()
            {
                if (instance == null && creationMode == CreationMode.CreateOnceOnExctraction)
                    return (instance = builder());
                if (creationMode == CreationMode.CreatePerCall)
                    return builder();

                return instance;
            }
        }
    }
}