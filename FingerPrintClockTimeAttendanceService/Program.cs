using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FingerPrintClockServer
{
    static class Program
    {
        static void Main()
        {

#if (DEBUG)

            FingerPrintClockTimeAttendanceService fingerPrintClockTimeAttendanceService = new FingerPrintClockTimeAttendanceService();
            //NB OnStart(string[] args) Will be Fired in Debug Compilation Flags in Initialise Section 
#else

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new FingerPrintClockTimeAttendanceService() 
			};
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
