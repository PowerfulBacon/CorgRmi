using CorgRmi.RemoteConstructs.RemoteInstances;
using CorgRmi.RemoteConstructs.RemoteTargets;
using CorgRmi.RemoteObjects;
using CorgRmi.Serialisation;
using CorgRmi.Serialisation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Tests
{
    [TestClass]
	public class SerialisationTests
	{


		[CorgSerialise]
		public class ComplexSerialisedClass : RemoteObject
		{

			[CorgSerialise]
			public int TestNumber { get; set; }

			[CorgSerialise]
			public string? TestString { get; set; }

			[CorgSerialise]
			public bool testBool;

			[CorgSerialise]
			public ComplexSerialisedClass? testObject;

			public ComplexSerialisedClass(RemoteInstance instance, RemoteTarget? initialOwner = null) : base(instance, initialOwner)
			{ }

		}

		/// <summary>
		/// Test that basic serialisation works.
		/// </summary>
		/// <param name="source"></param>
		[DataTestMethod]
		[DataRow(10, DisplayName = "Int Serialisation")]
		[DataRow('a', DisplayName = "Char Serialisation")]
		[DataRow((byte)130, DisplayName = "Byte Serialisation")]
		[DataRow((uint)16, DisplayName = "Uint Serialisation")]
		[DataRow(1.8f, DisplayName = "Float Serialisation")]
		[DataRow(100.83732, DisplayName = "Double Serialisation")]
		[DataRow("Hello There", DisplayName = "String Serialisation")]
		[DataRow(false, DisplayName = "String Serialisation")]
		[DataRow((long)100040040, DisplayName = "Long Serialisation")]
		[DataRow((short)1, DisplayName = "Short Serialisation")]
		public void TestPrimitiveSerialisation(object source)
		{
			int serialisationLength = NetSerialiser.GetSerialiastionLength(source.GetType(), source);
			byte[] serialisationStream = new byte[serialisationLength];
			using (BinaryWriter writer = new BinaryWriter(new MemoryStream(serialisationStream)))
			{
				NetSerialiser.Serialise(typeof(int), source, writer);
			}
			// Deserialise
			using (BinaryReader reader = new BinaryReader(new MemoryStream(serialisationStream)))
			{
				object? deserialisedObject = NetSerialiser.Deserialise(source.GetType(), reader);
				Assert.AreEqual(source, deserialisedObject, "Expected deserialised entity to be equal to the original entity.");
			}
		}

		/// <summary>
		/// Test that a class is successfully serialised and deserialised successfully.
		/// </summary>
		/// <exception cref="Exception"></exception>
		[TestMethod]
		public void TestSerialisationConsistency()
		{
			// Create a remote instance for simulation
			(RemoteInstance, RemoteInstance) remoteInstances = LocalInstanceFactory.CreateLocalInstancePair();
			RemoteInstance localInstanceHost = remoteInstances.Item1;
			RemoteInstance localInstanceClient = remoteInstances.Item2;
			// Create the classes
			ComplexSerialisedClass embeddedObject = new ComplexSerialisedClass(localInstanceHost)
			{
				TestNumber = 1,
				TestString = "B",
				testBool = true
			};
			ComplexSerialisedClass topLevelObject = new ComplexSerialisedClass(localInstanceHost)
			{
				TestNumber = 2,
				TestString = "A",
				testBool = false,
			};
			// Create a cyclic dependency
			embeddedObject.testObject = topLevelObject;
			topLevelObject.testObject = embeddedObject;
			embeddedObject.RemoteInitialise();
			topLevelObject.RemoteInitialise();
			// Get the object on both
			ComplexSerialisedClass? clientObject = (ComplexSerialisedClass?)localInstanceClient.GetObjectByID(topLevelObject.ID ?? throw new Exception());
			if (clientObject == null)
				Assert.Fail("Client did not recieve any remote object.");
			Assert.AreEqual(topLevelObject.TestNumber, clientObject.TestNumber);
			Assert.AreEqual(topLevelObject.TestString, clientObject.TestString);
			Assert.AreEqual(topLevelObject.testBool, clientObject.testBool);
			Assert.AreEqual(topLevelObject.testObject.TestNumber, embeddedObject.TestNumber);
			Assert.AreEqual(topLevelObject.testObject.TestString, embeddedObject.TestString);
			Assert.AreEqual(topLevelObject.testObject.testBool, embeddedObject.testBool);
		}

	}
}
