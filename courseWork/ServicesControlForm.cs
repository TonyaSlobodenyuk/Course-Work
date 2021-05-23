using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace courseWork
{
    public partial class ServicesControlForm : Form
    {
        AdminForm adminForm;
        public ServicesControlForm(AdminForm adminForm)
        {
            InitializeComponent();
            this.adminForm = adminForm;
        }

        private void saveChanges()
        {
            servicesBindingSource.EndEdit();
            servicesTableAdapter.Update(travel_agencyDataSet);
            travel_agencyDataSet.AcceptChanges();
            this.servicesTableAdapter.Fill(this.travel_agencyDataSet.Services);
        }

        private void ServicesControlForm_Load(object sender, EventArgs e)
        {
          
            this.servicesTableAdapter.Fill(this.travel_agencyDataSet.Services);

        }

        private void ServicesControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            adminForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control c in panel1.Controls)
            {
                foreach (Binding b in c.DataBindings)
                {
                    b.WriteValue();
                }
            }
            this.saveChanges();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Ви дійсно хочете видалити вибрану послугу?\nЦю дію неможливо скасувати.", "Підтвердіть видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (DataGridViewRow item in dataGridView1.SelectedRows)
                {
                    servicesBindingSource.RemoveAt(item.Index);
                }
                saveChanges();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            travel_agencyDataSet.Services.AddServicesRow(textBox4.Text, Convert.ToInt32(numericUpDown1.Value));
            this.saveChanges();
        }
    }
}
