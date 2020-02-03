using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Vardirsoft.XApp.IoC
{
    public static class IoCContainer
    {        
        private static readonly Dictionary<Type, IShelf> Shelves;
        
        static IoCContainer()
        {
            Shelves = new Dictionary<Type, IShelf>();
        }

        [DebuggerStepThrough]
        public static void Store(object instance)
        {
            var type = instance.GetType();

            if (Shelves.ContainsKey(type))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            Shelves.Add(type, new ObjectShelf(() => instance, CreationMode.CreateOnceOnRegister));
        }
        
        [DebuggerStepThrough]
        public static void Store<TKey, TInstance>(TInstance instance) => Store(typeof(TKey), instance);
        
        [DebuggerStepThrough]
        public static void Store(Type key, object instance)
        {
            if (Shelves.ContainsKey(key))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            Shelves.Add(key, new ObjectShelf(() => instance, CreationMode.CreateOnceOnRegister));
        }
        
        [DebuggerStepThrough]
        public static void Register<TKey, TInstance>(CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
            where TInstance: new()
        {
            var keyType = typeof(TKey);
            var instanceType = typeof(TInstance);

            Register(keyType, instanceType, creationMode);
        }
        
        [DebuggerStepThrough]
        public static void Register<TKey>(Func<object> builder, CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
        {
            var keyType = typeof(TKey);

            Register(keyType, builder, creationMode);
        }
        
        [DebuggerStepThrough]
        public static void Register(Type keyType, Type instanceType, CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
        {
            if (Shelves.ContainsKey(keyType))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            Shelves.Add(keyType, new ObjectShelf(() => Activator.CreateInstance(instanceType), creationMode));
        }
        
        [DebuggerStepThrough]
        public static void Register(Type key, Func<object> builder, CreationMode creationMode = CreationMode.CreateOnceOnExctraction)
        {
            if (Shelves.ContainsKey(key))
                throw new NotImplementedException("Storing multiple instances per one shelf is not implemented yet");

            Shelves.Add(key, new ObjectShelf(builder, creationMode));
        }

        [DebuggerStepThrough]
        public static void RegisterMap<TPrimaryKey, TSecondaryKey>() => RegisterMap<TPrimaryKey, TSecondaryKey, TPrimaryKey>();
        
        [DebuggerStepThrough]
        public static void RegisterMap<TPrimaryKey, TSecondaryKey, TInstance>()
        {
            var primaryKey = typeof(TPrimaryKey);
            
            if (Shelves.ContainsKey(primaryKey))
                throw new InvalidOperationException("Duplicate shelf primary key");
            
            Shelves.Add(primaryKey, new MappedShelf<TSecondaryKey, TInstance>());
        }

        [DebuggerStepThrough]
        public static void RegisterMapped<TPrimaryKey, TSecondaryKey>(TSecondaryKey key, TPrimaryKey instance) => RegisterMapped<TPrimaryKey, TSecondaryKey, TPrimaryKey>(key, instance);

        [DebuggerStepThrough]
        public static void RegisterMapped<TPrimaryKey, TSecondaryKey, TInstance>(TSecondaryKey key, TInstance instance)
        {
            var shelf = Shelves[typeof(TPrimaryKey)];

            if (shelf is MappedShelf<TSecondaryKey, TInstance> mappedShelf)
            {
                mappedShelf.Register(key, instance);
            }
            
            throw new KeyNotFoundException($"Couldn't find a shelf with key of type <{typeof(TSecondaryKey)}>");
        }

        [DebuggerStepThrough]
        public static OperationResult TryRegisterMap<TPrimaryKey, TSecondaryKey>() => TryRegisterMap<TPrimaryKey, TSecondaryKey, TPrimaryKey>();
        
        [DebuggerStepThrough]
        public static OperationResult TryRegisterMap<TPrimaryKey, TSecondaryKey, TInstance>()
        {
            if (IsRegistered<TPrimaryKey, TSecondaryKey, TInstance>())
                return OperationResult.FailWith($"Mapped shelf of type <{typeof(TPrimaryKey)}> and key <{typeof(TSecondaryKey)}> is already registered");
            
            try
            {
                RegisterMap<TPrimaryKey, TSecondaryKey, TInstance>();
            }
            catch (Exception exc)
            {
                return OperationResult.FailWith(exc.Message);
            }
                
            return OperationResult.SuccessfulResult();
        }

        [DebuggerStepThrough]
        public static bool IsRegistered<TKey>() => Shelves.ContainsKey(typeof(TKey));

        [DebuggerStepThrough]
        public static bool NotRegistered<TKey>() => !IsRegistered<TKey>();

        [DebuggerStepThrough]
        public static bool IsRegistered<TPrimaryKey, TSecondaryKey>() => IsRegistered<TPrimaryKey, TSecondaryKey, TPrimaryKey>();
        
        [DebuggerStepThrough]
        public static bool IsRegistered<TPrimaryKey, TSecondaryKey, TInstance>()
        {
            if (Shelves.TryGetValue(typeof(TPrimaryKey), out var shelf))
                return shelf is MappedShelf<TSecondaryKey, TInstance>;

            return false;
        }

        public static ShelfInitializer<T> ShelfByKey<T>() => new ShelfInitializer<T>();

        public static OperationResult ShelfByKey<T>(Func<ShelfInitializer<T>, BaseShelfBuilderFinalizer<T>> builder)
        {
            OperationResult results;
            
            try
            {
                results = builder(new ShelfInitializer<T>()).FinalizeBuilding();
            }
            catch (Exception e)
            {
                results = OperationResult.FailWith(e.Message);
            }

            return results;
        }

        [DebuggerStepThrough]
        public static TKey Resolve<TKey>(bool throwIfCantCast = false, bool throwIfNotFound = false) => Extract<TKey, TKey>(throwIfCantCast, throwIfNotFound);
        
        [DebuggerStepThrough]
        public static object Extract<TKey>(bool throwIfNotFound = false) => Extract(typeof(TKey), throwIfNotFound);
        
        [DebuggerStepThrough]
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
        
        [DebuggerStepThrough]
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

        [DebuggerStepThrough]
        public static TPrimaryKey ResolveByKey<TPrimaryKey, TSecondaryKey>(TSecondaryKey key) => ResolveByKey<TPrimaryKey, TSecondaryKey, TPrimaryKey>(key);
        
        [DebuggerStepThrough]
        public static TInstance ResolveByKey<TPrimaryKey, TSecondaryKey, TInstance>(TSecondaryKey key)
        {
            var shelf = Shelves[typeof(TPrimaryKey)];

            if (shelf is MappedShelf<TSecondaryKey, TInstance> mappedShelf)
                return mappedShelf.Extract(key);

            throw new KeyNotFoundException($"Couldn't find a shelf with key of type <{typeof(TSecondaryKey)}>");
        }

        [DebuggerStepThrough]
        internal static void Register<TKey>(IShelf shelf) => Register(typeof(TKey), shelf);
        
        [DebuggerStepThrough]
        internal static void Register(Type key, IShelf shelf)
        {
            if (Shelves.ContainsKey(key))
                throw new InvalidOperationException($"Couldn't register a shelf by duplicate key type <{key}>");
            
            Shelves.Add(key, shelf);
        }
    }
}