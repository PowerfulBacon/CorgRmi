using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking.Interfaces
{
	public interface INetHost
	{

		/// <summary>
		/// Event raised when a client is connected to this remote host.
		/// </summary>
		event Action<INetClient>? OnClientConnected;

	}
}
