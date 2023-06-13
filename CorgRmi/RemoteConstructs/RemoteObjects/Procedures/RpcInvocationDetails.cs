using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteConstructs.RemoteObjects.Procedures
{
    internal class RpcInvocationDetails
    {

        public bool RequiresTarget { get; } = false;

        public CallFrom CallFrom { get; } = 0;

        public InvokeOn InvokeTarget { get; } = 0;

        /// <summary>
        /// Get and cache the invocation details for a particulare method.
        /// </summary>
        /// <param name="targetMethod">The method info to source the reflection information from.</param>
        public RpcInvocationDetails(MethodInfo targetMethod)
        {

        }

    }
}
