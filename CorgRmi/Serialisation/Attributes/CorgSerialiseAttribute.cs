using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Serialisation.Attributes
{
    /// <summary>
    /// Indicates that a field of an object will be serialised along with that object during networking.
    /// Indicates that an object can be transfered over the network, which will assign it a network ID. And class
    /// that does not have this attribute will not be able to be communicated with other users.
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Property)]
    [MeansImplicitUse]
    public class CorgSerialiseAttribute : Attribute
    {

        public CorgSerialiseAttribute()
        {

        }

    }

}
