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
#if(DEBUG)

            FingerPrintClockTimeAttendanceServiceStartStop fingerPrintClockTimeAttendanceServiceStartStop = new FingerPrintClockTimeAttendanceServiceStartStop();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new FingerPrintClockTimeAttendanceServiceStartStop() 
			};
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
