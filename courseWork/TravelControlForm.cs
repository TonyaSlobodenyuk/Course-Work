using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace courseWork
{
    public partial class TravelControlForm : Form
    {
        AdminForm adminForm;
        public TravelControlForm(AdminForm adminForm)
        {
            InitializeComponent();
            this.adminForm = adminForm;
        }

        private void saveChanges()
        {
            toursBindingSource.EndEdit();
            toursTableAdapter.Update(travel_agencyDataSet);
            travel_agencyDataSet.AcceptChanges();
            this.toursTableAdapter.Fill(this.travel_agencyDataSet.Tours);
        }

        private void AppartamentsControlForm_Load(object sender, EventArgs e)
        {
           
            this.toursTableAdapter.Fill(this.travel_agencyDataSet.Tours);

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

            if (MessageBox.Show("Ви дійсно хочете видалити обрану путівку?\nЦю дію неможливо скасувати.", "Підтвердіть видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (DataGridViewRow item in dataGridView1.SelectedRows)
                {
                    toursBindingSource.RemoveAt(item.Index);
                }
                saveChanges();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            travel_agencyDataSet.Tours.AddToursRow(comboBox2.Text, Convert.ToInt32(numericUpDown3.Value), Convert.ToInt32(numericUpDown4.Value), 0);
            this.saveChanges();

        }

        private void AppartamentsControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            adminForm.Show();
        }
    }
}
