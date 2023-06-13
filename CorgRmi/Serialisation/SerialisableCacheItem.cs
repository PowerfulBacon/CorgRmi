using CorgRmi.Serialisation.Attributes;
using CorgRmi.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Serialisation
{
    internal class SerialisableCacheItem
    {

        private int netvarSerialisationLength;

        private IEnumerable<FieldInfo> netVarFieldMembers;

        private IEnumerable<PropertyInfo> netVarPropertyMembers;

        private IEnumerable<FieldInfo> serialisedFieldMembers;

        private IEnumerable<PropertyInfo> serialisedPropertyMembers;

        internal SerialisableCacheItem(object target)
        {
            // Fetch all the things that require serialisation
            // Every netvar requires 4 bytes (uint ID) - 4 billion variables max
            netVarFieldMembers = target.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.FieldType.IsAssignableFrom(typeof(NetVar<object>).BaseType));
            netVarPropertyMembers = target.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.PropertyType.IsAssignableFrom(typeof(NetVar<object>).BaseType));
            netvarSerialisationLength = (netVarFieldMembers.Count() + netVarPropertyMembers.Count()) * (sizeof(uint) + sizeof(uint));
            // We also need to add all serialised value types
            serialisedFieldMembers = target.GetType()
                // Select field types that are serialised
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<CorgSerialiseAttribute>() != null);
            serialisedPropertyMembers = target.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.GetCustomAttribute<CorgSerialiseAttribute>() != null);
        }

        /// <summary>
        /// Calculate the number of bytes required in order to serialise
        /// an object.
        /// </summary>
        /// <param name="target">The object we wish to serialise</param>
        /// <param name="depth">The current depth of the serialisation, useds to block self-referencing expressions</param>
        /// <returns>Returns the number of bytes required to serialise the target object.</returns>
        /// <exception cref="StackOverflowException">Exception thrown if a self-referential exception is present</exception>
        public int CalculateSerialisationLength(object? target, int depth = 0)
        {
            // Target can be null
            if (target == null)
                return sizeof(ushort);
            // Check for maximum depth
            if (depth > 1024)
                throw new StackOverflowException("Reached maximum recursion depth while calculating serialisation length, there may be a self-referencing object.");
            int currentSize = 0;
            // The object has a type which needs to be sent across
            currentSize += sizeof(ushort);
            // Calculate the required length needed to serialise the field members
            foreach (FieldInfo field in serialisedFieldMembers)
            {
                object? value = field.GetValue(target);
                if (value == null)
                    // Send across the null type
                    currentSize += sizeof(ushort);
                else
                    currentSize += NetSerialiser.GetSerialiastionLength(value);
            }
            // Do the same for properties
            foreach (PropertyInfo property in serialisedPropertyMembers)
            {
                object? value = property.GetValue(target);
                if (value == null)
                    // Send across the null type
                    currentSize += sizeof(ushort);
                else
                    currentSize += NetSerialiser.GetSerialiastionLength(value);
            }
            // Return the calculated size
            return currentSize + netvarSerialisationLength;
        }

        public void PerformSerialisation(object? target, BinaryWriter writer)
        {
            // Write null into the bytes
            if (target == null)
            {
                writer.Write((uint)0);
                return;
            }
            // Write the type of the object
            writer.Write(ObjectIdentifierGenerator.GetNetworkedIdentifier(target));
            // If we are a value type, then write the value
            if (target.GetType().IsValueType || target.GetType() == typeof(string) || target.GetType().IsArray)
            {
                SerialiseInherentType(target, writer);
                return;
            }
            // Write the field members
            foreach (FieldInfo field in serialisedFieldMembers)
            {
                object? value = field.GetValue(target);
                if (value == null)
                    writer.Write(ObjectIdentifierGenerator.NULL_TYPE);
                else
                    NetSerialiser.Serialise(target, writer);
            }
            // Write the property members
            foreach (PropertyInfo property in serialisedPropertyMembers)
            {
                object? value = property.GetValue(target);
                if (value == null)
                    writer.Write(ObjectIdentifierGenerator.NULL_TYPE);
                else
                    NetSerialiser.Serialise(target, writer);
            }
            // Write the NetVar IDs
            netVarFieldMembers.ForEach(x => writer.Write((((NetVar?)x.GetValue(target)) ?? null)?.Identifier ?? 0));
            netVarPropertyMembers.ForEach(x => writer.Write((((NetVar?)x.GetValue(target)) ?? null)?.Identifier ?? 0));
            // TODO: Return a list of the netvars, so we can determine if they need to be serialised too
        }

        public void PerformDeserialisation(object? target, BinaryReader reader)
        {

        }

        private void SerialiseInherentType(object target, BinaryWriter writer)
        {
            Type objectType = target.GetType();
            // Handle serialisation if strings
            if (objectType == typeof(string))
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
            // Handle serialisation of array types
            // Handle serialisation of struct types
            // Handle serialisation of primitive types
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
                throw new Exception($"Failed to serialise object of type {target.GetType()} with value {target}");
        }

    }
}
