﻿using CorgRmi.Networking;
using CorgRmi.Networking.Interfaces;
using CorgRmi.Networking.InternalPackets;
using CorgRmi.RemoteConstructs.RemoteInstances;
using CorgRmi.RemoteConstructs.RemoteObjects.Procedures;
using CorgRmi.RemoteObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteConstructs.RemoteTargets
{
    public class RemoteTarget
    {

        /// <summary>
        /// The remote instance that this target is associated with.
        /// </summary>
        public RemoteInstance Instance { get; }

        /// <summary>
        /// A list of all the clients that are visible to us.
        /// </summary>
        private ConcurrentBag<RemoteTarget> VisibleClients { get; } = new ConcurrentBag<RemoteTarget>();

        /// <summary>
        /// Returns the number of clients that are visible to this remote target.
        /// </summary>
        public int VisibleClientCount => VisibleClients.Count;

        /// <summary>
        /// The net client that we use to send data messages to this remote target.
        /// </summary>
        internal NetworkIntermediary netClient;

		/// <summary>
		/// Remote targets should only be created by using internal helper functions
		/// on the remote instance.
		/// </summary>
		/// <param name="instance"></param>
		internal RemoteTarget(RemoteInstance instance, INetClient client)
        {
            Instance = instance;
            netClient = new NetworkIntermediary(client, instance);
            instance.Clients.Add(this);
        }
        /// <summary>
        /// Gets the visible client list and acquires the monitor for it.
        /// This method must be inside of a using block.
        /// </summary>
        /// <returns></returns>
        public void GetVisibleClients(Action<ConcurrentBag<RemoteTarget>> action)
        {
            lock (VisibleClients)
            {
                action.Invoke(VisibleClients);
            }
        }

        /// <summary>
        /// Invoke a method on a specific remote object.
        /// </summary>
        internal void InternallyInvokeMethod(RemoteObject remoteObject, RpcInvocationDetails rpcInvocationDetails, params object?[] parameters)
        {

        }

        /// <summary>
        /// Send a packet to this remote target
        /// </summary>
        /// <typeparam name="TPacket"></typeparam>
        /// <typeparam name="TPacketInfo"></typeparam>
        /// <param name="packetInfo"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SendPacket<TPacket, TPacketInfo>(TPacketInfo packetInfo)
            where TPacket : NetPacket<TPacketInfo>, new()
        {
            netClient.SendPacket<TPacket, TPacketInfo>(packetInfo);
		}

    }
}
