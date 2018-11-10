using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace InteractPayrollClient
{
    public class clsRestartFingerPrintClockTimeAttendanceService
    {
        Assembly asAssembly;
        System.Type typObjectType;
        object busDynamicService;

        HttpClient HttpClient;
        public Uri UriHeader;
        Uri UriRequest;
        System.ServiceModel.Channels.Message respondMessage;

        private object pvtReturnObject;
        private string pvtstrBusinessObjectName = "";

        public bool pubblnErrorHasBeenHandled = false;

        public clsRestartFingerPrintClockTimeAttendanceService(string BusinessObjectName)
		{
            pvtstrBusinessObjectName = BusinessObjectName;

            if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() == "")
            {
#if(DEBUG)
                asAssembly = Assembly.LoadFrom(BusinessObjectName + ".dll");
                typObjectType = asAssembly.GetType("FingerPrintClockServer." + BusinessObjectName);
                busDynamicService = Activator.CreateInstance(typObjectType);
#else
                UriHeader = new Uri("http://" + AppDomain.CurrentDomain.GetData("URLClientPath").ToString() + "/FingerPrintClockServerStartStop/");

                HttpClient = new HttpClient(UriHeader, true);
#endif
            }
            else
            {
                UriHeader = new Uri("http://" + AppDomain.CurrentDomain.GetData("URLClientPath").ToString() + "/FingerPrintClockServerStartStop/");

                HttpClient = new HttpClient(UriHeader, true);
            }
        }

        public object DynamicFunction(string FunctionName, object[] objParm)
        {
            try
            {
                pvtReturnObject = null;
#if(DEBUG)
                if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() == "")
                {
                    MethodInfo mi = typObjectType.GetMethod(FunctionName);
                    pvtReturnObject = mi.Invoke(busDynamicService, objParm);

                    goto DynamicFunction_Continue;
                }
#endif
                DateTime dtTimeWait = DateTime.Now.AddMilliseconds(500);

                UriRequest = new Uri(UriHeader, FunctionName);

                respondMessage = null;
                respondMessage = HttpClient.Get(UriRequest);
                
                while (DateTime.Now < dtTimeWait)
                {
                    System.Threading.Thread.Sleep(50);
                }

                if (HttpClient.blnConnectionFailure == true
                | HttpClient.blnTimeoutFailure == true
                | HttpClient.blnOtherFailure == true)
                {
                    if (HttpClient.blnConnectionFailure == true)
                    {
                        pubblnErrorHasBeenHandled = true;
                    }
                    else
                    {
                        if (HttpClient.blnTimeoutFailure == true)
                        {
                            pubblnErrorHasBeenHandled = true;
                        }
                        else
                        {
                            pubblnErrorHasBeenHandled = true;
                        }
                    }
                }
                else
                {
                    Restart myReturnObject = respondMessage.GetBody<Restart>();

                    pvtReturnObject = myReturnObject;
                }

                return pvtReturnObject;
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Communication Error") > -1
                    | ex.Message.IndexOf("Timeout Error") > -1
                     | ex.Message.IndexOf("Bad Request") > -1)
                {
                    throw ex;
                }
                else
                {
                    if (ex.Message.IndexOf("has exceeded the allotted timeout") > -1)
                    {
                        pubblnErrorHasBeenHandled = true;
                    }
                    else
                    {
                        if (ex.Message.IndexOf("was no endpoint listening") > -1)
                        {
                            pubblnErrorHasBeenHandled = true;
                        }
                        else
                        {
                            pubblnErrorHasBeenHandled = true;
                        }
                    }
                }
            }

        DynamicFunction_Continue:

            return pvtReturnObject;
        }
    }
}
