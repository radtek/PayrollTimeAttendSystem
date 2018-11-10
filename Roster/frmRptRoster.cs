using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmRptRoster : Form
    {
        clsISUtilities clsISUtilities;
        DataSet DataSet = null;

        public frmRptRoster(DataSet rptDataSet)
        {
            InitializeComponent();

            DataSet = rptDataSet;
        }

        private void frmRptRoster_Load(object sender, EventArgs e)
        {
            clsISUtilities = new clsISUtilities();

            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", DataSet.Tables["Report"]);

                this.reportViewer.LocalReport.DataSources.Clear();
                this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);
                
                this.reportViewer.PageCountMode = Microsoft.Reporting.WinForms.PageCountMode.Actual;

                this.reportViewer.RefreshReport();
                this.reportViewer.Focus();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
    }
}
