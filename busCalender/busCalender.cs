using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InteractPayroll
{
    public class busCalender
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busCalender()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            //Used In Calender Menu Option
            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PaidHoliday", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
