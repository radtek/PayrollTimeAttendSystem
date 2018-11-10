using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace DailyDBBackup
{
    static class Program
    {
        static void Main()
        {
#if (DEBUG)

            DailyDBBackup DailyDBBackup = new DailyDBBackup();
       
            DailyDBBackup.RunBackupJob();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new DailyDBBackup() 
			};
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
