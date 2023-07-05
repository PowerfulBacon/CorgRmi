using CorgRmi.RemoteObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
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

        public static int GetSerialiastionLength(Type baseType, object? target)
        {
			if (baseType.IsPrimitive)
			{
				// Primitives cannot be null
				return Marshal.SizeOf(target!);
			}
			// Nullable type
			if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				// Read the null flag
				if (target == null)
					return sizeof(bool);
				return sizeof(bool) + GetSerialiastionLength(baseType.GenericTypeArguments[0], target);
			}
			if (baseType == typeof(string))
			{
				if (target == null)
					return sizeof(ushort);
				return sizeof(ushort) + Encoding.ASCII.GetBytes(target.ToString() ?? "").Length;
			}
			if (target == null)
				throw new ArgumentNullException("Attempting to serialise a null object stored in a non-nullable type. Any variables containing null must be marked as nullable.");
			return GetCacheItem(target.GetType()).CalculateSerialisationLength(target);
        }

        public static void Serialise(Type baseType, object? target, BinaryWriter writer)
		{
            Type objectType = target?.GetType() ?? baseType;
			if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				// Read the null flag
				if (target == null)
				{
					writer.Write(false);
					return;
				}
				writer.Write(true);
				Serialise(baseType.GenericTypeArguments[0], target, writer);
			}
			else if (target == null)
			{
				throw new NotImplementedException("Cannot serialise null at this time");
			}
			else if (typeof(RemoteObject).IsAssignableFrom(baseType))
			{
				GetCacheItem(target.GetType()).PerformSerialisation(target, writer);
			}
			else if (objectType == typeof(string))
			{
				if (string.IsNullOrEmpty(target as string))
				{
					writer.Write((ushort)0);
					return;
				}
				byte[] byteArray = Encoding.ASCII.GetBytes(target.ToString() ?? "");
				writer.Write((ushort)byteArray.Length);
				writer.Write(byteArray);
			}
			else if (objectType == typeof(bool))
				writer.Write((bool)target);
			else if (objectType == typeof(byte))
				writer.Write((byte)target);
			else if (objectType == typeof(char))
				writer.Write((char)target);
			else if (objectType == typeof(int))
				writer.Write((int)target);
			else if (objectType == typeof(float))
				writer.Write((float)target);
			else if (objectType == typeof(double))
				writer.Write((double)target);
			else if (objectType == typeof(long))
				writer.Write((long)target);
			else if (objectType == typeof(short))
				writer.Write((short)target);
			else if (objectType == typeof(uint))
				writer.Write((uint)target);
			else if (objectType == typeof(ushort))
				writer.Write((ushort)target);
			else if (objectType == typeof(ulong))
				writer.Write((ulong)target);
			else if (objectType == typeof(decimal))
				writer.Write((decimal)target);
			else
			    throw new NotImplementedException($"Cannot realised the object of type {objectType}");
		}

        public static object? Deserialise(Type deserialisationType, BinaryReader binaryReader)
		{
			if (typeof(RemoteObject).IsAssignableFrom(deserialisationType))
			{
				// First we need to determine the type that we are deserialising
				ushort networkedIdentifier = binaryReader.ReadUInt16();
				// Null
				if (networkedIdentifier == ObjectIdentifierGenerator.NULL_TYPE)
					return null;
				// Find the type
				Type? deserialisedType = ObjectIdentifierGenerator.GetTypeFromNetworkedIdentifier(networkedIdentifier);
				if (deserialisedType == null)
					return null;
				return GetCacheItem(deserialisedType).PerformDeserialisation(binaryReader);
			}
			else if (deserialisationType == typeof(string))
			{
				ushort stringLength = binaryReader.ReadUInt16();
				if (stringLength == 0)
					return null;
				byte[] byteArray = binaryReader.ReadBytes(stringLength);
				return Encoding.ASCII.GetString(byteArray);
			}
			else if (deserialisationType == typeof(bool))
				return binaryReader.ReadBoolean();
			else if (deserialisationType == typeof(byte))
				return binaryReader.ReadByte();
			else if (deserialisationType == typeof(char))
				return binaryReader.ReadChar();
			else if (deserialisationType == typeof(int))
				return binaryReader.ReadInt32();
			else if (deserialisationType == typeof(float))
				return binaryReader.ReadSingle();
			else if (deserialisationType == typeof(double))
				return binaryReader.ReadDouble();
			else if (deserialisationType == typeof(long))
				return binaryReader.ReadInt64();
			else if (deserialisationType == typeof(short))
				return binaryReader.ReadInt16();
			else if (deserialisationType == typeof(uint))
				return binaryReader.ReadUInt32();
			else if (deserialisationType == typeof(ushort))
				return binaryReader.ReadUInt16();
			else if (deserialisationType == typeof(ulong))
				return binaryReader.ReadUInt64();
			else if (deserialisationType == typeof(decimal))
				return binaryReader.ReadDecimal();
			else if (deserialisationType.IsGenericType && deserialisationType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				// Read the null flag
				if (binaryReader.ReadBoolean() == false)
					return null;
				return Deserialise(deserialisationType.GenericTypeArguments[0], binaryReader);
			}
			else
				throw new Exception($"Failed to deserialise object with type {deserialisationType}");
        }

        internal static SerialisableCacheItem GetCacheItem(Type targetType)
        {
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
                SerialisableCacheItem cacheItem = new(targetType);
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
                SerialisableCacheItem cacheItem = new(targetType);
                _reflectionCache.TryAdd(targetType, cacheItem);
                return cacheItem;
            }
        }

    }
}
