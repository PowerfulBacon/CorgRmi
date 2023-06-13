using CorgRmi.Serialisation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Variables
{

    public class NetVar
    {
        /// <summary>
        /// The identifier for this netvar.
        /// </summary>
        public uint Identifier { get; }
    }

    /// <summary>
    /// A networked variable with functionality for continuous serialisation, with
    /// networking prediction.
    /// Discrete variables are mapped with the Serialisable attribute and will be
    /// set recieving the object but will need to be updated by using method invocations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CorgSerialise]
    public class NetVar<T> : NetVar
    {



    }
}
