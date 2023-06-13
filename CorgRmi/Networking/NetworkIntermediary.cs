using CorgRmi.Networking.Interfaces;
using CorgRmi.Networking.InternalPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking
{
	/// <summary>
	/// Works between INetClient and transfering the bytes to the high level
	/// packets.
	/// </summary>
	internal class NetworkIntermediary
	{

		public struct FailureReason
		{
			public string? Reason { get; }

			public static FailureReason Success { get; } = new FailureReason();

			public FailureReason()
			{
				Reason = null;
			}

			public FailureReason(string? reason)
			{
				Reason = reason;
			}
		}

		/// <summary>
		/// This should be the same on both ends.
		/// </summary>
		private static NetPacket[] Singletons { get; } = typeof(NetPacket).Assembly
			.GetTypes()
			.Where(x => !x.IsAbstract && typeof(NetPacket).IsAssignableFrom(x))
			.OrderBy(x => x.Name)
			.Select(x => (NetPacket?)x.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public, new Type[0])?.Invoke(new object[0]))
			.Where(x => x != null)
			.Cast<NetPacket>()
			.ToArray();

		public NetworkIntermediary(INetClient netClient)
		{
			netClient.Recieve += HandleDataRecieve;
		}

		public void HandleDataRecieve(byte[] data, int start, int length)
		{
			BinaryReader reader = new BinaryReader(new MemoryStream(data, start, length));
			// First byte is the packet identifier
			byte packetIdentifier = reader.ReadByte();
			// Identifiy the length of the packet
			int packetLength = reader.ReadInt32();
			// Check against buffer overflows
			if (packetLength + sizeof(byte) + sizeof(int) > length)
				throw new IndexOutOfRangeException($"Attempting to access {packetLength + sizeof(byte) + sizeof(int)} bytes of data, when only {length} bytes have been authorised for use.");
			// Get the desired packet singleton and feed data into it
			if (packetIdentifier > Singletons.Length)
				throw new Exception($"Invalid packet identifier, packet identifier {packetIdentifier} does not exist.");
			// Handle the packet
			BinaryReader packetReader = new BinaryReader(new MemoryStream(data, start + sizeof(byte) + sizeof(int), packetLength));
			Singletons[packetIdentifier].Recieve(packetReader);
		}

	}
}
