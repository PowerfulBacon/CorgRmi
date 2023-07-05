using CorgRmi.Networking.InternalPackets;
using CorgRmi.RemoteConstructs.RemoteTargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking.Interfaces
{
	public static class NetClientExtensions
	{

		/// <summary>
		/// Waits until a netclient recieves a packet and then returns that returned packet.
		/// If the timeout expires, null will be returned instead.
		/// </summary>
		/// <param name="netClient"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		internal static async Task<NetPacket?> AwaitPacket(this RemoteTarget netClient, double timeout)
		{
			DateTime startTime = DateTime.Now;
			while ((DateTime.Now - startTime).TotalMilliseconds < timeout)
			{
				if (netClient.netClient.lastRecieved != null)
					return netClient.netClient.lastRecieved;
				// We need to ensure a context switch so that blazor has time to properly render
				// and do other tasks that want to be done.
				// Using await Task.Yield() doesn't ensure that we switch to other things.
				await Task.Delay(1);
			}
			return null;
		}

	}
}
