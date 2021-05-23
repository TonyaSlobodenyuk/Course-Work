using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace courseWork
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
        }

        public string md5(string input)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (md5(maskedTextBox1.Text) == Properties.Settings.Default.pass)
            {
                AdminForm adminForm = new AdminForm(this);
                adminForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Ви ввели неправильний пароль! ", " На жаль ", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}
