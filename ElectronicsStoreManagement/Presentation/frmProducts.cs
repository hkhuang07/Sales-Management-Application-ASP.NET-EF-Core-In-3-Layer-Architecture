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
using ElectronicsStore.BusinessLogic;
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataAccess;
using ElectronicsStore.DataTransferObject;
using SlugGenerator;

namespace ElectronicsStore.Presentation
{
    public partial class frmProducts : Form
    {
        private readonly ProductService _productservice;
        private readonly ManufacturerService _manufacturerService;
        private readonly CategoryService _categoryService;
        private bool signAdd = false;
        private int id;
        string imagesFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Images");
        BindingSource binding = new BindingSource();


        public frmProducts()
        {
            InitializeComponent();
            _productservice = new ProductService(MapperConfig.Initialize()); // AutoMapper configuration
            _manufacturerService = new ManufacturerService(MapperConfig.Initialize());
            _categoryService = new CategoryService(MapperConfig.Initialize());

            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "products.html";
        }

        private void EnableControls(bool value)
        {
            btnSave.Enabled = value;
            btnCancel.Enabled = value;
            txtProductName.Enabled = value;
            txtDescription.Enabled = value;
            numPrice.Enabled = value;
            numQuantity.Enabled = value;
            cboCategory.Enabled = value;
            cboManufacturer.Enabled = value;
            picImage.Enabled = value;

            btnAdd.Enabled = !value;
            btnUpdate.Enabled = !value;
            btnDelete.Enabled = !value;
            btnChangeImage.Enabled = !value;
            btnFind.Enabled = !value;
            btnImport.Enabled = !value;
            btnExport.Enabled = !value;
        }

        public void GetCategories()
        {
            var categories = _categoryService.GetAll();
            cboCategory.DataSource = categories;
            cboCategory.DisplayMember = "CategoryName";
            cboCategory.ValueMember = "ID";
        }


