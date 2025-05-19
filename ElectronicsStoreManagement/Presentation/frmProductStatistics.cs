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
using DocumentFormat.OpenXml.Office2010.Excel;
using ElectronicsStore.BusinessLogic;
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataTransferObject;
using ElectronicsStore.Presentation.Reports;
using Microsoft.Reporting.WinForms;

namespace ElectronicsStore.Presentation
{
    public partial class frmProductStatistics : Form
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ManufacturerService _manufacturerService;

        string reportsFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Reports");

        public frmProductStatistics()
        {
            InitializeComponent();
            _productService = new ProductService(MapperConfig.Initialize());
            _categoryService = new CategoryService(MapperConfig.Initialize());
            _manufacturerService = new ManufacturerService(MapperConfig.Initialize());

            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "productstatistics.html";
        }

        ElectronicsStoreDataSet.ProductListDataTable productListDataTable = new ElectronicsStoreDataSet.ProductListDataTable();
        List<ProductDTO> ProductList = new List<ProductDTO>();

        public void LoadData()
        {
            cboCategory.DataSource = _categoryService.GetAll();
            cboCategory.DisplayMember = "CategoryName";
            cboCategory.ValueMember = "ID";

            cboManufacturer.DataSource = _manufacturerService.GetAll();
            cboManufacturer.DisplayMember = "ManufacturerName";
            cboManufacturer.ValueMember = "ID";
        }

        private void frmProductStatistics_Load(object sender, EventArgs e)
        {
            LoadData();
            var productList = _productService.GetAllList();
            productListDataTable.Clear();
            foreach (var row in productList)
            {
                productListDataTable.AddProductListRow(row.ID,
                row.ManufacturerID,
                row.ManufacturerName,
                row.CategoryID,
                row.CategoryName,
                row.ProductName,
                row.Price,
                row.Quantity,
                row.Image,
                row.Description);
            }
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "ProductList";
            reportDataSource.Value = productListDataTable;
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptProductStatistics.rdlc");

            ReportParameter reportParameter = new ReportParameter("ResultDescription", "(All Products)");
            reportViewer.LocalReport.SetParameters(reportParameter);

            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 100;
            reportViewer.RefreshReport();
        }


        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (cboManufacturer.Text == "" && cboCategory.Text == "")
            {
                // Nếu cả 2 ComboBox đều bỏ trống thì hiển thị tất cả
                frmProductStatistics_Load(sender, e);
            }
            else
            {
                var productList = _productService.GetAllList();

                string manufacturer = null;
                string category = null;
                if (cboManufacturer.Text != "")
                {
                    int manufacturerID = Convert.ToInt32(cboManufacturer.SelectedValue.ToString());
                    manufacturer = "Manufacturer: " + cboManufacturer.Text;
                    productList = productList.Where(r => r.ManufacturerID == manufacturerID).ToList();
                }
                if (cboCategory.Text != "")
                {
                    int categoryID = Convert.ToInt32(cboCategory.SelectedValue.ToString());
                    category += "Category: " + cboCategory.Text;
                    productList = productList.Where(r => r.CategoryID == categoryID).ToList();
                }
                productListDataTable.Clear();
                foreach (var row in productList)
                {
                    productListDataTable.AddProductListRow(row.ID,
                    row.ManufacturerID,
                    row.ManufacturerName,
                    row.CategoryID,
                    row.CategoryName,
                    row.ProductName,
                    row.Price,
                    row.Quantity,
                    row.Image,
                    row.Description);
                }
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "ProductList";
                reportDataSource.Value = productListDataTable;
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptProductStatistics.rdlc");
                ReportParameter reportParameter = new ReportParameter("ResultDescription", "(" + manufacturer + " - " + category + ")");
                reportViewer.LocalReport.SetParameters(reportParameter);
                reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer.ZoomMode = ZoomMode.Percent;
                reportViewer.ZoomPercent = 100;
                reportViewer.RefreshReport();
            }
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
                    saveFileDialog.FileName = "Product_Statistics" + DateTime.Now.Date.ToString("dd_MM_yyyy") + ".pdf";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Save File PDF
                        File.WriteAllBytes(saveFileDialog.FileName, bytes);
                        MessageBox.Show("Product statistics saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when print report: " + ex.Message);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }
    }
}
