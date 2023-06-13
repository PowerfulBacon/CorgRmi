using CorgRmi.RemoteConstructs.RemoteInstances;
using CorgRmi.RemoteConstructs.RemoteTargets;
using CorgRmi.RemoteObjects;
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
		public class SerialisedClass : RemoteObject
		{

			[CorgSerialise]
			public int TestNumber { get; set; }

			[CorgSerialise]
			public string? TestString { get; set; }

			[CorgSerialise]
			public bool testBool;

			[CorgSerialise]
			public SerialisedClass? testObject;

			public SerialisedClass(RemoteInstance instance, RemoteTarget? initialOwner = null) : base(instance, initialOwner)
			{ }

		}

		[TestMethod]
		public void TestSerialisationConsistency()
		{
			// Create a remote instance for simulation
			(RemoteInstance, RemoteInstance) remoteInstances = LocalInstanceFactory.CreateLocalInstancePair();
			RemoteInstance localInstanceHost = remoteInstances.Item1;
			RemoteInstance localInstanceClient = remoteInstances.Item2;
			// Create the classes
			SerialisedClass embeddedObject = new SerialisedClass(localInstanceHost);
			embeddedObject.TestNumber = 1;
			embeddedObject.TestString = "B";
			embeddedObject.testBool = true;
			SerialisedClass topLevelObject = new SerialisedClass(localInstanceHost);
			topLevelObject.TestNumber = 2;
			topLevelObject.TestString = "A";
			topLevelObject.testBool = false;
			// Create a cyclic dependency
			embeddedObject.testObject = topLevelObject;
			topLevelObject.testObject = embeddedObject;
			// Create the serialiser
		}

	}
}
