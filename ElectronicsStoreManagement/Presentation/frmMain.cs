using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.InkML;
using ElectronicsStore.BussinessLogic;
using BC = BCrypt.Net.BCrypt;

namespace ElectronicsStore.Presentation
{
    public partial class frmMain : Form
    {
        frmLogin logIn = new frmLogin();
        frmChangePass changePass = null; // Đổi mật khẩu
        frmSale sale = null;
        frmCategories categories = null;
        frmManufacturers manufacturers = null;
        frmProducts products = null;
        frmCustomers customers = null;
        frmEmployees employees = null;
        frmOrders orders = null;
        frmOrderDetails orderDetails = null;
        frmProductStatistics productStatistics = null;
        frmRevenueStatistics revenueStatistics = null;
        AboutBox about = null;
        Flash flash = null;

        public string employeeName = ""; // Lấy tên người dùng hiển thị vào thanh Status.

        private readonly EmployeeService _employeesService;


        public frmMain()
        {
            Flash flash = new Flash();
            flash.ShowDialog();

            InitializeComponent();
            _employeesService = new EmployeeService(MapperConfig.Initialize()); // AutoMapper cấu hình

            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "main.html";
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            NotLoggedIn();
            LogIn();
        }

        #region EnableControl

        private void mnuLogIn_Click(object sender, EventArgs e)
        {
            LogIn();
        }

