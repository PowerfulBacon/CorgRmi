using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteConstructs.RemoteObjects.Procedures
{
    /// <summary>
    /// Attribute that indicates that a method can be invoked via
    /// RPC/RMI calls.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcAttribute : Attribute
    {

        /// <summary>
        /// Represents the set of individuals who are allowed to
        /// invoke this method/procedure.
        /// The default call from behaviour is both the host and the owner of the object.
        /// </summary>
        public CallFrom CallFrom { get; } = CallFrom.Host | CallFrom.Owner;

        /// <summary>
        /// When an RPC/RMI is invoked, it will be invoked on targets determined by these flags.
        /// These specify both the default behaviour for Invoke(), as well as who is allowed to invoke
        /// methods for security reasons.
        /// When Invoke() is called with the default targets, it will be sent to all respresented within
        /// the InvokeOn group.
        /// If a member is present in multiple groups, it will only be called on them once.
        /// 
        /// The default behaviour is to only invoke methods on the owner of the objects.
        /// </summary>
        public InvokeOn InvokeOn { get; } = InvokeOn.Owner;

        public RpcAttribute(CallFrom callFrom)
        {
            CallFrom = callFrom;
        }

        public RpcAttribute(CallFrom callFrom, InvokeOn invokeOn)
        {
            CallFrom = callFrom;
            InvokeOn = invokeOn;
        }
    }

}
