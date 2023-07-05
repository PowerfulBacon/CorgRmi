using CorgRmi.Logging;
using CorgRmi.Networking;
using CorgRmi.Networking.Interfaces;
using CorgRmi.Networking.InternalPackets;
using CorgRmi.RemoteConstructs.RemoteTargets;
using CorgRmi.RemoteObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteConstructs.RemoteInstances
{
    public class RemoteInstance
    {

        /// <summary>
        /// The remote target representing the host of this remote instance.
        /// </summary>
        public RemoteTarget Host { get; }

        /// <summary>
        /// The remote target representing the locally connected client to the
        /// remote instance.
        /// </summary>
        public RemoteTarget LocalClient { get; }

        /// <summary>
        /// Determines if we are the host or not
        /// </summary>
        public bool IsHost => Host == LocalClient;

        /// <summary>
        /// List of all clients connected to the instance
        /// </summary>
        public List<RemoteTarget> Clients { get; } = new List<RemoteTarget>();

        /// <summary>
        /// All network objects with their ID as their index.
        /// </summary>
        private RemoteObject?[] networkedObjects = new RemoteObject?[1024];

		/// <summary>
		/// The implementation of net-host that we use in order to instantiate and
        /// register connected clients.
		/// </summary>
		internal INetHost? NetHost { get; private set; }

        /// <summary>
        /// The logger for this object
        /// </summary>
        internal ILogger? logger;

		/// <summary>
		/// Remote instances can only be created internally by getting the instance
		/// from a connection function.
		/// </summary>
		internal RemoteInstance(INetClient hostClient, INetClient localClient)
		{
			CorgRmi.CheckReady();
			Host = new RemoteTarget(this, hostClient);
            if (hostClient != localClient)
                LocalClient = new RemoteTarget(this, localClient);
            else
                LocalClient = Host;
		}

        /// <summary>
        /// Start a remote instance where we are locally the host
        /// </summary>
        /// <param name="netHost"></param>
        internal RemoteInstance(INetHost netHost)
		{
			// Set the nethost
			NetHost = netHost;
			// Create a loopback client
			LoopbackNetClient loopbackClient = new LoopbackNetClient();
            Host = new RemoteTarget(this, loopbackClient);
            LocalClient = Host;
            // When a client connects, register that connection
            NetHost.OnClientConnected += c => AddConnection(c);
		}

        /// <summary>
        /// Add a new connection to the instance
        /// </summary>
        /// <param name="targetClient"></param>
        /// <returns></returns>
        internal RemoteTarget AddConnection(INetClient targetClient)
        {
            RemoteTarget client = new RemoteTarget(this, targetClient);
            // Tell the client that we accept their connection
            client.SendPacket<ConnectionAcceptedPacket, bool>(true);
            logger?.LogMessage(this, $"A remote target has connected.");
			return client;
		}

        /// <summary>
        /// Register a remote object in our remote instance list
        /// </summary>
        /// <param name="remoteObject"></param>
        internal void RegisterRemoteObject(RemoteObject remoteObject, bool registerToClients)
        {
            lock (this)
            {
                if (remoteObject.ID == null)
                    throw new Exception("Remote object has no ID and cannot be registered.");
                // Double the size of the array to fit the object
                while (remoteObject.ID >= networkedObjects.Length)
                {
                    // Double the length of the remote object array
                    RemoteObject?[] newRemoteObjectArray = new RemoteObject?[networkedObjects.Length * 2];
                    networkedObjects.CopyTo(newRemoteObjectArray, 0);
                    networkedObjects = newRemoteObjectArray;
                }
                // Insert the object
                networkedObjects[remoteObject.ID ?? throw new Exception()] = remoteObject;
			}
            if (!registerToClients)
                return;
            // Send the object to the individuals on the server who need to know about it
            if (remoteObject.VisibilityMode == RemoteObjects.DefaultVisibility.ALWAYS_VISIBLE)
            {
                Clients.ForEach(client => {
                    if (client == LocalClient)
                        return;
                    // Send the packet
                    client.SendPacket<CreateRemoteObjectPacket, RemoteObject>(remoteObject);
                });
			}
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Add a logger to this remove instance.
        /// </summary>
        /// <param name="logger">The instance of the logger to use</param>
        public RemoteInstance WithLogger(ILogger logger)
        {
            this.logger = logger;
			return this;
        }

        /// <summary>
        /// Get a remote object by its ID, assuming that the object is visible to us.
        /// </summary>
        /// <param name="ID">The ID of the object to retrieve</param>
        /// <returns></returns>
		public RemoteObject? GetObjectByID(int ID)
		{
            return networkedObjects[ID];
		}

        /// <summary>
        /// The count of remote entities
        /// </summary>
        private static int _remoteEntityCount = 0;

        /// <summary>
        /// Gets a unique ID in order to assign to remote objects.
        /// </summary>
        /// <returns></returns>
        internal int GetUniqueObjectID()
        {
            return Interlocked.Increment(ref _remoteEntityCount);
		}

    }
}
