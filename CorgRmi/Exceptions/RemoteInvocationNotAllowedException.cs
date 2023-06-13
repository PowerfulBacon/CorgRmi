using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Exceptions
{
    public class RemoteInvocationNotAllowedException : Exception
    {
        public RemoteInvocationNotAllowedException()
            : base("Attempting to invoke a remote invocation without the required authorisation.")
        { }

    }
}
