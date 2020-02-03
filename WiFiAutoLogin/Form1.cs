using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiFiAutoLogin
{
    public partial class Form1 : Form
    {
        private WebBrowser wb;
        private string username;
        private string password;
        int STEP = 0;
        public string filePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\wifiauth.config";
        private const string LOGIN_HOST = "n162.network-auth.com";
        private const string TEST_HOST = "www.google.com";
        private bool checkedUpdates = false;

        public Form1()
        {
            InitializeComponent();
            wb = new WebBrowser();
            wb.DocumentCompleted += wb_DocumentCompleted;
            if (!File.Exists(filePath))
            {
                new FormLogin(this).Show();
            }
            else
            {                
                loadAuthCreds();
                login();
            }
        }
        public void loadAuthCreds()
        {
            string[] config = File.ReadAllLines(filePath);
            byte[] key = new byte[8];
            string[] keyS = config[0].Split(' ');
            for(int i=0; i<8; i++)
            {
                key[i] = byte.Parse(keyS[i]);
            }
            username = StringUtil.Decrypt(config[1], key);
            password = StringUtil.Decrypt(config[2], key);
        }
        public void saveAuthCreds(string username, string password)
        {
            string outToFile = "";
            byte[] key = new byte[8];
            new Random().NextBytes(key);
            foreach (byte k in key)
                outToFile += k.ToString() + " ";
            outToFile += Environment.NewLine + StringUtil.Crypt(username, key);
            outToFile += Environment.NewLine + StringUtil.Crypt(password, key);
            File.WriteAllText(filePath, outToFile);
        }
        public void login()
        {
            timerLogin.Stop();
            STEP = 0;
            wb.Navigate("http://" + TEST_HOST);
        }

        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.Hide();
            if (STEP == 0 && wb.Url.Host.Equals(LOGIN_HOST))
            {
                wb.Document.GetElementById("email_field").SetAttribute("value", username);
                wb.Document.GetElementById("password_field").SetAttribute("value", password);
                wb.Document.GetElementById("sign_in_button").InvokeMember("Click");
                STEP++;
            }
            else if (STEP == 1 && wb.Url.Host.Equals(TEST_HOST))
            {
                notifyIcon1.ShowBalloonTip(2000, "Logged in", "logged in your network", ToolTipIcon.Info);
                timerLogin.Interval = 3602000;
                timerLogin.Start();
                checkUpdates();
            }
            else if(STEP == 0 && wb.Url.Host.Equals(TEST_HOST))
            {
                timerLogin.Interval = 5000;
                timerLogin.Start();
                checkUpdates();
            }
        }
        private void checkUpdates()
        {
            if (!checkedUpdates)
            {
                Updater.checkUpdates(notifyIcon1);
                checkedUpdates = true;
            }
        }

        private void timerLogin_Tick(object sender, EventArgs e)
        {
            login();
        }

        private void reconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            login();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Environment.Exit(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
        }

        private void changeCredsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormLogin(this).Show();
        }
    }
}
