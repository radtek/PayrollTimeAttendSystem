using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValiditeClockingsService
{
    static class Program
    {
        static void Main()
        {
#if (DEBUG)
            ValiditeClockingsService ValiditeClockingsService = new ValiditeClockingsService();
            ValiditeClockingsService.TestClockingService();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ValiditeClockingsService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
