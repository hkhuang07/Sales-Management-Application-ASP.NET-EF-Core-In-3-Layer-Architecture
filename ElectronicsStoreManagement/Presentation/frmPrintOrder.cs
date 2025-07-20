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
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataAccess;
using ElectronicsStore.Presentation.Reports;
using Microsoft.Reporting.WinForms;

namespace ElectronicsStore.Presentation
{
    public partial class frmPrintOrder : Form
    {
        ElectronicsStoreDataSet.OrderDetailsListDataTable orderDetailsListDataTable = new ElectronicsStoreDataSet.OrderDetailsListDataTable();
        string reportsFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Reports");
        int id; // Mã hóa đơn
        private readonly OrderService _orderService;
        private readonly OrderDetailsService _orderDetailsService;
        public frmPrintOrder(int orderID = 0)
        {
            InitializeComponent();
            id = orderID;
            _orderService = new OrderService(MapperConfig.Initialize());
            _orderDetailsService = new OrderDetailsService(MapperConfig.Initialize());
            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "printorder.html";
        }

        private void frmPrintOrder_Load(object sender, EventArgs e)
        {
            var orderDto = _orderService.GetById(id);
            string customer = orderDto.CustomerName;

            try
            {
                if (orderDto != null)
                {
                    // 2. Lấy danh sách chi tiết đơn hàng từ BLL
                    var detailsDto = _orderDetailsService.GetByOrderID(id);

                    // 3. Chuyển dữ liệu chi tiết sang định dạng DataTable để truyền vào ReportViewer
                    orderDetailsListDataTable.Clear();

                    foreach (var detail in detailsDto)
                    {
                        orderDetailsListDataTable.AddOrderDetailsListRow(
                            id, // OrderID
                            detail.ProductID, // ProductID
                            detail.ProductName ?? string.Empty, // ProductName
                            detail.Quantity, // Quantity
                            detail.Price, // Price
                            detail.Quantity * detail.Price // TotalPrice
                        );
                    }
                    // 4. Đổ dữ liệu vào ReportViewer
                    ReportDataSource reportDataSource = new ReportDataSource
                    {
                        Name = "OrderDetailsList",
                        Value = orderDetailsListDataTable
                    };

                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptOrder.rdlc");

                    string CompanyName = ConfigurationManager.AppSettings["CompanyName"]!.ToString();
                    string SellerAddress = ConfigurationManager.AppSettings["SellerAddress"]!.ToString();
                    string SellerTIN = ConfigurationManager.AppSettings["SellerTIN"]!.ToString();

                    Console.WriteLine($"CustomerName: {orderDto.CustomerName}");
                    // 5. Tạo tham số cho báo cáo
                    IList<ReportParameter> param = new List<ReportParameter>
                {
                    new ReportParameter("Date", string.Format("Ngày {0} Tháng {1} Năm {2}",
                        orderDto.Date.Day,
                        orderDto.Date.Month,
                        orderDto.Date.Year)),

                    new ReportParameter("CompanyName", CompanyName),
                    new ReportParameter("SellerAddress", SellerAddress),
                    new ReportParameter("SellerTIN",SellerTIN ),

                    new ReportParameter("BuyerName", customer),
                    new ReportParameter("BuyerAddress", ""),
                    new ReportParameter("BuyerTIN", ""),

                    new ReportParameter("Total", detailsDto.Sum(d => d.Quantity * d.Price).ToString())
                };

                    reportViewer.LocalReport.SetParameters(param);

                    // 6. Cài đặt chế độ xem và in
                    reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
                    reportViewer.ZoomMode = ZoomMode.Percent;
                    reportViewer.ZoomPercent = 100;
                    reportViewer.RefreshReport();
                }
             }
            catch (Exception ex)
            {
                MessageBox.Show("Error during load order: " + ex.Message);
                return;
            }

           
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var orderDto = _orderService.GetById(id);
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
                    saveFileDialog.FileName = "OrderReport_" + orderDto.ID.ToString() + ".pdf";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Save File PDF
                        File.WriteAllBytes(saveFileDialog.FileName, bytes);
                        MessageBox.Show("Order saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when print report: " + ex.Message);
            }
        }
    }
}
