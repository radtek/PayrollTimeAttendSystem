using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace InteractPayroll
{
    class clsFadeForm
    {
        public static void FadeForm(System.Windows.Forms.Form f)
        {
            DateTime myDateTime = DateTime.Now;

            int NumberOfSteps = 100;

            int StepVal = (int)(100f / NumberOfSteps);

            float fOpacity = 100f;

            for (int b = 0; b < NumberOfSteps; b++)
            {
                f.Opacity = fOpacity / 100;

                f.Refresh();

                fOpacity -= StepVal;

                myDateTime = DateTime.Now.AddMilliseconds(25);

                while (DateTime.Now < myDateTime)
                {
                    Application.DoEvents();
                }
            }
        }
    }
}
