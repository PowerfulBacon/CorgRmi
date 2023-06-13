using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.RemoteConstructs.RemoteObjects
{
	public enum DefaultVisibility
	{
		/// <summary>
		/// Indicates that a remote object is always visible to all remote
		/// targets on the server.
		/// </summary>
		ALWAYS_VISIBLE,
		/// <summary>
		/// When a remote object is created, it will only be visible to the
		/// host and the person who owns it. In order for it to become
		/// visible to other clients on the server, methods will have to be
		/// called to explicitly tell it to become visible.
		/// </summary>
		MANUAL_VISIBILITY,
	}
}
