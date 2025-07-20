using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataTransferObject;
using Microsoft.Reporting.WinForms;
using static ElectronicsStore.Presentation.Reports.ElectronicsStoreDataSet;

namespace ElectronicsStore.Presentation
{
    public partial class frmSale : Form
    {
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly CustomerService _customerService;
        private int currentOrderID = 0; // Giả sử là 0 khi chưa lưu xuống DB
        private List<OrderDetailsDTO> orderDetails = new List<OrderDetailsDTO>();
        string imagesFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Images");

        //frmMain main = null;
        BindingSource bindingOrder = new BindingSource();
        BindingSource bindingOrderDetails = new BindingSource();

        public frmSale()
        {
            InitializeComponent();
            _productService = new ProductService(MapperConfig.Initialize()); // AutoMapper configuration
            _orderService = new OrderService(MapperConfig.Initialize());
            _customerService = new CustomerService(MapperConfig.Initialize());

            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "sale.html";
        }

        //Load product
        private void LoadProducts()
        {
            flowLayoutPanel1.Controls.Clear();

            var products = _productService.GetAll(); // danh sách ProductDTO

            foreach (var product in products)
            {
                string fileName = string.IsNullOrEmpty(product.Image) ? "product_default.jpg" : product.Image;
                var card = new ProductCard();
                card.ProductName = product.ProductName;
                card.Price = product.Price.ToString();
                card.ProductImage = Image.FromFile(Path.Combine(imagesFolder, fileName)); // đường dẫn ảnh
                card.ProductData = product;
                // Gắn sự kiện
                card.AddClicked += ProductCard_AddToOrder;
                card.SubtractClicked += ProductCard_DeleteToOrder;
                card.CardDoubleClicked += SelectCard;


                flowLayoutPanel1.Controls.Add(card);

            }
        }

        //Load Order
        private void Saller_Load(object sender, EventArgs e)
        {
            //Load layout products
            LoadProducts();

            dgvOrder.AutoGenerateColumns = false;
            dgvOrderDetails.AutoGenerateColumns = false;

            //sttEmployee.Text = main.employeeName;

            var orderList = _orderService.GetAllList();
            bindingOrder.DataSource = orderList;
            dgvOrder.DataSource = orderList;
            UpdateRevenue();

        }

        //Find
        private void btnFind_Click(object sender, EventArgs e)
        {
            string keyword = txtFind.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadProducts();
            }
            else
            {
                var filtered = _productService.GetAllList()
                    .Where(p =>
                           p.ProductName.ToLower().Contains(keyword) ||
                           p.Description.ToLower().Contains(keyword) ||
                           p.ManufacturerName.Contains(keyword) ||
                           p.CategoryName.Contains(keyword)
                           )
                    .ToList();

                if (filtered.Count == 0)
                {
                    MessageBox.Show("No matching customer found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

                flowLayoutPanel1.Controls.Clear();
                foreach (var product in filtered)
                {
                    string fileName = string.IsNullOrEmpty(product.Image) ? "product_default.jpg" : product.Image;
                    var card = new ProductCard();
                    card.ProductName = product.ProductName;
                    card.Price = product.Price.ToString();

                    card.ProductImage = Image.FromFile(Path.Combine(imagesFolder, fileName)); // đường dẫn ảnh

                    flowLayoutPanel1.Controls.Add(card);
                }
            }
        }
        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            btnFind_Click(sender, e);
        }


        private void UpdateRevenue()
        {
            decimal totalRevenue = 0;

            foreach (DataGridViewRow row in dgvOrder.Rows)
            {
                if (row.Cells["TotalPrice"].Value != null &&
                    decimal.TryParse(row.Cells["TotalPrice"].Value.ToString(), out decimal price))
                {
                    totalRevenue += price;
                }
            }

            txtRevenue.Text = totalRevenue.ToString("N0"); // Định dạng có dấu phân cách hàng nghìn
        }

        private void RefreshOrderDetails()
        {
            dgvOrderDetails.DataSource = null;
            dgvOrderDetails.DataSource = orderDetails;

            // Tính tổng tiền
            int total = orderDetails.Sum(x => x.Price * x.Quantity);
            txtTotalDetails.Text = total.ToString("N0");
        }

        //Select productcard
        private void SelectCard(object sender, EventArgs e)
        {
            var card = sender as ProductCard;
            card.BackColor = System.Drawing.Color.CornflowerBlue;
        }