        public void GetManufacturers()
        {
            var manufacturers = _manufacturerService.GetAll();
            cboManufacturer.DataSource = manufacturers;
            cboManufacturer.DisplayMember = "ManufacturerName";
            cboManufacturer.ValueMember = "ID";
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
                    dataGridView.DataSource = _productservice.GetAllList();
                }
                else
                {
                    var filtered = _productservice.GetAllList()
                        .Where(p =>
                               p.ProductName.ToLower().Contains(keyword) ||
                               p.Description.ToLower().Contains(keyword) ||
                               p.ManufacturerName.Contains(keyword) ||
                               p.CategoryName.Contains(keyword)
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

        private void frmProducts_Load(object sender, EventArgs e)
        {
            dataGridView.AutoGenerateColumns = false;
            EnableControls(false);
            GetCategories();
            GetManufacturers();

            var list = _productservice.GetAllList(); // Trả về List<ProductList>
            binding.DataSource = list;
            SetupToolStrip();

            txtProductName.DataBindings.Clear();
            txtProductName.DataBindings.Add("Text", binding, "ProductName", false, DataSourceUpdateMode.Never);
            txtDescription.DataBindings.Clear();
            txtDescription.DataBindings.Add("Text", binding, "Description", false, DataSourceUpdateMode.Never);
            numPrice.DataBindings.Clear();
            numPrice.DataBindings.Add("Value", binding, "Price", false, DataSourceUpdateMode.Never);
            numQuantity.DataBindings.Clear();
            numQuantity.DataBindings.Add("Value", binding, "Quantity", false, DataSourceUpdateMode.Never);
            cboCategory.DataBindings.Clear();
            cboCategory.DataBindings.Add("SelectedValue", binding, "CategoryID", false, DataSourceUpdateMode.Never);
            cboManufacturer.DataBindings.Clear();
            cboManufacturer.DataBindings.Add("SelectedValue", binding, "ManufacturerID", false, DataSourceUpdateMode.Never);
            picImage.DataBindings.Clear();

            Binding iMage = new Binding("ImageLocation", binding, "Image");
            iMage.Format += (s, e) =>
            {
                // e.Value = Path.Combine(imagesFolder, e.Value.ToString());
                string fileName = string.IsNullOrEmpty(e.Value?.ToString()) ? "product_default.jpg" : e.Value.ToString();
                e.Value = Path.Combine(imagesFolder, fileName);

            };
            picImage.DataBindings.Add(iMage);


            dataGridView.DataSource = binding;
        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dataGridView.Columns[e.ColumnIndex].Name == "Image")
                {
                    string fileName = string.IsNullOrEmpty(e.Value?.ToString()) ? "product_default.jpg" : e.Value.ToString();
                    string fullPath = Path.Combine(imagesFolder, fileName);

                    System.Drawing.Image imageToShow;
                    if (File.Exists(fullPath))
                    {
                        imageToShow = System.Drawing.Image.FromFile(fullPath);
                    }
                    else
                    {
                        imageToShow = System.Drawing.Image.FromFile(Path.Combine(imagesFolder, "product_default.jpg"));
                    }

                    imageToShow = new Bitmap(imageToShow, 24, 24);
                    e.Value = imageToShow;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during load image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            signAdd = true;
            EnableControls(true);
            cboCategory.Text = "";
            cboManufacturer.Text = "";
            txtProductName.Clear();
            txtDescription.Clear();
            numPrice.Value = 0;
            numQuantity.Value = 0;
            picImage.Image = null;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            signAdd = false;
            EnableControls(true);
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                    _productservice.Delete(id);
                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmProducts_Load(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var dto = new ProductDTO
                {
                    ProductName = txtProductName.Text,
                    Description = txtDescription.Text,
                    Price = (int)numPrice.Value,
                    Quantity = (int)numQuantity.Value,
                    CategoryID = Convert.ToInt32(cboCategory.SelectedValue),
                    ManufacturerID = Convert.ToInt32(cboManufacturer.SelectedValue),
                    Image = Path.GetFileName(picImage.ImageLocation) // Lưu tên file
                };


                if (signAdd)
                    _productservice.Add(dto);
                else
                    _productservice.Update(id, dto);
                MessageBox.Show("Operation successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmProducts_Load(sender, e);
                EnableControls(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            frmProducts_Load(sender, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFind.Clear();
            binding.DataSource = _productservice.GetAllList();
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
                                var dto = new ProductDTO
                                {
                                    ProductName = r["ProductName"].ToString(),
                                    Description = r["Description"].ToString(),
                                    Price = Convert.ToInt32(r["Price"]),
                                    Quantity = Convert.ToInt32(r["Quantity"]),
                                    CategoryID = Convert.ToInt32(r["CategoryID"]),
                                    ManufacturerID = Convert.ToInt32(r["ManufacturerID"]),
                                    Image = Path.GetFileName(r["Image"].ToString()) // Lưu tên file
                                };


                                _productservice.Add(dto);
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error importing row: {ex.Message}");
                                continue;
                            }
                        }

                        MessageBox.Show($"{successCount} row is imported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frmProducts_Load(sender, e);
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
            saveFileDialog.FileName = "Products_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();
                    table.Columns.AddRange(new DataColumn[]
                        {
                        new DataColumn("ID"),
                        new DataColumn("ProductName"),
                        new DataColumn("Description"),
                        new DataColumn("Price"),
                        new DataColumn("Quantity"),
                        new DataColumn("Category"),
                        new DataColumn("Manufacturer"),
                        new DataColumn("Image")
                    });
                    var products = _productservice.GetAllList();
                    if (products != null)
                    {
                        foreach (var p in products)
                            table.Rows.Add(p.ID, p.ProductName, p.Description, p.Price, p.Quantity, p.CategoryName, p.ManufacturerName, p.Image);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var sheet = wb.Worksheets.Add(table, "Products");
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

        private void btnChangeImage_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Update Product Image",
                Filter = "Image file: |*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                string ext = Path.GetExtension(openFileDialog.FileName);
                string slugName = fileName.GenerateSlug() + ext;
                string fileSavePath = Path.Combine(imagesFolder, slugName);

                File.Copy(openFileDialog.FileName, fileSavePath, true);

                int id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);
                // Giải phóng hình ảnh cũ nếu có
                if (picImage.Image != null)
                {
                    picImage.Image.Dispose();
                    picImage.Image = null;
                }

                // Gọi qua BLL
                _productservice.UpdateImage(id, slugName);

                MessageBox.Show("Update successfully!");
                frmProducts_Load(sender, e);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
