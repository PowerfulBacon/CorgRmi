using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Exceptions
{
    public class RemoteInvocationNoTargetException : Exception
    {

        public RemoteInvocationNoTargetException(MethodInfo targetMethod)
            : base($"Attempted to call the method {targetMethod.Name} at {targetMethod.DeclaringType?.Name ?? "N/A"} without an invocation target despite this object requiring a target.")
        {

        }

    }
}
