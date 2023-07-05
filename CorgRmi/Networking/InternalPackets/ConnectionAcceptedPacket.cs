using CorgRmi.RemoteConstructs.RemoteInstances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking.InternalPackets
{
	internal class ConnectionAcceptedPacket : NetPacket<bool>
	{

		public override byte[] ConvertToBytes(bool packetInformation)
		{
			return new byte[0];
		}

		public override void Recieve(RemoteInstance instance, BinaryReader reader)
		{
			// Do nothing for now
		}

	}
}
