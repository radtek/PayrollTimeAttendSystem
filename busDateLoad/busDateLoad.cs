using System;
using System.Collections.Generic;
using System.Text;

namespace InteractPayroll
{
    public class busDateLoad
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busDateLoad()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public void Insert_Dates(string parstrType)
        {
            string strQry;
            DateTime dtDateTimeStart;
            DateTime dtDateTimeEnd;

            if (parstrType == "N")
            {
                dtDateTimeStart = new DateTime(DateTime.Now.Year, 03, 01);
                dtDateTimeEnd = new DateTime(DateTime.Now.Year + 1, 3, 1).AddDays(-1);
            }
            else
            {
                int intYear = DateTime.Now.Year;

                if (DateTime.Now.Month < 3)
                {
                    intYear = intYear - 1;
                }

                dtDateTimeStart = new DateTime(intYear, 3, 1);
                dtDateTimeEnd = new DateTime(intYear + 1, 3, 1).AddDays(-1);
            }

            while (true)
            {
                strQry = "";
                strQry += " INSERT INTO InteractPayroll.dbo.DATES";
                strQry += "(DAY_DATE";
                strQry += ",DAY_NO)";
                strQry += " VALUES ";
                strQry += "('" + dtDateTimeStart.ToString("yyyy-MM-dd") + "'";
                strQry += "," + Convert.ToInt32(dtDateTimeStart.DayOfWeek) + ")";

                clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

                if (dtDateTimeStart.Year == dtDateTimeEnd.Year
                    & dtDateTimeStart.Month == dtDateTimeEnd.Month
                    & dtDateTimeStart.Day == dtDateTimeEnd.Day)
                {
                    break;
                }

                dtDateTimeStart = dtDateTimeStart.AddDays(1);
            }
        }
    }
}
