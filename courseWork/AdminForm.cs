using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;

namespace courseWork
{
    public partial class AdminForm : Form
    {
        AuthForm authForm; 
        public AdminForm(AuthForm authForm) 
        {
            InitializeComponent();
            this.authForm = authForm;
        }

        private void AdminForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            authForm.Close(); 
        }

        private void selected_servicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServicesControlForm servicesControlForm = new ServicesControlForm(this);
            servicesControlForm.Show();
            this.Hide();
        }

        private void selected_toursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TravelControlForm appartamentsControlForm = new TravelControlForm(this);
            appartamentsControlForm.Show();
            this.Hide();
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {

        }

        List<int> appsIDs = new List<int>();
        List<int> servIDs = new List<int>();
        List<string[]> services = new List<string[]>();

        private void AdminForm_Activated(object sender, EventArgs e)
        {
            refreshAll();
        }

        private void refreshAll()
        {
            this.toursTableAdapter.Fill(this.travel_agencyDataSet.Tours);
            this.selected_servicesTableAdapter.Fill(this.travel_agencyDataSet.Selected_services);
            this.customersTableAdapter.Fill(this.travel_agencyDataSet.Customers);

            OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.travel_agencyConnectionString);
            conn.Open();
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id_services , Name_services ,Price FROM Services";
            OleDbDataReader cmdResult = cmd.ExecuteReader();

            comboBox3.Items.Clear();
            servIDs.Clear();
            while (cmdResult.Read())
            {
                servIDs.Add(int.Parse(cmdResult["Id_services"].ToString()));
                comboBox3.Items.Add(cmdResult["Name_services"].ToString() + ", " + cmdResult["Price"].ToString()+" гривень");
            }
            cmdResult.Close();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                cmd.CommandText = "SELECT Id_selected FROM Selected_services WHERE Id_customer = " + dataGridView1.Rows[i].Cells[0].Value.ToString();
                cmdResult = cmd.ExecuteReader();
                while (cmdResult.Read())
                {
                    OleDbCommand cmd2 = conn.CreateCommand();
                    cmd2.CommandText = "SELECT Price FROM Services WHERE Id_services = " + cmdResult["Id_services"];
                    int cmdResult2 = int.Parse(cmd2.ExecuteScalar().ToString());
                    int cureInt = 0;
                    if (dataGridView1.Rows[i].Cells[7].Value != null)
                    {
                        cureInt = Convert.ToInt32(dataGridView1.Rows[i].Cells[7].Value.ToString());
                    }
                    dataGridView1.Rows[i].Cells[7].Value = (cureInt + cmdResult2 * ((Convert.ToDateTime(dataGridView1.Rows[i].Cells[5].Value.ToString()) - Convert.ToDateTime(dataGridView1.Rows[i].Cells[4].Value.ToString())).Days + 1)).ToString();
                }
                cmdResult.Close();
            }

            cmd.CommandText = "SELECT Id_services , Name_services ,Price FROM Services";
            cmdResult = cmd.ExecuteReader();
            while (cmdResult.Read())
            {
                services.Add(new string[3] { cmdResult["Id_selected"].ToString(), cmdResult["Name_services"].ToString(), cmdResult["Price"].ToString() });
            }
            cmdResult.Close();

            cmd.CommandText = "SELECT Id_tour, Number_of_seats, Price_for_place FROM Tours";
            cmdResult = cmd.ExecuteReader();
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
           
            appsIDs.Clear();
            while (cmdResult.Read())
            {
                appsIDs.Add(int.Parse(cmdResult["Id_tour"].ToString()));
                comboBox1.Items.Add("Путівка " + cmdResult["Number_of_seats"].ToString() + ", " + cmdResult["Price_for_place"].ToString() + " гривень");
                comboBox2.Items.Add("Путівка " + cmdResult["Number_of_seats"].ToString() + ", " + cmdResult["Price_for_place"].ToString() + " гривень");
            
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[3].Value.ToString() == cmdResult["Id_tour"].ToString())
                    {
                        dataGridView1.Rows[i].Cells[6].Value = cmdResult["Number_of_seats"].ToString();
                        int cureInt = 0;
                        if (dataGridView1.Rows[i].Cells[7].Value != null)
                        {
                           cureInt = Convert.ToInt32(dataGridView1.Rows[i].Cells[7].Value.ToString());
                        }
                        dataGridView1.Rows[i].Cells[7].Value = (cureInt + (int.Parse(cmdResult["Price_for_place"].ToString()) * ((Convert.ToDateTime(dataGridView1.Rows[i].Cells[5].Value.ToString()) - Convert.ToDateTime(dataGridView1.Rows[i].Cells[4].Value.ToString())).Days+1))).ToString();
                    }
                }
            }
            cmdResult.Close();
            conn.Close();
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int j = 0;
                foreach (int app in appsIDs)
                {
                    if (app.ToString() == dataGridView1.SelectedRows[0].Cells[3].Value.ToString())
                    {
                        comboBox1.SelectedIndex = j;
                    }
                    j++;
                }
                foreach (string[] service in services)
                {
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        if (dataGridView2.Rows[i].Cells[2].Value.ToString() == service[0])
                        {
                            dataGridView2.Rows[i].Cells[3].Value = service[1];
                            dataGridView2.Rows[i].Cells[4].Value = service[2];
                        }
                    }
                }
            }
        }

        private void saveClientChanges()
        {
            customersBindingSource.EndEdit();
            customersTableAdapter.Update(travel_agencyDataSet);
            travel_agencyDataSet.AcceptChanges();
            refreshAll();
        }

        private void saveServicesChanges()
        {
            customers_selected_servicesSelected_servicesBindingSource.EndEdit();
            selected_servicesTableAdapter.Update(travel_agencyDataSet);
            travel_agencyDataSet.AcceptChanges();
            refreshAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            travel_agencyDataSet.ToursRow tour = travel_agencyDataSet.Tours.NewToursRow();
            tour.Id_tour = appsIDs[comboBox2.SelectedIndex];
            travel_agencyDataSet.Customers.AddCustomersRow(textBox4.Text, textBox3.Text, tour, dateTimePicker4.Value.Date, dateTimePicker3.Value.Date);
            this.saveClientChanges();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int j = 0;
                foreach (int app in appsIDs)
                {
                    if (app.ToString() == dataGridView1.SelectedRows[0].Cells[3].Value.ToString())
                    {
                        comboBox1.SelectedIndex = j;
                    }
                    j++;
                }


                foreach (string[] service in services)
                {
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        if (dataGridView2.Rows[i].Cells[2].Value.ToString() == service[0])
                        {
                            dataGridView2.Rows[i].Cells[3].Value = service[1];
                            dataGridView2.Rows[i].Cells[4].Value = service[2];
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            travel_agencyDataSet.CustomersRow client = travel_agencyDataSet.Customers.NewCustomersRow();
            client.Id_customer = int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            travel_agencyDataSet.ServicesRow service = travel_agencyDataSet.Services.NewServicesRow();
            service.Id_services = servIDs[comboBox3.SelectedIndex];
            travel_agencyDataSet.Selected_services.AddSelected_servicesRow(client, service);
            this.saveServicesChanges();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                dataGridView1.SelectedRows[0].Cells[3].Value = appsIDs[comboBox1.SelectedIndex];
            }
            foreach (Control c in panel1.Controls)
            {
                foreach (Binding b in c.DataBindings)
                {
                    b.WriteValue();
                }
            }
            this.saveClientChanges();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Ви дійсно хочете видалити обраного клієнта?\nЦю дію неможливо скасувати.", "Підтвердіть видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (DataGridViewRow item in dataGridView1.SelectedRows)
                {
                    customersBindingSource.RemoveAt(item.Index);
                }
                this.saveClientChanges();
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Ви дійсно хочете видалити вибрану послугу?\nЦю дію неможливо скасувати.", "Підтвердіть видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (DataGridViewRow item in dataGridView2.SelectedRows)
                {
                    customers_selected_servicesSelected_servicesBindingSource.RemoveAt(item.Index);
                }
                this.saveServicesChanges();
            }
        }

        private void showAvailableToursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRowCollection rows = travel_agencyDataSet.Tables["Tours"].Rows;
            string output = "";
            int counter = 0;
            foreach (DataRow row in rows)
            {
                var clientInf = travel_agencyDataSet.Tables["Customers"].Select("Id_tour = " + row[0].ToString());
                if (clientInf.Length == 0)
                {
                    counter++;
                    output += "\n\n"+ "Путівка: " + row[3].ToString() + ". Країна: " + row[1].ToString() + ". Вартість: " + row[2].ToString();
                }
            }
            MessageBox.Show("Список доступних путівок: " + counter.ToString() + output, "Список доступних путівок");
        }

        private void showAllToursToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRowCollection rows = travel_agencyDataSet.Tables["Tours"].Rows;
            string output = "";
            int counter = 0;
            foreach (DataRow row in rows)
            {
                counter++;
                var clientInf = travel_agencyDataSet.Tables["Customers"].Select("Id_tour = " + row[0].ToString());
                if (clientInf.Length > 0)
                {
                    output += "\n\n"  + "Путівка: " + row[3].ToString() + ". Країна: " + row[1].ToString() + ". Вартість: " + row[2].ToString() + ". Зареєстрованих клієнтів: " + clientInf.Length.ToString();
                }
                else
                {
                    output += "\n\n"  + "Путівка: " + row[3].ToString() + ". Країна: " + row[1].ToString() + ". Вартість: " + row[2].ToString();
                }
            }
            MessageBox.Show("Список путівок: " + counter.ToString() + output, "Список путівок");
        }

        private void showSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showSearchToolStripMenuItem.Checked = !showSearchToolStripMenuItem.Checked;
            panel5.Visible = showSearchToolStripMenuItem.Checked;
            if (showSearchToolStripMenuItem.Checked)
            {
                this.Height = 734;
            }
            else
            {
                this.Height = 625;
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void searchClient_Click(object sender, EventArgs e)
        {
            refreshAll();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (searchByName.Checked && dataGridView1.Rows[i].Cells[1].Value.ToString().IndexOf(searchClientName.Text) < 0)
                {
                    dataGridView1.Rows.RemoveAt(i);
                }
                if (searchByArrivalDate.Checked)
                {
                    if (searchFClientArrivalDate.Items[searchFClientArrivalDate.SelectedIndex].ToString() == "=")
                    {
                        if (Convert.ToDateTime(dataGridView1.Rows[i].Cells[4].Value.ToString()) != searchClientArrivalDate.Value)
                        {
                            dataGridView1.Rows.RemoveAt(i);
                        }
                    }
                    if (searchFClientArrivalDate.Items[searchFClientArrivalDate.SelectedIndex].ToString() == ">")
                    {
                        if (Convert.ToDateTime(dataGridView1.Rows[i].Cells[4].Value.ToString()) <= searchClientArrivalDate.Value)
                        {
                            dataGridView1.Rows.RemoveAt(i);
                        }
                    }
                    if (searchFClientArrivalDate.Items[searchFClientArrivalDate.SelectedIndex].ToString() == "<")
                    {
                        if (Convert.ToDateTime(dataGridView1.Rows[i].Cells[4].Value.ToString()) >= searchClientArrivalDate.Value)
                        {
                            dataGridView1.Rows.RemoveAt(i);
                        }
                    }
                    if (searchFClientArrivalDate.Items[searchFClientArrivalDate.SelectedIndex].ToString() == "<=")
                    {
                        if (Convert.ToDateTime(dataGridView1.Rows[i].Cells[4].Value.ToString()) > searchClientArrivalDate.Value)
                        {
                            dataGridView1.Rows.RemoveAt(i);
                        }
                    }
                    if (searchFClientArrivalDate.Items[searchFClientArrivalDate.SelectedIndex].ToString() == ">=")
                    {
                        if (Convert.ToDateTime(dataGridView1.Rows[i].Cells[4].Value.ToString()) < searchClientArrivalDate.Value)
                        {
                            dataGridView1.Rows.RemoveAt(i);
                        }
                    }
                }
                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            refreshAll();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangePassForm changePassForm = new ChangePassForm(this);
            changePassForm.Show();
            this.Hide();
        }
    }
}