        //Add Product to orderdetails
        private void ProductCard_AddToOrder(object sender, EventArgs e)
        {
            var card = sender as ProductCard;
            if (card == null || card.ProductData == null)
                return;

            var product = card.ProductData;

            if (product.Price <= 0)
            {
                MessageBox.Show("Product selling price must be greater than 0.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existingDetail = orderDetails.FirstOrDefault(x => x.ProductID == product.ID);

            if (existingDetail != null)
            {
                // Tăng số lượng nếu đã có
                existingDetail.Quantity++;
            }
            else
            {
                // Thêm mới
                var detail = new OrderDetailsDTO
                {
                    ID = 0,
                    OrderID = currentOrderID,
                    ProductID = product.ID,
                    ProductName = product.ProductName,
                    Quantity = 1,
                    Price = product.Price
                };
                orderDetails.Add(detail);
                card.BackColor = System.Drawing.Color.Lavender;

            }

            RefreshOrderDetails();
        }
        //delete ProductCard form orderdetails
        private void ProductCard_DeleteToOrder(object sender, EventArgs e)
        {
            var card = sender as ProductCard;
            if (card == null || card.ProductData == null)
                return;

            var product = card.ProductData;

            var existingDetail = orderDetails.FirstOrDefault(x => x.ProductID == product.ID);
            if (existingDetail != null)
            {
                existingDetail.Quantity--;

                if (existingDetail.Quantity <= 0)
                {
                    orderDetails.Remove(existingDetail);
                }
                RefreshOrderDetails();
            }
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            var selectedCard = flowLayoutPanel1.Controls
               .OfType<ProductCard>()
               .FirstOrDefault(c => c.BackColor == System.Drawing.Color.CornflowerBlue); // Giả sử chọn bằng cách đổi màu

            if (selectedCard != null)
            {
                ProductCard_AddToOrder(selectedCard, EventArgs.Empty);
                selectedCard.BackColor = System.Drawing.Color.Lavender;
            }
            else
            {
                MessageBox.Show("Please select a product to add.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        private void bntDelete_Click(object sender, EventArgs e)
        {
            if (dgvOrderDetails.CurrentRow == null)
            {
                MessageBox.Show("Please select a product from the order to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var productId = Convert.ToInt32(dgvOrderDetails.CurrentRow.Cells["ProductID"].Value);

            var existingDetail = orderDetails.FirstOrDefault(x => x.ProductID == productId);

            if (existingDetail != null)
            {
                existingDetail.Quantity--;

                if (existingDetail.Quantity <= 0)
                {
                    orderDetails.Remove(existingDetail);
                }
                RefreshOrderDetails();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            orderDetails.Clear();
            RefreshOrderDetails();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (dgvOrderDetails.Rows.Count <= 0)
            {
                MessageBox.Show("Please select a product from the order to order.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (frmConfirm confirm = new frmConfirm(0)) // Thêm mới, nên orderID = 0
            {

                if (confirm.ShowDialog() == DialogResult.OK)
                {
                    if (string.IsNullOrWhiteSpace(confirm.cboCustomer.Text))
                    {
                        MessageBox.Show("Customer name cannot be left blank!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var customerList = _customerService.GetByName(confirm.cboCustomer.Text);
                    var customer = customerList.FirstOrDefault();

                    if (orderDetails == null || !orderDetails.Any())
                    {
                        MessageBox.Show("No products selected for the order!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Tạo đối tượng OrderDTO từ dữ liệu trên form
                    var orderDto = new OrderDTO
                    {
                        ID = 0,
                        EmployeeID = Convert.ToInt32(confirm.cboEmployee.SelectedValue),
                        CustomerID = customer.ID,
                        Date = DateTime.Now,
                        Note = confirm.txtNote.Text
                    };

                    try
                    {
                        if (currentOrderID != 0)
                        {
                            _orderService.UpdateOrder(orderDto, orderDetails.ToList());
                            orderDetails.Clear();
                        }
                        else
                        {
                            currentOrderID = _orderService.CreateOrder(orderDto, orderDetails.ToList());
                            orderDetails.Clear();

                        }

                        // Nếu người dùng chọn in hóa đơn
                        if (confirm.chkPrintInvoice.Checked)
                        {
                            frmPrintOrder report = new frmPrintOrder(currentOrderID);
                            report.ShowDialog();
                        }

                        MessageBox.Show("Order saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Saller_Load(sender, e); // Tải lại đơn hàng
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show($"An error occurred while saving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        string fullMessage = ex.Message;
                        if (ex.InnerException != null)
                            fullMessage += "\n\n" + ex.InnerException.Message;
                        if (ex.InnerException?.InnerException != null)
                            fullMessage += "\n\n" + ex.InnerException.InnerException.Message;

                        MessageBox.Show($"An error occurred while saving:\n{fullMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    RefreshOrderDetails();
                    Saller_Load(sender, e); // Reload lại danh sách đơn hàng
                }
            }

        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            var orderList = _orderService.GetAllList();
            orderList = orderList.Where(r => r.Date >= dtpStart.Value && r.Date <= dtpEnd.Value).ToList();
            dgvOrder.DataSource = orderList;  //orderListDataTable.Clear(); 
            UpdateRevenue();

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(dgvOrder.CurrentRow.Cells["dataGridViewTextBoxColumn1"].Value.ToString());
            using (frmPrintOrder printOrder = new frmPrintOrder(id))
            {
                printOrder.ShowDialog();
            }
        }
    }
}
