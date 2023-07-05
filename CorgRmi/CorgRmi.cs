using CorgRmi.Logging;
using CorgRmi.Networking;
using CorgRmi.Networking.Interfaces;
using CorgRmi.Networking.InternalPackets;
using CorgRmi.RemoteConstructs.RemoteInstances;
using CorgRmi.Serialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi
{
    public static class CorgRmi
    {

        private volatile static bool isSetup = false;

        /// <summary>
        /// Try to join an instance using the net client provided.
        /// </summary>
        /// <param name="netClient"></param>
        /// <param name="joinedInstance"></param>
        /// <returns>Returns the created remote instance, or null if we could not join an instance</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static async Task<RemoteInstance?> TryJoinInstance(INetClient serverClient, double timeout, ILogger? logger = null)
        {
            // Check that we are ready
            CheckReady();
            // Create a loopback client
            INetClient loopbackClient = new LoopbackNetClient();
			// Create the remote instance
			RemoteInstance connectedInstance = new RemoteInstance(serverClient, loopbackClient);
            if (logger != null)
                connectedInstance.WithLogger(logger);
			// Send join instance messages through the net click
			connectedInstance.Host.SendPacket<RequestConnectionPacket, Void>(Void.Unit);
            // Wait for the response
            double timeleft = timeout;
            DateTime startTime = DateTime.Now;
            while (timeleft > 0)
			{
                // Get the packet
				NetPacket? recievedPacket = await connectedInstance.Host.AwaitPacket(timeleft);
                // Reduce the awaited time.
                timeleft -= (DateTime.Now - startTime).Milliseconds;
                // Break if null, timeout expired
                if (recievedPacket == null)
                    break;
                // Check if this is the packet we are looking for
                if (recievedPacket is ConnectionAcceptedPacket)
                {
                    // Accepted connection
                    return connectedInstance;
                }
			}
            // Failure to connect to an instance
            return null;
		}

        /// <summary>
        /// Start hosting an instance using the net click.
        /// </summary>
        public static RemoteInstance? HostInstance(INetHost networkingHost)
        {
            CheckReady();
            // Create and return the host instance
            return new RemoteInstance(networkingHost);
        }

        /// <summary>
        /// Check that CorgRMI is in a state where it is ready to be used.
        /// </summary>
        internal static void CheckReady()
        {
            // Quick check
            if (isSetup)
                return;
            // Acquire lock in cases of multi-threading
            lock (typeof(CorgRmi))
            {
                // Check setup conditions
                if (isSetup)
                    return;
                isSetup = true;
                // Get all the assemblies that are required for execution
                Assembly[] assembliesLoaded = AppDomain.CurrentDomain.GetAssemblies();
                // Perform setup tasks
                ObjectIdentifierGenerator.CreateNetworkedIDs(assembliesLoaded);
            }
        }

    }
}
