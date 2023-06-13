using CorgRmi.Serialisation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Serialisation
{
    /// <summary>
    /// Generates a version number based on the classes included with the project.
    /// Generates unique IDs for every type that has the serialise tag.
    /// </summary>
    internal static class ObjectIdentifierGenerator
    {

        public const ushort NULL_TYPE = 0;

        /// <summary>
        /// Allows for getting a version type for these, when used with generic
        /// types.
        /// The generic type will need to implement a deserialiser as they do not
        /// implement ICustomSerialisationBehaviour.
        /// </summary>
        private static Type[] SyncedBaseTypes = new Type[] {
            typeof(byte),
            typeof(char),
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(long),
            typeof(short),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(decimal),
            typeof(string)
        };

        /// <summary>
        /// A dictionary containing the networked Event IDs by type.
        /// </summary>
        private static Dictionary<Type, ushort> networkedTypeIds = new Dictionary<Type, ushort>();

        private static Dictionary<ushort, Type> networkIdToType = new Dictionary<ushort, Type>();

        /// <summary>
        /// Generates the networked IDs for all networked events.
        /// Deterministic, so they will always be in the same order
        /// (Client and server need to be synced)
        /// </summary>
        internal static void CreateNetworkedIDs(IEnumerable<Assembly> assemblyModules)
        {
            //Use reflection to collect all event types
            //Sort the types alphabetically (They need to be sorted to be deterministic)
            IOrderedEnumerable<Type> LocatedTypes = assemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .Where(t => t.GetCustomAttribute(typeof(CorgSerialiseAttribute)) != null))
                .Union(SyncedBaseTypes)
                .OrderBy(networkedType =>
                {
                    //return 0;
                    return networkedType.AssemblyQualifiedName;
                });
            //Assign all events a non-0 ID.
            ushort number = 1;
            foreach (Type type in LocatedTypes)
            {
                //Logger.WriteLine($"NETWORK VERSION: #{number}: {type.AssemblyQualifiedName} (HASH: #{GetUnifiedHashedString(type.AssemblyQualifiedName)})", LogType.DEBUG);
                networkIdToType.Add(number, type);
                networkedTypeIds.Add(type, number++);
            }
            //Print message
            //Logger?.WriteLine($"Generated IDs for {number - 1} networked types, current networked version ID: {versionID}", LogType.MESSAGE);
        }

        public static ushort GetNetworkedIdentifier(this Type type)
        {
            ushort output;
            if (!networkedTypeIds.TryGetValue(type.IsGenericType ? type.GetGenericTypeDefinition() : type, out output))
                throw new Exception($"Failed to get networked identifier for type {type}, the type does not implement IVersionSynced.");
            return output;
        }

        /// <summary>
        /// Get the networked identifier for a specific object that has the serialise tag.
        /// </summary>
        /// <param name="versionSyncedType">The object to lookup the type identifier for.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ushort GetNetworkedIdentifier(this object versionSyncedType)
        {
            if (versionSyncedType == null)
                return NULL_TYPE;
            ushort output;
            if (!networkedTypeIds.TryGetValue(versionSyncedType.GetType(), out output))
                throw new Exception($"Failed to get networked identifier for version synced class {versionSyncedType.GetType()}.");
            return output;
        }

        public static Type? GetTypeFromNetworkedIdentifier(ushort networkedID)
        {
            if (networkedID == NULL_TYPE)
                return null;
            return networkIdToType[networkedID];
        }

        /// <summary>
        /// Returns an uninitialized object of the type represented by the identifier of that type.
        /// </summary>
        internal static T? CreateTypeFromIdentifier<T>(ushort networkedID)
            where T : class
        {
            if (networkedID == NULL_TYPE)
                return null;
            return FormatterServices.GetUninitializedObject(networkIdToType[networkedID]) as T
                ?? throw new ArgumentOutOfRangeException($"The network ID {networkedID} is out of ranged.");
        }

    }
}
