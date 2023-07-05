using CorgRmi.RemoteConstructs.RemoteInstances;
using CorgRmi.RemoteObjects;
using CorgRmi.Serialisation;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking.InternalPackets
{
	internal class CreateRemoteObjectPacket : NetPacket<RemoteObject>
	{

		public override byte[] ConvertToBytes(RemoteObject remoteObject)
		{
			int serialisationLength = NetSerialiser.GetSerialiastionLength(typeof(RemoteObject), remoteObject);
			byte[] serialiationBytes = new byte[serialisationLength];
			using (BinaryWriter writer = new BinaryWriter(new MemoryStream(serialiationBytes)))
			{
				NetSerialiser.Serialise(typeof(RemoteObject), remoteObject, writer);
			}
			return serialiationBytes;
		}

		[Pure]
		public override void Recieve(RemoteInstance instance, BinaryReader reader)
		{
			RemoteObject? deserialisedObject = NetSerialiser.Deserialise(typeof(RemoteObject), reader) as RemoteObject;
			// Invalid deserialisation
			if (deserialisedObject == null)
				return;
			// Register the object with our instance
			instance.RegisterRemoteObject(deserialisedObject, false);
		}

	}
}
