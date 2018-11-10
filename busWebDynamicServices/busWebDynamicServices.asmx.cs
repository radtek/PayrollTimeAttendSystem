using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Services;

namespace InteractPayroll
{
    public class busWebDynamicServices : System.Web.Services.WebService
    {
        [WebMethod]
        public object DynamicFunction(string BusinessObjectName, string FunctionName, object[] objParm)
        {
            string strPath = System.Configuration.ConfigurationSettings.AppSettings["binPath"].ToString();

            Assembly asAssembly = Assembly.LoadFrom(strPath + BusinessObjectName + ".dll");
            System.Type typObjectType = asAssembly.GetType("InteractPayroll." + BusinessObjectName);
            object busDynamicProcedure = Activator.CreateInstance(typObjectType);

            MethodInfo mi = typObjectType.GetMethod(FunctionName);
            object objReturn = mi.Invoke(busDynamicProcedure, objParm);

            return objReturn;
        }

        [WebMethod]
        public string Ping()
        {
            return "Ping";
        }
    }
}