using CorgRmi.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteConstructs.RemoteInstances
{
    public static class LocalInstanceFactory
    {

        /// <summary>
        /// Create a pair of local instances that are connected to each other
        /// </summary>
        /// <returns></returns>
        public static (RemoteInstance, RemoteInstance) CreateLocalInstancePair()
        {
			LocalNetClient selfClient = new LocalNetClient();
            LocalNetClient otherClient = new LocalNetClient(selfClient);

            RemoteInstance hostInstance = new RemoteInstance(selfClient, selfClient);
            hostInstance.AddConnection(otherClient);
			RemoteInstance clientInstance = new RemoteInstance(otherClient, otherClient);
			clientInstance.AddConnection(selfClient);

			return (hostInstance, clientInstance);
        }

    }
}
