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
using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataAccess;

namespace ElectronicsStore.Presentation
{
    public partial class frmOrders : Form
    {
        private readonly OrderService _orderService;
        private int id;

        BindingSource binding = new BindingSource();

        public frmOrders()
        {
            InitializeComponent();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();
            _orderService = new OrderService(mapper);
            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "orders.html";
        }

        private void SetupToolStrip()
        {
            // Nút di chuyển đến đầu tiên
            btnBegin.Click += (s, e) =>
            {
                if (binding.Count > 0)
                    binding.MoveFirst();
                dataGridView.DataSource = binding;
            };

            // Nút di chuyển đến dòng trước
            btnPrevious.Click += (s, e) =>
            {
                if (binding.Position > 0)
                    binding.MovePrevious();
                dataGridView.DataSource = binding;
            };

            // Nút di chuyển đến dòng tiếp theo
            btnNext.Click += (s, e) =>
            {
                if (binding.Position < binding.Count - 1)
                    binding.MoveNext();
                dataGridView.DataSource = binding;
            };

            // Nút di chuyển đến cuối cùng
            btnEnd.Click += (s, e) =>
            {
                if (binding.Count > 0)
                    binding.MoveLast();
                dataGridView.DataSource = binding;
            };

            // Nút tìm kiếm
            // Thực thi tìm kiếm khi có sự thay đổi trong txtFind
            btnFind.Click += (s, e) =>
            {
                string keyword = txtFind.Text.Trim().ToLower();
                if (string.IsNullOrEmpty(keyword))
                {
                    // Nếu ô tìm kiếm trống, load lại toàn bộ danh sách
                    dataGridView.DataSource = _orderService.GetAllList();
                }
                else
                {
                    var filtered = _orderService.GetAllList()
                        .Where(ord =>
                               ord.CustomerName.ToLower().Contains(keyword) ||
                               ord.Date.ToString().Contains(keyword) ||
                               ord.EmployeeName.Contains(keyword) ||
                               ord.Note.ToLower().Contains(keyword) ||
                               ord.TotalPrice.ToString().Contains(keyword) // Thêm điều kiện tìm kiếm theo giá
                               )
                        .ToList();

                    if (filtered.Count == 0)
                    {
                        lblMessage.Text = "No matching customer found.";
                    }

                    binding.DataSource = filtered;
                    dataGridView.DataSource = binding;
                }
            };

            // Fix for the errors in the problematic line
            txtFind.TextChanged += (s, e) =>
            {
                lblMessage.Text = ""; // Clear the message label when text changes
                btnFind.PerformClick(); // Trigger the click event of btnFind when txtFind text changes
            };

        }

        private void frmOrders_Load(object sender, EventArgs e)
        {
            dataGridView.AutoGenerateColumns = false;
            var orderList = _orderService.GetAllList();
            binding.DataSource = orderList;
            SetupToolStrip();
            dataGridView.DataSource = orderList;

        }



        private void btnCreate_Click(object sender, EventArgs e)
        {
            using (frmOrderDetails orderDetails = new frmOrderDetails(0)) // Thêm mới, nên orderID = 0
            {
                if (orderDetails.ShowDialog() == DialogResult.OK)
                {
                    orderDetails.MdiParent = this.MdiParent;
                    frmOrders_Load(sender, e); // Reload lại danh sách đơn hàng
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
            using (frmPrintOrder printOrder = new frmPrintOrder(id))
            {
                printOrder.ShowDialog();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            /* if (dataGridView.CurrentRow != null)
             {
                 id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);
                 using (frmOrderDetails orderDetails = new frmOrderDetails(id))
                 {
                     orderDetails.ShowDialog();
                 }
             } */

            if (dataGridView.CurrentRow != null)
            {
                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);
                using (frmOrderDetails orderDetails = new frmOrderDetails(id))
                {
                    if (orderDetails.ShowDialog() == DialogResult.OK)
                    {
                        frmOrders_Load(sender, e); // Reload lại danh sách đơn hàng sau khi cập nhật
                    }
                }
            }
        }

        private void bntDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this order?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);
                    _orderService.Delete(id);
                    MessageBox.Show("Order deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmOrders_Load(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {

        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Export to Excel file";
            saveFileDialog.Filter = "Excel file|*.xls;*.xlsx";
            saveFileDialog.FileName = "Orders_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();
                    table.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("ID", typeof(int)),
                        new DataColumn("Date", typeof(DateTime)),
                        new DataColumn("EmployeeName", typeof(string)),
                        new DataColumn("CustomerName", typeof(string)),
                        new DataColumn("TotalPrice", typeof(double)),
                        new DataColumn("Note", typeof(string)),
                        new DataColumn("ViewDetails", typeof(string)),
                    });
                    var order = _orderService.GetAllList();
                    if (order != null)
                    {
                        foreach (var ord in order)
                        {
                            table.Rows.Add(ord.ID, ord.Date, ord.EmployeeName, ord.CustomerName, ord.TotalPrice, ord.Note, ord.ViewDetails);
                        }
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var sheet = wb.Worksheets.Add(table, "Orders");
                        sheet.Columns().AdjustToContents();
                        wb.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Exported data to Excel file successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView.Columns["ViewDetails"].Index)
            {
                int orderId = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["ID"].Value);
                using (frmOrderDetails orderDetails = new frmOrderDetails(orderId))
                {
                    orderDetails.ShowDialog();
                }
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFind.Clear();
            binding.DataSource = _orderService.GetAllList();
            dataGridView.DataSource = binding;
        }
    }
}
