using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace InteractPayroll
{
    public class FeaturesContainer
    {
        //NB Up to 1.5k
        [DataMember]
        public byte[] DPFeaturesByteArray;
    }

}
