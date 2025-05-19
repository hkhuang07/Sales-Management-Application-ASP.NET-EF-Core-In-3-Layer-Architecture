using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using ElectronicsStore.BussinessLogic;
using ElectronicsStore.DataTransferObject;

namespace ElectronicsStore.Presentation
{
    public partial class frmConfirm : Form
    {
        private readonly EmployeeService _employeeService;
        private readonly CustomerService _customerService;
        bool signAdd = true;
        int customerID = 0;
       
        public int OrderID { get; set; }
        public int CustomerID { get; set; }

        public frmConfirm(int orderID = 0)
        {
            InitializeComponent();
            _employeeService = new EmployeeService(MapperConfig.Initialize()); // AutoMapper cấu hình
            _customerService = new CustomerService(MapperConfig.Initialize());
            OrderID = orderID;
        }


        public void LoadData()
        {
            var employee = _employeeService.GetAll();
            cboEmployee.DataSource = employee;
            cboEmployee.DisplayMember = "FullName";
            cboEmployee.ValueMember = "ID";

            var customer = _customerService.GetAll();
            cboCustomer.DataSource = customer;
            cboCustomer.DisplayMember = "CustomerName";
            cboCustomer.ValueMember = "ID";

        }

        public void EnableControls(bool value)
        {
            //cboCustomer.Enabled = value;
            txtCustomerEmail.Enabled = value;
            txtCustomerPhone.Enabled = value;
            txtCustomerAddress.Enabled = value;
            txtNote.Enabled = value;
            cboEmployee.Enabled = value;
            btnConfirm.Enabled = value;
            chkPrintInvoice.Enabled = value;


            btnAdd.Enabled = !value;
            btnUpdate.Enabled = !value;
        }

        private void frmCustomer_Add_Load(object sender, EventArgs e)
        {
            LoadData();
            EnableControls(false);
        }


        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                var dto = new CustomerDTO
                {
                    CustomerName = cboCustomer.Text,
                    CustomerAddress = txtCustomerAddress.Text,
                    CustomerPhone = txtCustomerPhone.Text,
                    CustomerEmail = txtCustomerEmail.Text
                };

                if (signAdd)
                    _customerService.Add(dto);
                else
                    _customerService.Update(customerID, dto);
                MessageBox.Show("Operation successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            signAdd = true;
            EnableControls(true);
            cboCustomer.Text = "";
            txtNote.Clear();
            txtCustomerEmail.Clear();
            txtCustomerPhone.Clear();
            txtCustomerAddress.Clear();
        }

        

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            signAdd = false;
            customerID = Convert.ToInt32(cboCustomer.SelectedValue);
            CustomerDTO customer = _customerService.GetById(Convert.ToInt32(cboCustomer.SelectedValue));
            EnableControls(true);

            txtNote.Clear();
            txtCustomerEmail.Clear();
            txtCustomerPhone.Clear();
            txtCustomerAddress.Clear();
            txtCustomerAddress.Text = customer.CustomerAddress;
            txtCustomerPhone.Text = customer.CustomerPhone;
            txtCustomerEmail.Text = customer.CustomerEmail;
            
             
        }

    
    }
}
