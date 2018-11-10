using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace FingerPrintClockServer
{
    [ServiceContract]
    public interface IFingerPrintClockService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/Ping")]
        Ping GetPing();

        [OperationContract]
        [WebGet(UriTemplate = "/PingDB")]
        PingDB GetPingDB();

        [OperationContract]
        [WebGet(UriTemplate = "/DynamicProcedure/{ObjectName}/{FunctionName}")]
        ReturnObject GetDynamicProcedure(string ObjectName, string FunctionName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/DynamicFunction/{ObjectName}/{FunctionName}")]
        ReturnObject GetDynamicFunction(string ObjectName, string FunctionName, ObjectParameters myObjectParameters);

        [OperationContract]
        [WebGet(UriTemplate = "/DeviceInfo/{DeviceNo}")]
        Device GetDeviceInfo(string DeviceNo);

        [OperationContract]
        [WebGet(UriTemplate = "/GetUserCompanies/{DBNo}")]
        UserCompany GetUserCompanies(string DBNo);
        
        //2017-05-13
        [OperationContract]
        [WebGet(UriTemplate = "/GetCurrentUserCompanies/{UserPinNo}")]
        UserCompany GetCurrentUserCompanies(string UserPinNo);
         
        //ELR 2014-08-16
        [OperationContract]
        [WebGet(UriTemplate = "/GetEmployeePinClocked/{DBNo}/{DeviceNo}/{InOutParm}/{BreakParm}/{EmployeeNo}/{Pin}")]
        EmployeeNameSurname GetEmployeePinClocked(string DBNo, string DeviceNo, string InOutParm, string BreakParm, string EmployeeNo, string Pin);

        [OperationContract]
        [WebGet(UriTemplate = "/DeleteEmployeeFingerprint/{DBNo}/{CompanyNo}/{EmployeeNo}/{FingerprintNo}")]
        DeletedFingerReply DeleteEmployeeFingerprint(string DBNo, string CompanyNo, string EmployeeNo, string FingerprintNo);

        [OperationContract]
        [WebGet(UriTemplate = "/DeleteUserFingerprint/{DBNo}/{UserNo}/{FingerprintNo}")]
        DeletedFingerReply DeleteUserFingerprint(string DBNo, string UserNo, string FingerprintNo);

        [OperationContract]
        [WebGet(UriTemplate = "/RfidToEmployee/{CompanyNo}/{EmployeeNo}/{CardNo}")]
        SaveReply SaveRfidToEmployee(string CompanyNo, string EmployeeNo, string CardNo);

        [OperationContract]
        [WebGet(UriTemplate = "/EmployeeDetails/{UserNo}/{EmployeeNo}/{PrevCompanyNo}/{Direction}")]
        EmployeeDetails GetEmployeeDetails(string UserNo, string EmployeeNo, string PrevCompanyNo, string Direction);

        [OperationContract]
        [WebGet(UriTemplate = "/CompanyEmployeeDetails/{DBNo}/{CompanyNo}")]
        CompanyEmployeeDetails GetCompanyEmployeeDetails(string DBNo, string CompanyNo);

        [OperationContract]
        [WebGet(UriTemplate = "/CompanyEmployeeDetailsNew/{DBNo}/{CompanyNo}")]
        CompanyEmployeeDetails GetCompanyEmployeeDetailsNew(string DBNo, string CompanyNo);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    UriTemplate = "/EnrolledUser/{UserNo}/{FingerNo}/{TemplateNo}")]
        EnrollFingerReply GetEnrolledUser(string UserNo, string FingerNo, string TemplateNo, ImageContainer ImageContainer);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    UriTemplate = "/EnrolledEmployee/{CompanyNo}/{EmployeeNo}/{FingerNo}/{TemplateNo}")]
        EnrollFingerReply GetEnrolledEmployee(string CompanyNo, string EmployeeNo, string FingerNo, string TemplateNo, ImageContainer ImageContainer);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    UriTemplate = "/User")]
        UserReply GetUser(ImageContainer ImageContainer);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    UriTemplate = "/UserFeaturesClocked/{DBNo}")]
        UserCompany GetUserFeaturesClocked(string DBNo, ImageFeaturesContainer ImageFeaturesContainer);
        
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/UserFeaturesClockedNew/{DBNo}")]
        UserCompany GetUserFeaturesClockedNew(string DBNo, ImageFeaturesContainer ImageFeaturesContainer);
        
        [OperationContract]
        [WebGet(UriTemplate = "/UserDetails/{UserNo}")]
        UserDetails GetUserDetails(string UserNo);
        
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/EmployeeClocked/{DeviceNo}/{ReaderNo}/{InOutParm}/{EmployeeNo}/{RfIdNo}")]
        EmployeeNames GetEmployeeClocked(string DeviceNo, string ReaderNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageContainer ImageByteArray);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/EmployeeClockedNew/{DeviceNo}/{InOutParm}/{EmployeeNo}/{RfIdNo}")]
        EmployeeNames GetEmployeeClockedNew(string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageContainer ImageByteArray);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/EmployeeBreakClocked/{DeviceNo}/{ReaderNo}/{InOutParm}/{EmployeeNo}/{RfIdNo}")]
        EmployeeNames GetEmployeeBreakClocked(string DeviceNo, string ReaderNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageContainer ImageByteArray);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/EmployeeBreakClockedNew/{DeviceNo}/{InOutParm}/{EmployeeNo}/{RfIdNo}")]
        EmployeeNames GetEmployeeBreakClockedNew(string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageContainer ImageByteArray);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/EmployeeFeaturesClocked/{DBNo}/{DeviceNo}/{InOutParm}/{EmployeeNo}/{RfIdNo}")]
        EmployeeNameSurname GetEmployeeFeaturesClocked(string DBNo, string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageFeaturesContainer ImageFeaturesByteArray);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/EmployeeFeaturesClockedNew/{DeviceNo}/{InOutParm}/{EmployeeNo}/{RfIdNo}")]
        EmployeeNameSurname GetEmployeeFeaturesClockedNew(string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageFeaturesContainer ImageFeaturesByteArray);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/EmployeeFeaturesBreakClocked/{DBNo}/{DeviceNo}/{InOutParm}/{EmployeeNo}/{RfIdNo}")]
        EmployeeNameSurname GetEmployeeFeaturesBreakClocked(string DBNo, string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageFeaturesContainer ImageFeaturesByteArray);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/EmployeeFeaturesBreakClockedNew/{DeviceNo}/{InOutParm}/{EmployeeNo}/{RfIdNo}")]
        EmployeeNameSurname GetEmployeeFeaturesBreakClockedNew(string DeviceNo, string InOutParm, string EmployeeNo, string RfIdNo, ImageFeaturesContainer ImageFeaturesByteArray);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "/InsertDPTemplate/{DBNo}/{CompanyNo}/{EmployeeNo}/{FingerNo}/{TemplateNo}")]
        EnrollTemplateContainerReply InsertDPTemplate(string DBNo, string CompanyNo, string EmployeeNo, string FingerNo, string TemplateNo, FeaturesContainer DPFeaturesByteArray);

        [OperationContract]
        [WebInvoke(Method = "POST",
                  UriTemplate = "/InsertUserDPTemplate/{DBNo}/{UserNo}/{FingerNo}/{TemplateNo}")]
        EnrollTemplateContainerReply InsertUserDPTemplate(string DBNo, string UserNo, string FingerNo, string TemplateNo, FeaturesContainer DPFeaturesByteArray);

        [OperationContract]
        [WebGet(UriTemplate = "/GetNewDeviceInfo/{DBNo}/{DeviceNo}/{Online}")]
        DeviceNewInfo GetNewDeviceInfo(string DBNo, string DeviceNo, string Online);

        [OperationContract]
        [WebGet(UriTemplate = "/GetDeviceInfoNew/{DeviceNo}")]
        DeviceNewInfo GetDeviceInfoNew(string DeviceNo);
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class Ping
    {
        [DataMember]
        public string OK;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class PingDB
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

    [DataContract(Namespace = "http://tempuri.org/TimeAttendanceAndClockServer")]
    public class ReturnObject
    {
        [DataMember]
        public object obj;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class EmployeeDetails
    {
        [DataMember]
        public string OK;

        [DataMember]
        public string CompanyNo;

        [DataMember]
        public string EmployeeNo;

        [DataMember]
        public string EmployeeCode;

        [DataMember]
        public string EmployeeNames;

        [DataMember]
        public string RecordCount;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class CompanyEmployeeDetails
    {
        [DataMember]
        public string OK;
        [DataMember]
        public string EmployeeNo;
        [DataMember]
        public string EmployeeCode;
        [DataMember]
        public string EmployeeName;
        [DataMember]
        public string EmployeeSurname;
        [DataMember]
        public string EmployeeFinger;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class UserDetails
    {
        [DataMember]
        public string OK;

        [DataMember]
        public string UserNames;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class SaveReply
    {
        [DataMember]
        public string OK;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class Device
    {
        [DataMember]
        public string FoundInfo;

        [DataMember]
        public string DeviceDesc;

        [DataMember]
        public string DeviceUsage;

        [DataMember]
        public string ClockInOutParm;

        [DataMember]
        public string ClockInRangeFrom;

        [DataMember]
        public string ClockInRangeTo;

        [DataMember]
        public string DateTime;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class UserCompany
    {
        [DataMember]
        public string UserNoLoggedIn;
        [DataMember]
        public string CompanyDesc;
        [DataMember]
        public string CompanyNo;
        [DataMember]
        public string UserNo;
        [DataMember]
        public string UserName;
        [DataMember]
        public string UserSurname;
        [DataMember]
        public string UserFinger;
        [DataMember]
        public string EmployeeNo;
        [DataMember]
        public string EmployeeCode;
        [DataMember]
        public string EmployeeName;
        [DataMember]
        public string EmployeeSurname;
        [DataMember]
        public string EmployeeFinger;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class EmployeeNameSurname
    {
        [DataMember]
        public string OkInd;

        [DataMember]
        public string Name;

        [DataMember]
        public string Surname;

        [DataMember]
        public string EmployeeNo;

        [DataMember]
        public string FingerPrintScore;

        [DataMember]
        public string RfIdNo;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class EmployeeNames
    {
        [DataMember]
        public string OkInd;

        [DataMember]
        public string ReaderNo;

        [DataMember]
        public string Names;

        [DataMember]
        public string EmployeeNo;

        [DataMember]
        public string FingerPrintScore;

        [DataMember]
        public string RfIdNo;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class UserReply
    {
        [DataMember]
        public string OkInd;

        [DataMember]
        public string UserNo;
    }
    
    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class CurrentUser
    {
        [DataMember]
        public string UserNo;
    }
    
    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class ImageContainer
    {
        [DataMember]
        public byte[] DPImageByteArray;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class ImageFeaturesContainer
    {
        [DataMember]
        public byte[] DPImageFeaturesByteArray;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class FeaturesContainer
    {
        //NB Up to 1.5k
        [DataMember]
        public byte[] DPFeaturesByteArray;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class EnrollTemplateContainerReply
    {
        [DataMember]
        public string OK;

        [DataMember]
        public string EnrollInd;

        [DataMember]
        public string EnrollQuality;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class EnrollFingerReply
    {
        [DataMember]
        public string OkInd;

        [DataMember]
        public string EnrollInd;

        [DataMember]
        public string EnrollQuality;

        [DataMember]
        public int VerifyFingerPrintsCompareScore;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class DeletedFingerReply
    {
        [DataMember]
        public string OkInd;
    }

    [DataContract(Namespace = "http://tempuri.org/FingerPrintClockServer")]
    public class DeviceNewInfo
    {
        [DataMember]
        public string FoundInfo;

        [DataMember]
        public string DeviceDesc;

        [DataMember]
        public string DeviceUsage;

        [DataMember]
        public string ClockInOutParm;

        [DataMember]
        public string ClockInRangeFrom;

        [DataMember]
        public string ClockInRangeTo;

        [DataMember]
        public string DateTime;

        [DataMember]
        public string LanInd;

        [DataMember]
        public string FingerEngine;
    }
}


