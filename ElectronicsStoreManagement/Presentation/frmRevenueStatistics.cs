using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.InkML;
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataTransferObject;
using ElectronicsStore.Presentation.Reports;
using Microsoft.Reporting.WinForms;
using static ElectronicsStore.Presentation.Reports.ElectronicsStoreDataSet;

namespace ElectronicsStore.Presentation
{
    public partial class frmRevenueStatistics : Form
    {
        private readonly OrderService _orderService;
        string reportsFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Reports");
        public frmRevenueStatistics()
        {
            InitializeComponent();
            _orderService = new OrderService(MapperConfig.Initialize());

            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "revenuestatistics.html";
        }

        ElectronicsStoreDataSet.OrderListDataTable orderListDataTable = new ElectronicsStoreDataSet.OrderListDataTable();
        List<OrderDTO> OrderList = new List<OrderDTO>();


        private void rptRevenueStatistics_Load(object sender, EventArgs e)
        {
            var orderList = _orderService.GetAllList();
            orderListDataTable.Clear();
            foreach (var row in orderList)
            {
                orderListDataTable.AddOrderListRow(row.ID,
                    row.EmployeeID,
                    row.EmployeeName,
                    row.CustomerID,
                    row.CustomerName,
                    row.Date,
                    row.Note,
                    row.TotalPrice ?? 0);
            }
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "OrderList";
            reportDataSource.Value = orderListDataTable;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptRevenueStatistics.rdlc");

            reportViewer.LocalReport.Refresh(); ReportParameter reportParameter = new ReportParameter("ResultDescription", "(All time)");
            reportViewer.LocalReport.SetParameters(reportParameter);

            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 100;
            reportViewer.RefreshReport();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            var orderList = _orderService.GetAllList();
            orderList = orderList.Where(r => r.Date >= dtpStart.Value && r.Date <= dtpEnd.Value).ToList();
            orderListDataTable.Clear();
            foreach (var row in orderList)
            {
                orderListDataTable.AddOrderListRow(row.ID,
                    row.EmployeeID,
                    row.EmployeeName,
                    row.CustomerID,
                    row.CustomerName,
                    row.Date,
                    row.Note,
                    row.TotalPrice ?? 0);

            }
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "OrderList";
            reportDataSource.Value = orderListDataTable;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptRevenueStatistics.rdlc");
            ReportParameter reportParameter = new ReportParameter("ResultDescription", "From date: " + dtpStart.Value.Date.ToString("MM/dd/yyyy") + " - To date: " + dtpEnd.Value.Date.ToString("MM/dd/yyyy"));
            reportViewer.LocalReport.SetParameters(reportParameter);
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 100;
            reportViewer.RefreshReport();
        }


        private void btnShowAll_Click(object sender, EventArgs e)
        {
            rptRevenueStatistics_Load(sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //Check if have no data in ReportViewer
                if (reportViewer.LocalReport.DataSources.Count == 0)
                {
                    MessageBox.Show("No report data to save.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //byte[] bytes = rvrReport.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                byte[] bytes = reportViewer.LocalReport.Render(
                    "PDF", null,
                    out string newMimeType,
                    out string newEncoding,
                    out string newExtension,
                    out string[] newStreamIds,
                    out Warning[] newWarnings
                    );
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PDF Files(*.pdf)|*.pdf";
                    saveFileDialog.DefaultExt = "pdf";
                    saveFileDialog.FileName = "Revenue_Statistics" + DateTime.Now.Date.ToString("dd_MM_yyyy") + ".pdf";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Save File PDF
                        File.WriteAllBytes(saveFileDialog.FileName, bytes);
                        MessageBox.Show("Revenue Statistics saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when print report: " + ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
