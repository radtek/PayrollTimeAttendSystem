using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractPayrollClient
{
    class clsFadeForm
    {
        public static void FadeForm(System.Windows.Forms.Form f)
        {
            int NumberOfSteps = 50;

            int StepVal = (int)(100f / NumberOfSteps);

            float fOpacity = 100f;

            for (int b = 0; b < NumberOfSteps; b++)
            {
                f.Opacity = fOpacity / 100;

                f.Refresh();

                fOpacity -= StepVal;

                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
