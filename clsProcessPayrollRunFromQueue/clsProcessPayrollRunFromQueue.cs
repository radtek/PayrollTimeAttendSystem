using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractPayroll
{
    public class clsProcessPayrollRunFromQueue
    {
        busTimeAttendanceRun busTimeAttendanceRun;
        busClosePayrollRun busClosePayrollRun;
        busOpenPayrollRun busOpenPayrollRun;
        busRptPaySlip busRptPaySlip;
        
        public clsProcessPayrollRunFromQueue()
        {
            busTimeAttendanceRun = new busTimeAttendanceRun();
            busClosePayrollRun = new busClosePayrollRun();
            busOpenPayrollRun = new busOpenPayrollRun();
            busRptPaySlip = new busRptPaySlip();
        }

        public void ProcessPayrollRunFromQueue(RunQueue runQueue)
        {
            if (runQueue.PayCategoryType == "T")
            {
                busTimeAttendanceRun.Calculate_TimeAttendance_From_TimeSheets(runQueue.UserNo, runQueue.CompanyNo, runQueue.PayCategoryNumberArray, runQueue.PayCategoryType, runQueue.PayPeriodDate);
            }
            else
            {
                busTimeAttendanceRun.Calculate_Payroll_From_TimeSheets(runQueue.UserNo, runQueue.CompanyNo, runQueue.PayCategoryNumberArray, runQueue.PayCategoryType, runQueue.PayPeriodDate);
            }
        }

        public void ProcessCloseRunFromQueue(CloseQueue closeQueue)
        {
            if (closeQueue.RunType == "T")
            {
                busClosePayrollRun.Close_TimeAttendance_Run(closeQueue.CompanyNo, closeQueue.WagePayPeriodDate,  closeQueue.WagesPayCategoryNumberArray, closeQueue.UserNo);
            }
            else
            {
                if (closeQueue.RunType == "W")
                {
                    busClosePayrollRun.Close_Wage_Run(closeQueue.CompanyNo, closeQueue.WagePayPeriodDate, closeQueue.WagesPayCategoryNumberArray, closeQueue.UserNo);
                }
                else
                {
                    if (closeQueue.RunType == "S")
                    { 
                        busClosePayrollRun.Close_Salary_Run(closeQueue.CompanyNo, closeQueue.SalaryPayPeriodDate, closeQueue.SalariesPayCategoryNumberArray, closeQueue.UserNo);
                    }
                    else
                    {
                        //Both
                        busClosePayrollRun.Close_Both_Run(closeQueue.CompanyNo, closeQueue.WagePayPeriodDate, closeQueue.SalaryPayPeriodDate, closeQueue.WagesPayCategoryNumberArray, closeQueue.SalariesPayCategoryNumberArray, closeQueue.UserNo);
                    }
                }
            }
        }


        public void ProcessOpenRunFromQueue(OpenRunQueue openRunQueue)
        {
            if (openRunQueue.PayCategoryType == "W")
            {
                busOpenPayrollRun.Insert_Wage_Run_Records(openRunQueue.CompanyNo, openRunQueue.PayPeriodDate, openRunQueue.PayCategoryNumberArray);
            }
            else
            {
                if (openRunQueue.PayCategoryType == "S")
                {
                    busOpenPayrollRun.Insert_Salary_Run_Records(openRunQueue.CompanyNo, openRunQueue.PayPeriodDate, openRunQueue.PayCategoryNumberArray);

                }
                else
                {
                    busOpenPayrollRun.Insert_TimeAttendance_Run_Records(openRunQueue.CompanyNo, openRunQueue.PayPeriodDate, openRunQueue.PayCategoryNumberArray);
                }
            }
        }
        
        public void ProcessEmailPayslipFromQueue(EmailPayslipQueue emailPayslipQueue)
        {
            busRptPaySlip.Email_Payslips(emailPayslipQueue.CompanyNo, emailPayslipQueue.UserNo,  emailPayslipQueue.PayPeriodDate, emailPayslipQueue.PayslipEmailQueueNo);
        }
    }
}
