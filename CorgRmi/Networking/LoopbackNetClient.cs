using CorgRmi.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking
{
	internal class LoopbackNetClient : INetClient
	{

		public event Action<byte[], int, int>? Recieve;

		public void Send(byte[] data, int start, int length)
		{
			Recieve?.Invoke(data, start, length);
		}

	}
}
