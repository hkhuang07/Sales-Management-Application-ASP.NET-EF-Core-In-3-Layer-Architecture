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
    public partial class frmEmployees : Form
    {
        private readonly EmployeeService _service;
        bool signAdd = false;
        int id;
        BindingSource binding = new BindingSource();

        public frmEmployees()
        {
            InitializeComponent();
            _service = new EmployeeService(MapperConfig.Initialize()); // AutoMapper cấu hình
            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "employees.html";
        }

        private void EnableControls(bool value)
        {
            btnSave.Enabled = value;
            btnCancel.Enabled = value;
            txtEmployeeName.Enabled = value;
            txtEmployeeUsername.Enabled = value;
            txtEmployeePassword.Enabled = value;
            txtEmployeePassword.Text = "";
            txtEmployeeAddress.Enabled = value;
            txtEmployeePhone.Enabled = value;
            cboRoles.Enabled = value;

            btnAdd.Enabled = !value;
            btnUpdate.Enabled = !value;
            btnDelete.Enabled = !value;
            btnFind.Enabled = !value;
            btnImport.Enabled = !value;
            btnExport.Enabled = !value;
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
                        .Where(emp =>
                               emp.FullName.ToLower().Contains(keyword) ||
                               emp.EmployeeAddress.ToLower().Contains(keyword) ||
                               emp.EmployeePhone.ToLower().Contains(keyword) ||
                               emp.UserName.ToLower().Contains(keyword)  // Thay đổi ở đây
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

        private void Employees_Load(object sender, EventArgs e)
        {
            dataGridView.AutoGenerateColumns = false;
            EnableControls(false);
            var list = _service.GetAll();
            binding.DataSource = list;
            SetupToolStrip();
            txtEmployeeName.DataBindings.Clear();
            txtEmployeeName.DataBindings.Add("Text", binding, "FullName", false, DataSourceUpdateMode.Never);
            txtEmployeeUsername.DataBindings.Clear();
            txtEmployeeUsername.DataBindings.Add("Text", binding, "UserName", false, DataSourceUpdateMode.Never);
            /*txtEmployeePassword.DataBindings.Clear();
            txtEmployeePassword.DataBindings.Add("Text", binding, "Password", false, DataSourceUpdateMode.Never);*/
            txtEmployeeAddress.DataBindings.Clear();
            txtEmployeeAddress.DataBindings.Add("Text", binding, "EmployeeAddress", false, DataSourceUpdateMode.Never);
            txtEmployeePhone.DataBindings.Clear();
            txtEmployeePhone.DataBindings.Add("Text", binding, "EmployeePhone", false, DataSourceUpdateMode.Never);
            cboRoles.DataBindings.Clear();
            cboRoles.DataBindings.Add("SelectedValue", binding, "Role", false, DataSourceUpdateMode.Never);
            dataGridView.DataSource = binding;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            signAdd = true;
            EnableControls(true);
            txtEmployeeName.Focus();
            txtEmployeeName.Clear();
            txtEmployeeUsername.Clear();
            txtEmployeePassword.Clear();
            txtEmployeeAddress.Clear();
            txtEmployeePhone.Clear();
            cboRoles.SelectedIndex = -1;
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
                var employee = new EmployeeDTO
                {
                    FullName = txtEmployeeName.Text,
                    UserName = txtEmployeeUsername.Text,
                    Password = txtEmployeePassword.Text,
                    EmployeeAddress = txtEmployeeAddress.Text,
                    EmployeePhone = txtEmployeePhone.Text,
                    Role = cboRoles.SelectedIndex == 0 ? true : false,
                };
                if (signAdd)
                {
                    _service.Add(employee);
                }
                else
                {
                    _service.Update(id, employee);
                }
                MessageBox.Show("Operation successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Employees_Load(sender, e);
                EnableControls(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this employee?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                    _service.Delete(id);
                    MessageBox.Show("Employee deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Employees_Load(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Employees_Load(sender, e);
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
                                var dto = new EmployeeDTO
                                {
                                    FullName = r["FullName"].ToString(),
                                    UserName = r["UserName"].ToString(),
                                    Password = r["Password"].ToString(),
                                    EmployeeAddress = r["EmployeeAddress"].ToString(),
                                    EmployeePhone = r["EmployeePhone"].ToString(),
                                    Role = Convert.ToBoolean(r["Role"])
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
                        Employees_Load(sender, e);
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
            saveFileDialog.FileName = "Employees_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();
                    table.Columns.AddRange(new DataColumn[]
                    {
                       new DataColumn("ID", typeof(int)),
                        new DataColumn("FullName", typeof(string)),
                        new DataColumn("EmployeePhone", typeof(string)),
                        new DataColumn("EmployeeAddress", typeof(string)),
                        new DataColumn("UserName", typeof(string)) ,
                        new DataColumn("Role", typeof(string)),

                    });
                    var employee = _service.GetAll();
                    if (employee != null)
                    {
                        foreach (var em in employee)
                            table.Rows.Add(em.ID, em.FullName, em.EmployeePhone, em.EmployeeAddress, em.UserName, em.Role);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var sheet = wb.Worksheets.Add(table, "Employees");
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
                                           