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
		public abstract byte[] ConvertToBytes();

		[Pure]
		public abstract void Recieve(BinaryReader reader);

	}
}
