using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace FingerPrintClockServer
{
    [ServiceContract]
    public interface IFingerPrintClockServiceStartStop
    {
        [OperationContract]
        [WebGet(UriTemplate = "/Ping")]
        Ping GetPing();

        [OperationContract]
        [WebGet(UriTemplate = "/RestartFingerPrintClockTimeAttendanceService")]
        Restart RestartFingerPrintClockTimeAttendanceService();

        [OperationContract]
        [WebGet(UriTemplate = "/RestartFingerPrintClockTimeAttendanceServiceNew")]
        RestartFingerPrintClockTimeAttendanceServiceNewResponse RestartFingerPrintClockTimeAttendanceServiceNew();
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServerStartStop")]
    public class Ping
    {
        [DataMember]
        public string OK;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServerStartStop")]
    public class Restart
    {
        [DataMember]
        public string OK;
    }


    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServerStartStop")]
    public class ServiceStartResponse
    {
        [DataMember]
        public string ServiceName;

        [DataMember]
        public string ServiceLoginUser;

        [DataMember]
        public bool ServiceInstalled;

        [DataMember]
        public bool ServiceEnabled;

        [DataMember]
        public bool ServiceStarted;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServerStartStop")]
    public class GetServiceInformationResponse
    {
        [DataMember]
        public string ServiceName;

        [DataMember]
        public string ServiceLoginUser;

        [DataMember]
        public bool ServiceInstalled;

        [DataMember]
        public bool ServiceEnabled;

        [DataMember]
        public bool ServiceStarted;
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



