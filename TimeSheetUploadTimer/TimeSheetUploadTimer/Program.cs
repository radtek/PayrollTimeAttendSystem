using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace TimeSheetUploadTimer
{
    class Program
    {
        static System.Timers.Timer myTimer;
        //5 Minutes
        static string strMilliSeconds = "10";

        static void Main(string[] args)
        {
            FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "TimerMilliSeconds.txt");

            if (fiFileInfo.Exists == true)
            {
                StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "TimerInSeconds.txt");
                strMilliSeconds = srStreamReader.ReadLine();

                srStreamReader.Close();
            }

            myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            myTimer.Interval = 100;
            myTimer.Enabled = true;
           
            //Loop Forever 
            for (;;)
            {
            }
        }

        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("In TimerCallback: " + DateTime.Now);

            myTimer.Enabled = false;

            if (myTimer.Interval != Convert.ToInt32(strMilliSeconds) * 1000)
            {
                myTimer.Interval = Convert.ToInt32(strMilliSeconds) * 1000;
            }
          
            GC.Collect();

            myTimer.Enabled = true;
        } 
    }
}
