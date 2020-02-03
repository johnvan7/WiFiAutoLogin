using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiFiAutoLogin
{
    public partial class FormLogin : Form
    {
        Form1 Form1;

        public FormLogin(Form1 form1)
        {
            InitializeComponent();
            Form1 = form1;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Form1.saveAuthCreds(textBoxUser.Text, textBoxPass.Text);
            Form1.loadAuthCreds();
            Form1.login();
            this.Hide();
        }
    }
}
