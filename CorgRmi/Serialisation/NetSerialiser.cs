using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Serialisation
{
    /// <summary>
    /// Automatically serialise
    /// </summary>
    internal static class NetSerialiser
    {

        /// <summary>
        /// the cache for reflection items to prevent performance drops from large numbers
        /// of slow reflection calls
        /// </summary>
        private static ConcurrentDictionary<Type, SerialisableCacheItem> _reflectionCache = new();

        public static int GetSerialiastionLength(object target)
        {
            return GetCacheItem(target).CalculateSerialisationLength(target);
        }

        public static void Serialise(object target, BinaryWriter writer)
        {

        }

        public static object Deserialise(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        internal static SerialisableCacheItem GetCacheItem(object target)
        {
            Type targetType = target.GetType();
            // Deal with value types
            if (targetType.IsValueType)
            {
                throw new ArgumentException($"The value type {targetType} cannot be serialised.");
            }
            // Deal with generic types
            if (targetType.IsGenericType)
            {
                if (targetType.DeclaringType == null)
                    throw new ArgumentException("Generic type does not have a declaring type. This should be impossible.");
                // Look in the cache for the declaring type
                if (_reflectionCache.TryGetValue(targetType.DeclaringType, out var cachedItem))
                {
                    return cachedItem;
                }
                // Generate the cache item, cache it and then return the value that we need.
                SerialisableCacheItem cacheItem = new(target);
                _reflectionCache.TryAdd(targetType.DeclaringType, cacheItem);
                return cacheItem;
            }
            else
            {
                // Deal with object types
                if (!targetType.IsClass)
                    throw new ArgumentException($"The type {targetType} is not supported for serialisation, it must be a value type, generic type or class.");
                // Look in the cache
                if (_reflectionCache.TryGetValue(targetType, out var cachedItem))
                {
                    return cachedItem;
                }
                // Generate the cache item, cache it and then return the value that we need.
                SerialisableCacheItem cacheItem = new(target);
                _reflectionCache.TryAdd(targetType, cacheItem);
                return cacheItem;
            }
        }

    }
}
