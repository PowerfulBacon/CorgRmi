using CorgRmi.Networking.Interfaces;
using CorgRmi.Networking.InternalPackets;
using CorgRmi.RemoteConstructs.RemoteInstances;
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

		private class ScuffedIDHolder<T>
			where T : NetPacket
		{
			internal static byte PACKET_ID = 0;
		}

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

		private static byte staticID = 0;

		/// <summary>
		/// This should be the same on both ends.
		/// </summary>
		private static NetPacket[] Singletons { get; } = typeof(NetPacket).Assembly
			.GetTypes()
			.Where(x => !x.IsAbstract && typeof(NetPacket).IsAssignableFrom(x))
			.OrderBy(x => x.Name)
			.Select(Activator.CreateInstance)
			.Where(x => x != null)
			.Cast<NetPacket>()
			.Select(x => {
				typeof(ScuffedIDHolder<>).MakeGenericType(x.GetType()).GetField("PACKET_ID", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, staticID++);
				return x;
			})
			.ToArray();

		private RemoteInstance attachedInstance;

		private INetClient netClient;

		internal volatile NetPacket? lastRecieved = null;

		public NetworkIntermediary(INetClient netClient, RemoteInstance remoteInstance)
		{
			this.netClient = netClient;
			netClient.Recieve += HandleDataRecieve;
			attachedInstance = remoteInstance;
		}

		/// <summary>
		/// Send a packet to the remote target
		/// </summary>
		/// <typeparam name="TPacket"></typeparam>
		/// <typeparam name="TPacketInfo"></typeparam>
		/// <param name="packetInfo"></param>
		public void SendPacket<TPacket, TPacketInfo>(TPacketInfo packetInfo)
			where TPacket : NetPacket<TPacketInfo>, new()
		{
			// Get a byte stream
			byte[] data = new TPacket().ConvertToBytes(packetInfo);
			byte[] fullData = new byte[data.Length + sizeof(byte) + sizeof(int)];
			using (BinaryWriter writer = new BinaryWriter(new MemoryStream(fullData)))
			{
				// Get the packet ID
				writer.Write(ScuffedIDHolder<TPacket>.PACKET_ID);
				// Write the size of the data
				writer.Write(data.Length);
			}
			data.CopyTo(fullData, sizeof(byte) + sizeof(int));
			// Send the data
			netClient.Send(fullData, 0, fullData.Length);
			attachedInstance.logger?.LogMessage(this, $"Sent packet of length {fullData.Length}, packet of type {typeof(TPacket).Name}");
		}

		public void HandleDataRecieve(byte[] data, int start, int length)
		{
			attachedInstance.logger?.LogMessage(this, $"Recieved packet of length {length}, start {start}, array length {data.Length}");
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
			attachedInstance.logger?.LogMessage(this, $"Recieved packet of length {length}, type {Singletons[packetIdentifier].GetType().Name}");
			// Handle the packet
			BinaryReader packetReader = new BinaryReader(new MemoryStream(data, start + sizeof(byte) + sizeof(int), packetLength));
			Singletons[packetIdentifier].Recieve(attachedInstance, packetReader);
			lastRecieved = Singletons[packetIdentifier];
		}

	}
}
