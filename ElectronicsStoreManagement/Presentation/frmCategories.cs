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
using DocumentFormat.OpenXml.InkML;
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataAccess;
using ElectronicsStore.DataTransferObject;

namespace ElectronicsStore.Presentation
{
    public partial class frmCategories : Form
    {
        private readonly CategoryService _service;
        bool signAdd = false;
        int id;

        BindingSource binding = new BindingSource();

        public frmCategories()
        {
            InitializeComponent();
            _service = new CategoryService(MapperConfig.Initialize()); // AutoMapper cấu hình
            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "categories.html";
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
                        .Where(c => !string.IsNullOrEmpty(c.CategoryName) && c.CategoryName.ToLower().Contains(keyword))
                        .ToList();

                    if (filtered.Count == 0)
                    {
                        lblMessage.Text = "No matching category found.";
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


        private void EnableControls(bool value)
        {
            btnSave.Enabled = value;
            btnCancel.Enabled = value;
            txtCategoryName.Enabled = value;

            btnAdd.Enabled = !value;
            btnEdit.Enabled = !value;
            btnDelete.Enabled = !value;
            btnImport.Enabled = !value;
            btnExport.Enabled = !value;

        }

        private void Categories_Load(object sender, EventArgs e)
        {
            EnableControls(false);
            var list = _service.GetAll();
            binding.DataSource = list;
            SetupToolStrip();
            txtCategoryName.DataBindings.Clear();
            txtCategoryName.DataBindings.Add("Text", binding, "CategoryName", false, DataSourceUpdateMode.Never);
            dataGridView.DataSource = binding;

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            signAdd = true;
            EnableControls(true);
            txtCategoryName.Clear();
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            signAdd = false;
            EnableControls(true);
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var dto = new CategoryDTO { CategoryName = txtCategoryName.Text };

                if (signAdd)
                    _service.Add(dto);
                else
                    _service.Update(id, dto);
                MessageBox.Show("Operation successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Categories_Load(sender, e);
                EnableControls(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this category?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                    _service.Delete(id);
                    MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Categories_Load(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Categories_Load(sender, e);
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFind.Clear();
            binding.DataSource = _service.GetAll();
            dataGridView.DataSource = binding;
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
                                var dto = new CategoryDTO
                                {
                                    CategoryName = r["CategoryName"].ToString()
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
                        Categories_Load(sender, e);
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
            saveFileDialog.FileName = "Categories_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();
                    table.Columns.AddRange(new DataColumn[2]
                    {
                        new DataColumn("ID", typeof(int)),new DataColumn("TenLoai", typeof(string))
                    });
                    var cate = _service.GetAll();
                    if (cate != null)
                    {
                        foreach (var p in cate)
                            table.Rows.Add(p.ID, p.CategoryName);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var sheet = wb.Worksheets.Add(table, "Categories");
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

      
    }
}
