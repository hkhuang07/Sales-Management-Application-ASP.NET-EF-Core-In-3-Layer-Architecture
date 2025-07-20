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

namespace ElectronicsStore.Presentation
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            string helpURL = ConfigurationManager.AppSettings["HelpURL"]!.ToString();
            helpProvider1.HelpNamespace = helpURL + "login.html";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