        private void mnuLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to log out ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (Form child in MdiChildren)
                {
                    child.Close();
                }
                NotLoggedIn();
            }  
        }

        private void mnuChangePass_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(employeeName))
            {
                MessageBox.Show("You are not logged in!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy lại user từ EmployeeService
            var emp = _employeesService.GetByFullName(employeeName); // bạn cần thêm GetByFullName nếu chưa có

            if (emp != null)
            {
                if (changePass == null || changePass.IsDisposed)
                {
                    changePass = new frmChangePass(emp.UserName);
                    changePass.MdiParent = this;
                    changePass.Show();
                }
                else
                {
                    changePass.Activate();
                }
            }

        }
        private void mnuRestore_Click(object sender, EventArgs e)
        {
           OpenFileDialog restoreDialog = new OpenFileDialog();
            restoreDialog.Filter = "Backup files (*.bak)|*.bak";
            restoreDialog.Title = "Select the backup file (.bak) to restore";

            if (restoreDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = restoreDialog.FileName;

                if (!string.IsNullOrEmpty(filePath))
                {
                    var databaseService = new DatabaseService();
                    bool success = databaseService.RestoreDatabase(filePath);

                    if (success)
                    {
                        MessageBox.Show("Data recovery successful!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Data recovery successful!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void mnuBackup_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog backupFolder = new FolderBrowserDialog();
            backupFolder.Description = "Select folder to backup data";

            if (backupFolder.ShowDialog() == DialogResult.OK)
            {
                string path = backupFolder.SelectedPath;

                if (!string.IsNullOrEmpty(path))
                {
                    var databaseService = new DatabaseService();
                    bool success = databaseService.BackupDatabase(path);

                    if (success)
                    {
                        MessageBox.Show("Data backup successful!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Data backup failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to exit the program?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void mnuSale_Click(object sender, EventArgs e)
        {
            if (sale == null || sale.IsDisposed)
            {
                sale = new frmSale();
                sale.MdiParent = this;
                sale.Show();
            }
            else
                sale.Activate();
        }
        private void mnuCategories_Click(object sender, EventArgs e)
        {
            if (categories == null || categories.IsDisposed)
            {
                categories = new frmCategories();
                categories.MdiParent = this;
                categories.Show();
            }
            else
                categories.Activate();
        }

        private void mnuManufacturers_Click(object sender, EventArgs e)
        {
            if (manufacturers == null || manufacturers.IsDisposed)
            {
                manufacturers = new frmManufacturers();
                manufacturers.MdiParent = this;
                manufacturers.Show();
            }
            else
                manufacturers.Activate();
        }

        private void mnuProducts_Click(object sender, EventArgs e)
        {
            if (products == null || products.IsDisposed)
            {
                products = new frmProducts();
                products.MdiParent = this;
                products.Show();
            }
            else
                products.Activate();
        }

        private void mnuOrders_Click(object sender, EventArgs e)
        {
            if (orders == null || orders.IsDisposed)
            {
                orders = new frmOrders();
                orders.MdiParent = this;
                orders.Show();
            }
            else
                orders.Activate();
        }

        private void mnuCustomers_Click(object sender, EventArgs e)
        {
            if (customers == null || customers.IsDisposed)
            {
                customers = new frmCustomers();
                customers.MdiParent = this;
                customers.Show();
            }
            else
                customers.Activate();
        }

        private void mnuEmployees_Click(object sender, EventArgs e)
        {
            if (employees == null || employees.IsDisposed)
            {
                employees = new frmEmployees();
                employees.MdiParent = this;
                employees.Show();
            }
            else
                employees.Activate();
        }

        private void mnuProductStatistics_Click(object sender, EventArgs e)
        {
            if (productStatistics == null || productStatistics.IsDisposed)
            {
                productStatistics = new frmProductStatistics();
                productStatistics.MdiParent = this;
                productStatistics.Show();
            }
            else
                productStatistics.Activate();
        }

        private void mnuRevenueStatistics_Click(object sender, EventArgs e)
        {
            if (revenueStatistics == null || revenueStatistics.IsDisposed)
            {
                revenueStatistics = new frmRevenueStatistics();
                revenueStatistics.MdiParent = this;
                revenueStatistics.Show();
            }
            else
                revenueStatistics.Activate();
        }

        private void mnuSoftwareInformation_Click(object sender, EventArgs e)
        {
            about = new AboutBox();
            about.ShowDialog();
        }

        private void mnuUserGuide_Click(object sender, EventArgs e)
        {
            string helpurl = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer.exe";
            info.Arguments = helpurl + "index.html";
            Process.Start(info);
        }



        private void lblLink_Click(object sender, EventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer.exe";
            info.Arguments = "https://github.com/hkhuang07"; Process.Start(info);
        }
        #endregion

        #region System Process

        private void LogIn()
        {
            bool isAuthenticated = false;

            while (!isAuthenticated)
            {
                if (changePass == null || changePass.IsDisposed)
                {
                    logIn = new frmLogin();
                }

                //Hiển thị form đăng nhập
                if (logIn.ShowDialog() == DialogResult.OK)
                {
                    string userName = logIn.txtUserName.Text.Trim();
                    string passWord = logIn.txtPassword.Text.Trim();

                    //Kiểm tra thông tin hợp lệ
                    if (string.IsNullOrEmpty(userName))
                    {
                        MessageBox.Show("Username cannot be left blank!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        logIn.txtUserName.Focus();
                        continue;
                    }

                    if (string.IsNullOrEmpty(passWord))
                    {
                        MessageBox.Show("Password cannot be left blank!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        logIn.txtPassword.Focus();
                        continue;
                    }

                    // Tìm nhân viên theo tên check username và mã kiểm tra mất khẩu
                    var employee = _employeesService.GetByUserName(userName); // phương thức mới trong service

                    if (employee == null)
                    {
                        MessageBox.Show("Incorrect login name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logIn.txtUserName.Focus();
                        continue;
                    }

                    if (!BC.Verify(passWord, employee.Password)) // BC là BCrypt
                    {
                        MessageBox.Show("Incorrect password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logIn.txtPassword.Focus();
                        continue;
                    }

                    // Đăng nhập thành công
                    employeeName = employee.FullName;
                    isAuthenticated = true;
                    MessageBox.Show($"Wellcome {employeeName} !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (logIn.chkSave.Checked == false)
                    {
                        logIn.txtUserName.Clear();
                        //logIn.txtPassword.Clear();
                    }

                    if (employee.Role == true)
                        Administrator();
                    else
                        Member();
                }
                else
                {
                    // Người dùng bấm Cancel hoặc đóng form
                    logIn.Dispose();
                    return;
                }
            }
        }

        public void NotLoggedIn()
        {
            // Sáng đăng nhập
            mnuLogIn.Enabled = true;
           
            // Mờ tất cả
            mnuLogout.Enabled = false;
            mnuChangePass.Enabled = false;
            mnuBackup.Enabled = false;
            mnuRestore.Enabled = false;
            mnuSale.Enabled = false;
            managementToolStripMenuItem.Enabled = false;
            reportStatisticToolStripMenuItem.Enabled = false;
            mnuCategories.Enabled = false;
            mnuManufacturers.Enabled = false;
            mnuProducts.Enabled = false;
            mnuCustomers.Enabled = false;
            mnuEmployees.Enabled = false;
            mnuOrders.Enabled = false;
            mnuProductStatistics.Enabled = false;
            mnuRevenueStatistics.Enabled = false;

            tabLogin.Enabled = true;
            tabLogout.Enabled = false;
            tabChangepass.Enabled = false;
            tabRestore.Enabled = false;
            tabBackup.Enabled = false;
            tabSale.Enabled = false;
            tabManagement.Enabled = false;
            tabReportStatistics.Enabled = false;
            tabCategories.Enabled = false;
            tabManufacturer.Enabled = false;
            tabProducts.Enabled = false;
            tabCustomers.Enabled = false;
            tabEmployees.Enabled = false;
            tabOrders.Enabled = false;
            tabProductStatistics.Enabled = false;
            tabRevenueStatistics.Enabled = false;

            btnLogin.Enabled = true;
            btnLogout.Enabled = false;
            btnChangePass.Enabled = false;
            btnRestore.Enabled = false;
            btnBackup.Enabled = false;
            btnSale.Enabled = false;
            btnCategories.Enabled = false;
            btnManufacturers.Enabled = false;
            btnProducts.Enabled = false;
            btnProducts.Enabled = false;
            btnCustomers.Enabled = false;
            btnEmployees.Enabled = false;
            btnOrders.Enabled = false;
            btnProductStatistics.Enabled = false;
            btnRevenueStatistics.Enabled = false;

            // Hiển thị thông tin trên thanh trạng thái
            lblStatus.Text = "Not Logged In.";
        }
        private void Administrator()
        {
            // Sáng đăng nhập
            mnuLogIn.Enabled = false;
            // Mờ tất cả                   
            mnuLogout.Enabled = true;
            mnuChangePass.Enabled = true;
            mnuRestore.Enabled = true;
            mnuBackup.Enabled = true;
            mnuSale.Enabled = true;
            managementToolStripMenuItem.Enabled = true;
            reportStatisticToolStripMenuItem.Enabled = true;
            mnuCategories.Enabled = true;
            mnuManufacturers.Enabled = true;
            mnuProducts.Enabled = true;
            mnuCustomers.Enabled = true;
            mnuEmployees.Enabled = true;
            mnuOrders.Enabled = true;
            mnuProductStatistics.Enabled = true;
            mnuRevenueStatistics.Enabled = true;

            tabLogin.Enabled = false;
            tabLogout.Enabled = true;
            tabChangepass.Enabled = true;
            tabRestore.Enabled = true;
            tabBackup.Enabled = true;
            tabSale.Enabled = true;
            tabManagement.Enabled = true;
            tabReportStatistics.Enabled = true;
            tabCategories.Enabled = true;
            tabManufacturer.Enabled = true;
            tabProducts.Enabled = true;
            tabCustomers.Enabled = true;
            tabEmployees.Enabled = true;
            tabOrders.Enabled = true;
            tabProductStatistics.Enabled = true;
            tabRevenueStatistics.Enabled = true;

            btnLogin.Enabled = false;
            btnLogout.Enabled = true;
            btnChangePass.Enabled = true;
            btnRestore.Enabled = true;
            btnBackup.Enabled = true;
            btnSale.Enabled = true;
            btnCategories.Enabled = true;
            btnManufacturers.Enabled = true;
            btnProducts.Enabled = true;
            btnCustomers.Enabled = true;
            btnEmployees.Enabled = true;
            btnOrders.Enabled = true;
            btnProductStatistics.Enabled = true;
            btnRevenueStatistics.Enabled = true;

            //lblLink.Visible = false;
            lblStatus.Text = $"Wellcome Administrator: {employeeName} to the program!";
        }

        private void Member()
        {
            // Sáng đăng nhập
            mnuLogIn.Enabled = false;
            // Mờ tất cả
            mnuLogout.Enabled = true;
            mnuChangePass.Enabled = true;
            mnuRestore.Enabled = true;
            mnuBackup.Enabled = true;
            mnuSale.Enabled = true;
            managementToolStripMenuItem.Enabled = false;
            reportStatisticToolStripMenuItem.Enabled = false;
            tabLogin.Enabled = false;
            tabLogout.Enabled = true;
            tabChangepass.Enabled = true;
            tabRestore.Enabled = true;
            tabBackup.Enabled = true;
            tabSale.Enabled = true;
            tabManagement.Enabled = true;
            tabCategories.Enabled = true;
            tabManufacturer.Enabled = true;
            tabProducts.Enabled = true;
            tabCustomers.Enabled = true;
            tabEmployees.Enabled = false; // Nhân viên không có quyền truy cập vào danh sách nhân viên
            tabOrders.Enabled = true;
            tabProductStatistics.Enabled = false; // Nhân viên không có quyền truy cập vào thống kê sản phẩm
            tabRevenueStatistics.Enabled = false; // Nhân viên không có quyền truy cập vào thống kê doanh thu
            tabReportStatistics.Enabled = false;
            btnLogin.Enabled = false;
            btnLogout.Enabled = true;
            btnChangePass.Enabled = true;
            btnRestore.Enabled = true;
            btnBackup.Enabled = true;
            btnSale.Enabled = true;
            btnCategories.Enabled = true;
            btnManufacturers.Enabled = true;
            btnProducts.Enabled = true;
            btnCustomers.Enabled = true;
            btnEmployees.Enabled = false; // Nhân viên không có quyền truy cập vào danh sách nhân viên
            btnOrders.Enabled = true;
            btnProductStatistics.Enabled = false; // Nhân viên không có quyền truy cập vào thống kê sản phẩm
            btnRevenueStatistics.Enabled = false; // Nhân viên không có quyền truy cập vào thống kê doanh thu
            //lblLink.Visible = false;
            lblStatus.Text = $"Wellcome Member: {employeeName} to the program!";
        }

        #endregion


     
    }
}
