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
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataAccess;
using ElectronicsStore.DataTransferObject;

namespace ElectronicsStore.Presentation
{
    public partial class frmOrderDetails : Form
    {

        private readonly OrderService _orderService;
        private readonly OrderDetailsService _odService;
        private readonly ProductService _productService;
        private readonly EmployeeService _employeeService;
        private readonly CustomerService _customerService;
        private readonly IMapper _mapper;

        private BindingList<OrderDetailsDTO> orderDetails;

        public int OrderID { get; set; }
        public int EmployeeID { get; set; }
        public int CustomerID { get; set; }

        public frmOrderDetails(int orderID = 0)
        {
            InitializeComponent();

            _mapper = MapperConfig.Initialize();
            _orderService = new OrderService(_mapper);
            _odService = new OrderDetailsService(_mapper);
            _productService = new ProductService(_mapper);
            _employeeService = new EmployeeService(_mapper);
            _customerService = new CustomerService(_mapper);
            OrderID = orderID;

            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "orderdetais.html";
        }

        private void LoadData()
        {
            cboEmployee.DataSource = _employeeService.GetAll();
            cboEmployee.DisplayMember = "FullName";
            cboEmployee.ValueMember = "ID";

            cboCustomer.DataSource = _customerService.GetAll();
            cboCustomer.DisplayMember = "CustomerName";
            cboCustomer.ValueMember = "ID";

            cboProduct.DataSource = _productService.GetAll();
            cboProduct.DisplayMember = "ProductName";
            cboProduct.ValueMember = "ID";

            if (EmployeeID != 0)
                cboEmployee.SelectedValue = EmployeeID;

            if (CustomerID != 0)
                cboCustomer.SelectedValue = CustomerID;
        }

        public void EnableControls()
        {
            if (OrderID == 0 && dataGridView.Rows.Count == 0) // Add mode
            {
                cboCustomer.SelectedIndex = -1;
                cboEmployee.SelectedIndex = -1;
                cboProduct.SelectedIndex = -1;
                numQuantity.Value = 1;
                numPrice.Value = 0;
            }

            btnSave.Enabled = dataGridView.Rows.Count > 0;
            btnDelete.Enabled = dataGridView.Rows.Count > 0;
        }


        private void frmOrderDetails_Load(object sender, EventArgs e)
        {
            LoadData();
            dataGridView.AutoGenerateColumns = false;

            if (OrderID != 0)
            {
                var order = _orderService.GetById(OrderID);
                if (order != null)
                {
                    cboEmployee.SelectedValue = order.EmployeeID;
                    cboCustomer.SelectedValue = order.CustomerID;
                    txtNote.Text = order.Note;
                    numQuantity.Value = 1;
                    numPrice.Value = 0;     

                    var details = _odService.GetByOrderID(OrderID);
                    foreach (var item in details)
                    {
                        var product = _productService.GetById(item.ProductID);
                        item.ProductName = product?.ProductName ?? "";
                        //item.TotalPrice = item.Quantity * item.Price;
                    }

                    orderDetails = new BindingList<OrderDetailsDTO>(details);
                }
            }
            else
            {
                orderDetails = new BindingList<OrderDetailsDTO>();
            }

            dataGridView.DataSource = orderDetails;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            EnableControls();

        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null || dataGridView.CurrentRow.DataBoundItem == null)
                return;

            var selectedDetail = dataGridView.CurrentRow.DataBoundItem as OrderDetailsDTO;
            if (selectedDetail != null)
            {
                cboProduct.SelectedValue = selectedDetail.ProductID;
                numQuantity.Value = selectedDetail.Quantity;
                numPrice.Value = selectedDetail.Price;
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboProduct.Text))
            {
                MessageBox.Show("Please select product.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numQuantity.Value <= 0)
            {
                MessageBox.Show("Sales quantity must be greater than 0.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numPrice.Value <= 0)
            {
                MessageBox.Show("Product selling price must be greater than 0.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productID = Convert.ToInt32(cboProduct.SelectedValue);
            var existingDetail = orderDetails.FirstOrDefault(x => x.ProductID == productID);

            if (existingDetail != null)
            {
                // Đã có sản phẩm này thì cập nhật
                existingDetail.Quantity = (short)Convert.ToInt32(numQuantity.Value);
                existingDetail.Price = Convert.ToInt32(numPrice.Value);
                //existingDetail.TotalPrice = existingDetail.Quantity * existingDetail.Price;
                dataGridView.Refresh();
            }
            else
            {
                // Thêm mới chi tiết sản phẩm
                var newDetail = new OrderDetailsDTO
                {
                    ID = 0, // Đang thêm mới nên ID = 0
                    OrderID = OrderID,
                    ProductID = productID,
                    ProductName = cboProduct.Text,
                    Quantity = (short)numQuantity.Value,
                    Price = Convert.ToInt32(numPrice.Value),
                    //TotalPrice = Convert.ToInt32(numQuantity.Value * numPrice.Value)
                };

                orderDetails.Add(newDetail);
            }

            EnableControls();
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null) return;

            int productID = Convert.ToInt32(dataGridView.CurrentRow.Cells["ProductID"].Value);
            var item = orderDetails.FirstOrDefault(x => x.ProductID == productID);
            if (item != null)
                orderDetails.Remove(item);

            EnableControls();
        }

        private void cboProduct_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int productID = Convert.ToInt32(cboProduct.SelectedValue.ToString());
            var product = _productService.GetById(productID);
            numPrice.Value = product.Price;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboEmployee.Text))
            {
                MessageBox.Show("Please select billing staff.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(cboCustomer.Text))
            {
                MessageBox.Show("Please select customer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tạo đối tượng OrderDTO từ dữ liệu trên form
            var orderDto = new OrderDTO
            {
                ID = OrderID,
                EmployeeID = Convert.ToInt32(cboEmployee.SelectedValue),
                CustomerID = Convert.ToInt32(cboCustomer.SelectedValue),
                Date = DateTime.Now,
                Note = txtNote.Text
            };

            try
            {
                if (OrderID != 0)
                {
                    // Cập nhật đơn hàng
                    _orderService.UpdateOrder(orderDto, orderDetails.ToList());
                }
                else
                {
                    // Thêm mới đơn hàng và cập nhật OrderID sau khi lưu
                    OrderID = _orderService.CreateOrder(orderDto, orderDetails.ToList());
                }

                MessageBox.Show("Saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close(); // Đóng form sau khi lưu
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
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not yet implemented.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

