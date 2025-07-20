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
using BC = BCrypt.Net.BCrypt;

namespace ElectronicsStore.Presentation
{
    public partial class frmChangePass : Form
    {
        private readonly EmployeeService _employeeService;
        private readonly string _userName;

        public frmChangePass(string userName)
        {
            InitializeComponent();
            _employeeService = new EmployeeService(MapperConfig.Initialize());
            _userName = userName;

            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "changepassword.html";
        }


        private void frmChangePass_Load(object sender, EventArgs e)
        {

        }
        private void btnChange_Click(object sender, EventArgs e)
        {
            string oldPass = txtOldPass.Text.Trim();
            string newPass = txtNewPass.Text.Trim();
            string confirmPass = txtConfirm.Text.Trim();

            if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                MessageBox.Show("Please fill in all fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("New passwords do not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var employee = _employeeService.GetByUserName(_userName);
            if (employee == null)
            {
                MessageBox.Show("User not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!BC.Verify(oldPass, employee.Password))
            {
                MessageBox.Show("Old password is incorrect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Cập nhật mật khẩu
            string hashedPass = BC.HashPassword(newPass);
            _employeeService.UpdatePassword(employee.ID, hashedPass); // viết phương thức UpdatePassword

            MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
    }
}
