using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Vardirsoft.XApp.Helpers
{
    public static class Guard
    {
        public static void EnsureNot(bool condition, string failureMessage)
        {
            if (condition)
                throw new Exception(failureMessage);
        }
        
        public static void EnsureNot(bool condition, Type exceptionType, string failureMessage = null)
        {
            if (condition)
                throw GenerateException(exceptionType, failureMessage);
        }
        
        public static void Ensure(bool condition, string failureMessage)
        {
            if (!condition)
                throw new Exception(failureMessage);
        }

        public static void EnsureForAll(IEnumerable collection, Func<object, bool> predicate, Type exceptionType, string failureMessage = null)
        {
            EnsureNotNull(collection);
            EnsureCanUpCast(exceptionType, typeof(Exception));

            foreach (var item in collection)
            {
                if (!predicate(item)) 
                   throw GenerateException(exceptionType, typeof(Exception), failureMessage);
            }
        }
        
        public static void EnsureForAll<T>(IEnumerable<T> collection, Func<T, bool> predicate, Type exceptionType, string failureMessage = null)
        {
            EnsureNotNull(collection);
            EnsureCanUpCast(exceptionType, typeof(Exception));

            if (!collection.All(predicate))
                throw GenerateException(exceptionType, typeof(Exception), failureMessage);
        }
        
        public static void Ensure(bool condition, Type exceptionType, string failureMessage = null)
        {
            if (!condition)
                throw GenerateException(exceptionType, failureMessage);
        }
        
        public static void EnsureEmpty(string value, Type exceptionType, string failureMessage = null)
        {
            if (!string.IsNullOrEmpty(value))
                throw GenerateException(exceptionType, failureMessage);
        }

        public static void EnsureNonEmpty(string value, Type exceptionType, string failureMessage = null)
        {
            if (string.IsNullOrEmpty(value))
                throw GenerateException(exceptionType, typeof(NullReferenceException), failureMessage);
        }
        
        public static void EnsureNonEmpty<T>(IEnumerable<T> collection, string failureMessage = null)
        {
            EnsureNotNull(collection, null, failureMessage);
            
            if (!collection.Any())
                throw new Exception(failureMessage);
        }
        
        public static void EnsureNonEmpty<T>(ICollection<T> collection, string failureMessage = null)
        {
            EnsureNotNull(collection, null, failureMessage);
            
            if (collection.Count == 0)
                throw new Exception(failureMessage);
        }
        
        public static void EnsureNonEmpty<T>(T[] collection, string failureMessage = null)
        {
            EnsureNotNull(collection, null, failureMessage);
            
            if (collection.Length == 0)
                throw new Exception(failureMessage);
        }
        
        public static void EnsureIsNull<T>(T obj, Type exceptionType = null, string failureMessage = null)
        {
            if (!(obj is null))
                throw GenerateException(exceptionType, typeof(NullReferenceException), failureMessage);
        }
        
        public static void EnsureNotNull<T>(T obj, Type exceptionType = null, string failureMessage = null)
        {
            if (obj is null)
                throw GenerateException(exceptionType, typeof(NullReferenceException), failureMessage);
        }

        public static void ArgumentNotNull<T>(T obj, string argumentName)
        {
            if (obj is null)
                throw new ArgumentNullException(argumentName);
        }

        public static void EnsureCanUpCast(Type type, Type baseType, string failureMessage = null)
        {
            EnsureNotNull(type);
            EnsureNotNull(baseType);
            
            if (!type.IsSubclassOf(baseType))
                throw new InvalidCastException(failureMessage);
        }

        private static Exception GenerateException(Type exceptionType, Type supportingExceptionType, string message)
        {
            if (exceptionType is null)
                return GenerateException(supportingExceptionType, message);

            return GenerateException(exceptionType, message);
        }

        private static Exception GenerateException(Type exceptionType, string message)
        {
            EnsureCanUpCast(exceptionType, typeof(Exception));
            
            return (Exception)Activator.CreateInstance(exceptionType, message);
        }
    }
}