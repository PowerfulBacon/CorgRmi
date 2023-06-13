using CorgRmi.Exceptions;
using CorgRmi.RemoteConstructs.RemoteObjects.Procedures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteObjects
{
    public abstract partial class RemoteObject
    {

        /// <summary>
        /// Cache the invocation details for performance. Networking will be the biggest bottleneck, but we
        /// might as well make this fast.
        /// </summary>
        private ConcurrentDictionary<MethodInfo, RpcInvocationDetails> cachedInvocationDetails = new();

        /// <summary>
        /// Asynchronously invoke a method on the default targets.
        /// </summary>
        /// <param name="internalAction"></param>
        /// <param name="parameters"></param>
        private void InternallyInvokeAsync(MethodInfo methodDetails, params object?[] parameters)
        {
            RpcInvocationDetails invocationDetails;
            // Attempt to fetch the invocation details from the cache.
            if (!cachedInvocationDetails.TryGetValue(methodDetails, out invocationDetails!))
            {
                // Generate the cache of invocation details.
                // This might create 2 of the same invocation details in the event of a thread conflict,
                // however this is fine and the system can deal with this due to invocation details being
                // read only.
                invocationDetails = new RpcInvocationDetails(methodDetails);
                // Try and add the value to the cache.
                // If this fails, we don't really care since it will be the same across all threads.
                cachedInvocationDetails.TryAdd(methodDetails, invocationDetails);
            }
            // Verify if a target was required
            // If we required a target to call the method but didn't provide one, throw an exception
            if (invocationDetails.RequiresTarget)
                throw new RemoteInvocationNoTargetException(methodDetails);
            // Ensure that we are allowed to call this
            // We cannot call from anyone and...
            if ((invocationDetails.CallFrom & CallFrom.Anyone) == 0)
            {
                // ... we are not the owner but need to be the owner
                // and are not the host but can be the host.
                if (!IsOwner || (invocationDetails.CallFrom & CallFrom.Owner) == 0)
                    throw new RemoteInvocationNotAllowedException();
            }
            // Determine who we are attempting to target
            // and send the method to be handled by the generic RMI handler.
            // Handle simple cases
            switch (invocationDetails.InvokeTarget)
            {
                case InvokeOn.Host:
                    Owner.Instance.Host.InternallyInvokeMethod(this, invocationDetails, parameters);
                    return;
                case InvokeOn.Owner:
                    Owner.InternallyInvokeMethod(this, invocationDetails, parameters);
                    return;
                // Throw an exception as we require a target if one is not provided by default.
                case 0:
                    throw new RemoteInvocationNoTargetException(methodDetails);
            }
            // Handle cases where we have multiple invocation targets
            if ((invocationDetails.InvokeTarget & InvokeOn.AllAware) != 0)
            {
                // Send to all
                Owner.GetVisibleClients(clients => clients.ForEach(client => client.InternallyInvokeMethod(this, invocationDetails, parameters)));
                return;
            }
            // Invoke on both the host and the owner
            if ((invocationDetails.InvokeTarget & (InvokeOn.Host | InvokeOn.Owner)) == (InvokeOn.Host | InvokeOn.Owner))
            {
                // Check if the host and owner are the same person.
                if (Owner.Instance.Host != Owner)
                    Owner.Instance.Host.InternallyInvokeMethod(this, invocationDetails, parameters);
                Owner.InternallyInvokeMethod(this, invocationDetails, parameters);
            }
            // Throw an exception, we shouldn't hit this state.
            throw new ArgumentOutOfRangeException("Could not handle remote invocation due to the invocation call flags not being implemented.");
        }

        public void Invoke(Action targetAction) => InternallyInvokeAsync(targetAction.Method);

        public void Invoke<T>(Action<T> targetAction, T param1) => InternallyInvokeAsync(targetAction.Method, param1);

        public void Invoke<T1, T2>(Action<T1, T2> targetAction, T1 p1, T2 p2) => InternallyInvokeAsync(targetAction.Method, p1, p2);

        public void Invoke<T1, T2, T3>(Action<T1, T2, T3> targetAction, T1 p1, T2 p2, T3 p3) => InternallyInvokeAsync(targetAction.Method, p1, p2, p3);

        public void Invoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> targetAction, T1 p1, T2 p2, T3 p3, T4 p4) => InternallyInvokeAsync(targetAction.Method, p1, p2, p3, p4);

        public void Invoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> targetAction, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => InternallyInvokeAsync(targetAction.Method, p1, p2, p3, p4, p5);

        public void Invoke<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> targetAction, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => InternallyInvokeAsync(targetAction.Method, p1, p2, p3, p4, p5, p6);

    }
}
