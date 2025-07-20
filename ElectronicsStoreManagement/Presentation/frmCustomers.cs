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
using ClosedXML.Excel;
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataTransferObject;

namespace ElectronicsStore.Presentation
{
    public partial class frmCustomers : Form
    {
        private readonly CustomerService _service;
        bool signAdd = false;
        int id;

        private BindingSource binding = new BindingSource();

        public frmCustomers()
        {
            InitializeComponent();
            _service = new CustomerService(MapperConfig.Initialize()); // AutoMapper cấu hình

            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "customers.html";
        }

        private void Customers_Load(object sender, EventArgs e)
        {
            EnableControls(false);
            var list = _service.GetAll();
            binding.DataSource = list;
            SetupToolStrip();

            txtCustomerName.DataBindings.Clear();
            txtCustomerName.DataBindings.Add("Text", binding, "CustomerName", false, DataSourceUpdateMode.Never);
            txtCustomerAddress.DataBindings.Clear();
            txtCustomerAddress.DataBindings.Add("Text", binding, "CustomerAddress", false, DataSourceUpdateMode.Never);
            txtCustomerPhone.DataBindings.Clear();
            txtCustomerPhone.DataBindings.Add("Text", binding, "CustomerPhone", false, DataSourceUpdateMode.Never);
            txtCustomerEmail.DataBindings.Clear();
            txtCustomerEmail.DataBindings.Add("Text", binding, "CustomerEmail", false, DataSourceUpdateMode.Never);
            dataGridView.DataSource = binding;
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
                    dataGridView.DataSource = _service.GetAll();
                }
                else
                {
                    var filtered = _service.GetAll()
                        .Where(c =>
                               c.CustomerName.ToLower().Contains(keyword) ||
                               c.CustomerAddress.ToLower().Contains(keyword) ||
                               c.CustomerPhone.ToLower().Contains(keyword) ||
                               c.CustomerEmail.ToLower().Contains(keyword)  // Thay đổi ở đây
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
                lblMessage.Text = string.Empty; // Clear the message label
                btnFind.PerformClick(); // Trigger the click event of btnFind when txtFind text changes
            };

        }
        private void EnableControls(bool value)
        {
            btnSave.Enabled = value;
            btnCancel.Enabled = value;
            txtCustomerName.Enabled = value;
            txtCustomerAddress.Enabled = value;
            txtCustomerPhone.Enabled = value;
            txtCustomerEmail.Enabled = value;

            btnAdd.Enabled = !value;
            btnUpdate.Enabled = !value;
            btnDelete.Enabled = !value;
            btnFind.Enabled = !value;
            btnImport.Enabled = !value;
            btnExport.Enabled = !value;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            signAdd = true;
            EnableControls(true);
            txtCustomerName.Clear();
            txtCustomerAddress.Clear();
            txtCustomerPhone.Clear();
            txtCustomerEmail.Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            signAdd = false;
            EnableControls(true);
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var dto = new CustomerDTO
                {
                    CustomerName = txtCustomerName.Text,
                    CustomerAddress = txtCustomerAddress.Text,
                    CustomerPhone = txtCustomerPhone.Text,
                    CustomerEmail = txtCustomerEmail.Text
                };

                if (signAdd)
                    _service.Add(dto);
                else
                    _service.Update(id, dto);
                MessageBox.Show("Operation successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Customers_Load(sender, e);
                EnableControls(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this customer?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                    _service.Delete(id);
                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Customers_Load(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Customers_Load(sender, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Import data from Excel file";
            openFileDialog.Filter = "Excel file|*.xls;*.xlsx";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();
                    using (XLWorkbook workbook = new XLWorkbook(openFileDialog.FileName))
                    {
                        IXLWorksheet worksheet = workbook.Worksheet(1);
                        bool firstRow = true;
                        string readRange = "1:1";
                        foreach (IXLRow row in worksheet.RowsUsed())
                        {
                            // Đọc dòng tiêu đề (dòng đầu tiên)
                            if (firstRow)
                            {
                                readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                                foreach (IXLCell cell in row.Cells(readRange))
                                    table.Columns.Add(cell.Value.ToString());
                                firstRow = false;
                            }
                            else // Đọc các dòng nội dung (các dòng tiếp theo)
                            {
                                table.Rows.Add();
                                int cellIndex = 0;
                                foreach (IXLCell cell in row.Cells(readRange))
                                {
                                    table.Rows[table.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                                    cellIndex++;
                                }
                            }
                        }
                        if (firstRow)
                        {
                            MessageBox.Show("Excel file is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        int successCount = 0;
                        foreach (DataRow r in table.Rows)
                        {
                            try
                            {
                                var dto = new CustomerDTO
                                {
                                    CustomerName = r["CustomerName"].ToString(),
                                    CustomerAddress = r["CustomerAddress"].ToString(),
                                    CustomerPhone = r["CustomerPhone"].ToString(),
                                    CustomerEmail = r["CustomerEmail"].ToString()
                                };

                                _service.Add(dto);
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error importing row: {ex.Message}");
                                continue;
                            }
                        }

                        MessageBox.Show($"{successCount} row is imported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Customers_Load(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Export to Excel file";
            saveFileDialog.Filter = "Excel file|*.xls;*.xlsx";
            saveFileDialog.FileName = "Customers_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();
                    table.Columns.AddRange(new DataColumn[5]
                    {
                       new DataColumn("ID", typeof(int)),
                        new DataColumn("CustomerName", typeof(string)),
                        new DataColumn("CustomerAddress", typeof(string)),
                        new DataColumn("CustomerPhone", typeof(string)),
                        new DataColumn("CustomerEmail", typeof(string))
                    });
                    var customer = _service.GetAll();
                    if (customer != null)
                    {
                        foreach (var c in customer)
                            table.Rows.Add(c.ID, c.CustomerName, c.CustomerAddress, c.CustomerPhone, c.CustomerEmail);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var sheet = wb.Worksheets.Add(table, "Customers");
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFind.Clear();
            binding.DataSource = _service.GetAll();
            dataGridView.DataSource = binding;
        }
    }
}
