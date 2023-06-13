using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteConstructs.RemoteObjects.Procedures
{
    /// <summary>
    /// Represents the set of individuals who are allowed to
    /// invoke this method/procedure.
    /// The default call from behaviour is both the host and the owner of the object.
    /// </summary>
    public enum CallFrom
    {
        /// <summary>
        /// The host of the networking session is allowed to invoke this method.
        /// </summary>
        Host = 1 << 0,
        /// <summary>
        /// The owner of the object associated with this object holding this method
        /// is allowed to invoke the method. If the method is static, then the host
        /// will always be the owner of the method.
        /// </summary>
        Owner = 1 << 1,
        /// <summary>
        /// All clients in the session are allowed to invoke this method.
        /// </summary>
        Anyone = 1 << 2,
    }
}
