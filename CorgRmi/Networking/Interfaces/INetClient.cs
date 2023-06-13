using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Networking.Interfaces
{
    /// <summary>
    /// Wrapper for network clients in order to stub them during testing.
    /// When steam integration is added, this will wrap around the steam library.
    /// </summary>
    public interface INetClient
    {

        /// <summary>
        /// Action called when information is recieved.
        /// </summary>
        event Action<byte[], int, int>? Recieve;

        /// <summary>
        /// Send data to a remote target.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        void Send(byte[] data, int start, int length);

    }
}
