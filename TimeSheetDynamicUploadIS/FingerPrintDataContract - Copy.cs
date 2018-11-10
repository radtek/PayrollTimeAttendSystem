using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FingerPrintClockServer.FingerPrintClockServiceStartStop
{
    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServerStartStop")]
    public class Restart
    {
        [DataMember]
        public string OK;
    }
}
