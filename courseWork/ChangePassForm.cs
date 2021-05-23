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
    public partial class ChangePassForm : Form
    {
        AdminForm adminForm;
        public ChangePassForm(AdminForm adminForm)
        {
            InitializeComponent();
            this.adminForm = adminForm;
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


        private void ChangePass_Load(object sender, EventArgs e)
        {

        }

        private void ChangePass_FormClosing(object sender, FormClosingEventArgs e)
        {
            adminForm.Show();
        }

        private void savePass_Click(object sender, EventArgs e)
        {
            if (md5(passLast.Text) == Properties.Settings.Default.pass)
            {
                if (passNew.Text == passNewConfirm.Text)
                {
                    Properties.Settings.Default.pass = md5(passNew.Text);
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Ви успішно змінили пароль адміністратора!");
                }
                else
                {
                    MessageBox.Show("Введені паролі не збігаються, а треба щоб збігалися. Спробуйте ще раз.");
                }
            }
            else
            {
                MessageBox.Show("На жаль, попередній пароль введений невірно. Спробуйте ще разок.");
            }
        }
    }
}
