using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Services;

namespace InteractPayroll
{
    public class busTest : System.Web.Services.WebService
    {
        public busTest()
        {
      
        }

        [WebMethod]
        public string Test()
        {
            return "Test";
        }
    }
}
