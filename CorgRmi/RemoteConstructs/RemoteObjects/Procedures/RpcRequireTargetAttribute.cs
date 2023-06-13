using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteConstructs.RemoteObjects.Procedures
{
    /// <summary>
    /// Indicates that an RPC requires a target in order to be called.
    /// If this RPC is called with the default target behaviour, a runtime exception
    /// will be raised instead.
    /// </summary>
    public class RpcRequireTargetAttribute : Attribute
    {
    }
}
