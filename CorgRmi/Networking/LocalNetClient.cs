using CorgRmi.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking
{
	internal class LocalNetClient : INetClient
	{

		private LocalNetClient? localNetClient;

		public LocalNetClient()
		{ }

		public LocalNetClient(LocalNetClient otherSide)
		{
			localNetClient = otherSide;
			otherSide.localNetClient = this;
		}

		public event Action<byte[], int, int>? Recieve;

		public void Send(byte[] data, int start, int length)
		{
			localNetClient?.Recieve?.Invoke(data, start, length);
		}

	}
}
