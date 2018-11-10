using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace InteractPayrollClient
{
    [DataContract(Namespace = "http://tempuri.org/TimeAttendanceAndClockServer")]
    public class ReturnObject
    {
        [DataMember]
        public object obj;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServerStartStop")]
    public class Restart
    {
        [DataMember]
        public string OK;
    }

    [DataContract(Namespace = "http://tempuri.org/TimeAttendanceAndClockServer")]
    public class ObjectParameters
    {
        [DataMember]
        public byte[] bytParameter;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServerStartStop")]
    public class RestartFingerPrintClockTimeAttendanceServiceNewResponse
    {
        [DataMember]
        public string FingerPrintClockTimeAttendanceServiceStartStopLoginUser;

        [DataMember]
        public string FingerPrintClockTimeAttendanceServiceLoginUser;

        [DataMember]
        public bool FingerPrintClockTimeAttendanceServiceStarted;
    }
}
