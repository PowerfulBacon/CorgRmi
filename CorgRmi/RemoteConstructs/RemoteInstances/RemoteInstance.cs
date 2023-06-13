using CorgRmi.Networking.Interfaces;
using CorgRmi.RemoteConstructs.RemoteTargets;
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
        /// Remote instances can only be created internally by getting the instance
        /// from a connection function.
        /// </summary>
        internal RemoteInstance(INetClient hostClient, INetClient localClient)
        {
            Host = new RemoteTarget(this, hostClient);
            if (hostClient != localClient)
                LocalClient = new RemoteTarget(this, localClient);
            else
                LocalClient = Host;
        }

        internal void SendMessage(RemoteTarget target, byte[] data)
        {

        }

    }
}
