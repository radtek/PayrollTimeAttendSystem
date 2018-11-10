using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractPayroll
{
    public class RunQueue
    {
        public Int64 UserNo { get; set; }
        public Int64 CompanyNo { get; set; }
        public string PayCategoryType { get; set; }
        public string PayCategoryNumberArray { get; set; }
        public DateTime PayPeriodDate { get; set; }
    }

    public class CloseQueue
    {
        public Int64 UserNo { get; set; }
        public Int64 CompanyNo { get; set; }
        public string RunType { get; set; }
        public string WagesPayCategoryNumberArray { get; set; }
        public string SalariesPayCategoryNumberArray { get; set; }
        public DateTime WagePayPeriodDate { get; set; }
        public DateTime SalaryPayPeriodDate { get; set; }
    }

    public class OpenRunQueue
    {
        public Int64 UserNo { get; set; }
        public Int64 CompanyNo { get; set; }
        public string PayCategoryType { get; set; }
        public string PayCategoryNumberArray { get; set; }
        public DateTime PayPeriodDate { get; set; }
    }
    
    public class EmailPayslipQueue
    {
        public Int64 CompanyNo { get; set; }
        public Int64 UserNo { get; set; }
        public DateTime PayPeriodDate { get; set; }
        public int PayslipEmailQueueNo { get; set; }
    }
}