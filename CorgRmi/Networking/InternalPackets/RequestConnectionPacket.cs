using CorgRmi.RemoteConstructs.RemoteInstances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking.InternalPackets
{
	internal class RequestConnectionPacket : NetPacket<Void>
	{

		public override byte[] ConvertToBytes(Void packetInformation)
		{
			return new byte[0];
		}

		public override void Recieve(RemoteInstance instance, BinaryReader reader)
		{
			// Add a new client
			// Return connection successful
		}

	}
}
