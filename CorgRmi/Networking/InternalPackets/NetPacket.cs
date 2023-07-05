using CorgRmi.RemoteConstructs.RemoteInstances;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking.InternalPackets
{
	internal abstract class NetPacket
	{

		[Pure]
		public abstract void Recieve(RemoteInstance instance, BinaryReader reader);

	}

	internal abstract class NetPacket<T> : NetPacket
	{

		public abstract byte[] ConvertToBytes(T packetInformation);

	}
}
