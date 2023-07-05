using CorgRmi.RemoteConstructs.RemoteInstances;
using CorgRmi.RemoteConstructs.RemoteObjects;
using CorgRmi.RemoteConstructs.RemoteTargets;
using CorgRmi.Serialisation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteObjects
{
    /// <summary>
    /// Represents an object which can be shared between different clients
    /// during a networking session.
    /// This provides functionality to serialise and deserialise the contents
    /// of the object, as well as calling shared functions.
    /// </summary>
    public abstract partial class RemoteObject
    {

        /// <summary>
        /// The instance that this object exists within.
        /// </summary>
        public RemoteInstance Instance { get; }

        /// <summary>
        /// The remote owner of this object.
        /// </summary>
        public RemoteTarget Owner { get; private set; }

        /// <summary>
        /// Are we the owner of this object? Returns true if the remote target that this object belongs to
        /// owner this object.
        /// </summary>
        public bool IsOwner => Owner == Instance.LocalClient;

        /// <summary>
        /// The shared network ID of this object.
        /// </summary>
        [CorgSerialise]
        public int? ID { get; internal set; } = null;

        /// <summary>
        /// What visibility mode should this remote object use?
        /// Note that objects contained within variables inside other objects will always be visible.
        /// </summary>
        public virtual DefaultVisibility VisibilityMode { get; } = DefaultVisibility.ALWAYS_VISIBLE;

        public RemoteObject(RemoteInstance instance, RemoteTarget? initialOwner = null)
        {
            Instance = instance;
            Owner = initialOwner ?? Instance.Host;
        }

		/// <summary>
		/// Perform remote initialisation. Transfer the object to other users.
		/// </summary>
		public void RemoteInitialise()
		{
			// We cannot remote initialise if we are not the host
			if (!Instance.IsHost)
				throw new Exception("Cannot remote initialise remote object if we are not the host.");
			ID = Instance.GetUniqueObjectID();
			Instance.RegisterRemoteObject(this, true);
		}

		/// <summary>
		/// Perform remote initialisation. Transfer the object to other users.
		/// </summary>
		public T RemoteInitialise<T>()
            where T : RemoteObject
		{
            // We cannot remote initialise if we are not the host
            if (!Instance.IsHost)
                throw new Exception("Cannot remote initialise remote object if we are not the host.");
			ID = Instance.GetUniqueObjectID();
            Instance.RegisterRemoteObject(this, true);
            return (T)this;
		}

        /// <summary>
        /// If the target cannot currently see this object, then it will be serialised and shared with them.
        /// Any subsequent updates to this object will also be shared with the target in the future.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Returns true if the operation was successful, returns false otherwise,</returns>
        public bool ExposeTo(RemoteTarget target)
        {
            if (VisibilityMode == DefaultVisibility.ALWAYS_VISIBLE)
                return false;
            throw new NotImplementedException();
        }

        /// <summary>
        /// If the target can see this object, and the visibility mode is not always visible, then this
        /// method will cause the object to stop streaming updates to the client.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Returns true if the operation was successful, false otherwise.</returns>
        public bool HideFrom(RemoteTarget target)
		{
			if (VisibilityMode == DefaultVisibility.ALWAYS_VISIBLE)
				return false;
			throw new NotImplementedException();
		}

    }
}
