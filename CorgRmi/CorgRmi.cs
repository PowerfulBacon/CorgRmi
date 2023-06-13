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

        public static bool TryJoinInstance(IPAddress address, int port, out RemoteInstance joinedInstance)
        {
            CheckReady();
            throw new NotImplementedException();
        }

        public static bool TryHostInstance(int port, out RemoteInstance hostedInstance)
        {
            CheckReady();
            throw new NotImplementedException();
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
