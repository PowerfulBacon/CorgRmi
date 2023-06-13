using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteConstructs.RemoteObjects.Procedures
{
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
    public enum InvokeOn
    {
        /// <summary>
        /// The method is allowed to be invoked on the host of the game.
        /// </summary>
        Host = 1 << 0,
        /// <summary>
        /// The method will be invoked on the owner of the object.
        /// </summary>
        Owner = 1 << 1,
        /// <summary>
        /// The method will be invoked on everyone who is aware of the object/procedure.
        /// </summary>
        AllAware = 1 << 2,
    }
}
