using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.IO;
using System.Diagnostics;

namespace InfSystRGR
{
    public partial class Form2 : Form
    {
        string nameTable, nameAct;
        string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=12213;Database=rgr_atrs";
        DataSet dataSet;
        public NpgsqlDataAdapter dataAdapter;
        public NpgsqlConnection npgsqlConnection;
        public NpgsqlCommand npgComm;
        int acs_lvl = 0;
        Button myButt = new Button();
        ComboBox myWhence = new ComboBox();
        ComboBox myWhither = new ComboBox();
        DateTimePicker myDeparture = new DateTimePicker();
        DateTimePicker myArrival = new DateTimePicker();
        ComboBox myCompany = new ComboBox();
        ComboBox myPlane = new ComboBox();
        TextBox myText1 = new TextBox();
        TextBox myText2 = new TextBox();
        TextBox myText3 = new TextBox();
        Label labWhence = new Label();
        Label labWhither = new Label();
        Label labDeparture = new Label();
        Label labArrival = new Label();
        Label labCompany = new Label();
        Label labPlane = new Label();
        Label labText1 = new Label();
        Label labText2 = new Label();
        Label labText3 = new Label();

        public Form2()
        {
            InitializeComponent();
            npgsqlConnection = new NpgsqlConnection();
            npgComm = new NpgsqlCommand();
            npgComm.Connection = npgsqlConnection;
            npgsqlConnection.ConnectionString = connectionString;
            LoadNameTables();
            LoadNameAct();
            loadControls();
        }

        public Form2(int acs_lvl)
        {
            bool exBD = false;
            InitializeComponent();
            npgsqlConnection = new NpgsqlConnection();
            npgComm = new NpgsqlCommand();
            npgComm.Connection = npgsqlConnection;
            npgsqlConnection.ConnectionString = "Server=localhost;Port=5432;User Id=postgres;Password=12213;";
            npgsqlConnection.Open();
            npgComm.CommandText = "select datname from pg_database";
            NpgsqlDataReader dr = npgComm.ExecuteReader();
            while (dr.Read())
            {
                if (dr["datname"].ToString() == "rgr_atrs") exBD = true;
            }
            npgsqlConnection.Close();
            npgsqlConnection.Open();
            if (exBD == false)
            {
                try {
                    npgComm.CommandText = "Create database rgr_atrs";
                    npgComm.ExecuteNonQuery();
                    label4.Text = "Log: Создание базы данных успешно выполнено.";
                }
                catch (Exception ex)
                {
                    label4.Text = "Log: Создание базы данных провалилось";
                    MessageBox.Show(ex.ToString());
                }
            }
            npgsqlConnection.Close();
            npgsqlConnection.ConnectionString = connectionString;
            this.acs_lvl = acs_lvl;
            LoadNameTables();
            LoadNameAct();
            loadControls();
        }

        public void loadControls()
        {
            labWhence.Text = "Whence:";
            labWhence.AutoSize = true;
            labWhither.Text = "Whither:";
            labWhither.AutoSize = true;
            labDeparture.Text = "Departure:";
            labDeparture.AutoSize = true;
            labArrival.Text = "Arrival:";
            labArrival.AutoSize = true;
            labCompany.Text = "Company:";
            labCompany.AutoSize = true;
            labPlane.Text = "AirPlane:";
            labPlane.AutoSize = true;
            myButt.AutoSize = true;
            labText1.AutoSize = true;
            labText2.AutoSize = true;
            labText3.AutoSize = true;
            myButt.Click += new System.EventHandler(myButt_Click);
        }

        public void LoadNameCity()
        {
            npgsqlConnection.Open();
            npgComm.CommandText = "select name from dir_city";
            NpgsqlDataReader dr = npgComm.ExecuteReader();
            myPlane.Items.Clear();
            while (dr.Read())
            {
                string nameComp = dr["name"].ToString();
                myWhence.Items.Add(nameComp);
                myWhither.Items.Add(nameComp);
            }
            npgsqlConnection.Close();
        }

        public void LoadNameComp()
        {
            npgsqlConnection.Open();
            npgComm.CommandText = "select name from dir_company";
            NpgsqlDataReader dr = npgComm.ExecuteReader();
            myCompany.Items.Clear();
            while (dr.Read())
            {
                string nameComp = dr["name"].ToString();
                myCompany.Items.Add(nameComp);
            }
            npgsqlConnection.Close();
        }

        public void LoadNamePlane()
        {
            npgsqlConnection.Open();
            npgComm.CommandText = "select name from dir_plane";
            NpgsqlDataReader dr = npgComm.ExecuteReader();
            myPlane.Items.Clear();
            while (dr.Read())
            {
                string nameComp = dr["name"].ToString();
                myPlane.Items.Add(nameComp);
            }
            npgsqlConnection.Close();
        }

        public void LoadNameTables()
        {
            npgsqlConnection.Open();
            npgComm.CommandText = "select table_Name from information_Schema.Tables where table_schema='public'";
            NpgsqlDataReader dr = npgComm.ExecuteReader();
            comboBox1.Items.Clear();
            while (dr.Read())
            {
                nameTable = dr["TABLE_NAME"].ToString();
                comboBox1.Items.Add(nameTable);
            }
            npgsqlConnection.Close();
        }
        
        public void LoadNameAct()
        {
            comboBox2.Items.Add("Показать");
            if (acs_lvl > 1)
            {
                comboBox2.Items.Add("Добавить");
                comboBox2.Items.Add("Удалить");
                comboBox2.Items.Add("Изменить");
                if (acs_lvl > 2)
                {
                    comboBox2.Items.Add("Dump");
                    comboBox2.Items.Add("Load");
                    comboBox2.Items.Add("DropBD");
                }
            }
        }

        public void showTable()
        {
            if (nameTable == "flights")
            {
                try
                {
                    string commSellect = "select * from flights";
                    //string commSellect = "select flights.id as " + '"' + "id" + '"' + ", whence, whither, departure_time,";
                    //commSellect += "arrival_time, dir_company.name as " + '"' + "company" + '"' + ", dir_plane.name";
                    //commSellect += " as " + '"' + "airplane" + '"' + ", platform from flights join dir_company on ";
                    //commSellect += "flights.id_company = dir_company.id join dir_plane on flights.id_airplane = dir_plane.id";
                    npgsqlConnection.Open();
                    dataAdapter = new NpgsqlDataAdapter(commSellect, npgsqlConnection);
                    NpgsqlCommandBuilder commandBuilder = new NpgsqlCommandBuilder(dataAdapter);
                    dataSet = new DataSet();
                    dataAdapter.Fill(dataSet, nameTable);
                    dataGridView1.DataSource = dataSet.Tables[0];
                    npgsqlConnection.Close();
                }
                catch (Exception ex) { label4.Text = "Log: Show fails"; MessageBox.Show(ex.ToString()); }
            }
            else
            {
                try
                {
                    npgsqlConnection.Open();
                    dataAdapter = new NpgsqlDataAdapter("select * from " + nameTable, npgsqlConnection);
                    NpgsqlCommandBuilder commandBuilder = new NpgsqlCommandBuilder(dataAdapter);
                    dataSet = new DataSet();
                    dataAdapter.Fill(dataSet, nameTable);
                    dataGridView1.DataSource = dataSet.Tables[0];
                    npgsqlConnection.Close();
                    label4.Text = "Log: Таблица отображена";
                }
                catch (Exception ex) { label4.Text = "Log: Show fails"; MessageBox.Show(ex.ToString()); }
            }
        }

        public void Dump(string path, string database)
        {
            StreamWriter sw = new StreamWriter("DBBackup.bat");
            StringBuilder strSB = new StringBuilder("cd C:\\Program Files\\PostgreSQL\\10\\bin\\\r\n");
            strSB.Append("C:\r\n");
            strSB.Append("SET PGPASSWORD=12213\r\n");
            strSB.Append("pg_dump -h localhost -U postgres -F c -f " + path + " " + database);
            sw.WriteLine(strSB);
            sw.Close();
            Process processDB = Process.Start("DBBackup.bat");
        }

        public void Restore(string path, string database)
        {
            StreamWriter sw = new StreamWriter("DBRestore.bat");
            StringBuilder strSB = new StringBuilder("cd C:\\Program Files\\PostgreSQL\\10\\bin\\\r\n");
            strSB.Append("C:\r\n");
            strSB.Append("SET PGPASSWORD=12213\r\n");
            strSB.Append("pg_restore -h localhost -U postgres -F c -d " + database + " " + path);
            sw.WriteLine(strSB);
            sw.Close();
            Process processDB = Process.Start("DBRestore.bat");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nameTable = comboBox1.SelectedItem.ToString();
            showTable();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            nameAct = comboBox2.SelectedItem.ToString();
            if (nameAct == "Показать")
            {
                showTable();
            }

            if (nameAct == "Добавить")
            {
                myButt.Text = "Добавить";
                if (nameTable == "flights")
                {
                    LoadNameCity();
                    LoadNameComp();
                    LoadNamePlane();
                    labText1.Text = "Platform:";
                    labWhence.Location = new Point(5, 7);
                    myWhence.Location = new Point(65, 3);
                    labWhither.Location = new Point(5, 32);
                    myWhither.Location = new Point(65, 28);
                    labDeparture.Location = new Point(5, 57);
                    myDeparture.Location = new Point(65, 53);
                    labArrival.Location = new Point(5, 82);
                    myArrival.Location = new Point(65, 78);
                    labCompany.Location = new Point(5, 107);
                    myCompany.Location = new Point(65, 103);
                    labPlane.Location = new Point(5, 132);
                    myPlane.Location = new Point(65, 128);
                    labText1.Location = new Point(5, 157);
                    myText1.Location = new Point(65, 153);
                    myButt.Location = new Point(65, 178);
                    panel1.Controls.Add(labWhence);
                    panel1.Controls.Add(myWhence);
                    panel1.Controls.Add(labWhither);
                    panel1.Controls.Add(myWhither);
                    panel1.Controls.Add(labDeparture);
                    panel1.Controls.Add(myDeparture);
                    panel1.Controls.Add(labArrival);
                    panel1.Controls.Add(myArrival);
                    panel1.Controls.Add(labCompany);
                    panel1.Controls.Add(myCompany);
                    panel1.Controls.Add(labPlane);
                    panel1.Controls.Add(myPlane);
                    panel1.Controls.Add(labText1);
                    panel1.Controls.Add(myText1);
                    panel1.Controls.Add(myButt);
                } else
                {
                    try { dataAdapter.Update(dataSet, nameTable); label4.Text = "Log: Таблица сохранена"; }
                    catch (Exception ex) { label4.Text = "Log: update fails"; MessageBox.Show(ex.ToString()); }
                }
            }

            if (nameAct == "Изменить")
            {
                myButt.Text = "Изменить";
                if (nameTable == "flights")
                {
                    LoadNameCity();
                    LoadNameComp();
                    LoadNamePlane();
                    labText1.Text = "Platform:";
                    labText2.Text = "Id:";
                    labText2.Location = new Point(5, 7);
                    myText2.Location = new Point(65, 3);
                    labWhence.Location = new Point(5, 32);
                    myWhence.Location = new Point(65, 28);
                    labWhither.Location = new Point(5, 57);
                    myWhither.Location = new Point(65, 53);
                    labDeparture.Location = new Point(5, 82);
                    myDeparture.Location = new Point(65, 78);
                    labArrival.Location = new Point(5, 107);
                    myArrival.Location = new Point(65, 103);
                    labCompany.Location = new Point(5, 132);
                    myCompany.Location = new Point(65, 128);
                    labPlane.Location = new Point(5, 157);
                    myPlane.Location = new Point(65, 153);
                    labText1.Location = new Point(5, 182);
                    myText1.Location = new Point(65, 178);
                    myButt.Location = new Point(65, 203);
                    panel1.Controls.Add(labText2);
                    panel1.Controls.Add(myText2);
                    panel1.Controls.Add(labWhence);
                    panel1.Controls.Add(myWhence);
                    panel1.Controls.Add(labWhither);
                    panel1.Controls.Add(myWhither);
                    panel1.Controls.Add(labDeparture);
                    panel1.Controls.Add(myDeparture);
                    panel1.Controls.Add(labArrival);
                    panel1.Controls.Add(myArrival);
                    panel1.Controls.Add(labCompany);
                    panel1.Controls.Add(myCompany);
                    panel1.Controls.Add(labPlane);
                    panel1.Controls.Add(myPlane);
                    panel1.Controls.Add(labText1);
                    panel1.Controls.Add(myText1);
                    panel1.Controls.Add(myButt);
                }
                else
                {
                    labText1.Text = "Id:";
                    labText2.Text = "Поле:";
                    labText3.Text = "Значение:";
                    labText1.Location = new Point(5, 7);
                    myText1.Location = new Point(65, 3);
                    labText2.Location = new Point(5, 32);
                    myText2.Location = new Point(65, 28);
                    labText3.Location = new Point(5, 57);
                    myText3.Location = new Point(65, 53);
                    myButt.Location = new Point(65, 78);
                    panel1.Controls.Add(labText1);
                    panel1.Controls.Add(myText1);
                    panel1.Controls.Add(labText2);
                    panel1.Controls.Add(myText2);
                    panel1.Controls.Add(labText3);
                    panel1.Controls.Add(myText3);
                    panel1.Controls.Add(myButt);
                }
            }

            if (nameAct == "Удалить")
            {

                myButt.Text = "Удалить";
                labText1.Text = "Id:";
                labText1.Location = new Point(5, 7);
                myText1.Location = new Point(65, 3);
                myButt.Location = new Point(65, 28);
                panel1.Controls.Add(labText1);
                panel1.Controls.Add(myText1);
                panel1.Controls.Add(myButt);
            }

            if (nameAct == "Dump")
            {
                saveFileDialog1.Filter = "Dump Files|*.dump|All Files|*.*";
                saveFileDialog1.Title = "Save a dump file";
                saveFileDialog1.FileName = "New dump";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Dump(saveFileDialog1.FileName.Replace(@"\", @"\\").Replace(" ", "\" \""), "rgr_atrs");
                    label4.Text = "Log: DB Saved";
                }
            }

            if (nameAct == "Load")
            {
                npgsqlConnection.Open();
                try
                {
                    npgComm.CommandText = "create table flights (id serial, whence char(15) not null, whither char(15) not null, departure_time char(30) not null, arrival_time char(30) not null, company char(10) not null, airplane char(10) not null, platform int not null);";
                    npgComm.ExecuteNonQuery();
                    npgComm.CommandText = "create table passangers (id serial, passport char(11) not null, lugg_weight int not null, gender char(5) not null);";
                    npgComm.ExecuteNonQuery();
                    npgComm.CommandText = "create table registers (id serial, id_flight int not null, id_passanger int not null, presence int not null);";
                    npgComm.ExecuteNonQuery();
                    npgComm.CommandText = "create table dir_city (id serial, name char(15) not null);";
                    npgComm.ExecuteNonQuery();
                    npgComm.CommandText = "create table dir_company (id serial, name char(10) not null);";
                    npgComm.ExecuteNonQuery();
                    npgComm.CommandText = "create table dir_plane (id serial, name char(10) not null);";
                    npgComm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                npgsqlConnection.Close();
                openFileDialog1.Filter = "Dump Files|*.dump|All Files|*.*";
                openFileDialog1.Title = "Select a dump file";
                openFileDialog1.FileName = "";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Restore(openFileDialog1.FileName.Replace(@"\", @"\\").Replace(" ", "\" \""), "rgr_atrs");
                    label4.Text = "Log: DB Opened";
                }
                LoadNameTables();
            }

            if (nameAct == "DropBD")
            {
                npgsqlConnection.ConnectionString = "Server=localhost;Port=5432;User Id=postgres;Password=12213;";
                npgsqlConnection.Open();
                try
                {
                    //npgComm.CommandText = "DROP SCHEMA public CASCADE;";
                    //npgComm.ExecuteNonQuery();
                    //npgComm.CommandText = "CREATE SCHEMA public;";
                    //npgComm.ExecuteNonQuery();
                    //npgComm.CommandText = "GRANT ALL ON SCHEMA public TO postgres;";
                    //npgComm.ExecuteNonQuery();
                    //npgComm.CommandText = "GRANT ALL ON SCHEMA public TO public;";
                    //npgComm.ExecuteNonQuery();
                    npgComm.CommandText = "Drop database rgr_atrs;";
                    npgComm.ExecuteNonQuery();
                    label4.Text = "Log: Удаление БД успешно выполнено.";
                }
                catch (Exception ex)
                {
                    label4.Text = "Log: Удаление БД прошло не успешно";
                    MessageBox.Show(ex.ToString());
                }
                npgsqlConnection.Close();
                npgsqlConnection.ConnectionString = connectionString;
                LoadNameTables();
            }
        }

        private void myButt_Click(object sender, EventArgs e)
        {
            if ((nameAct == "Добавить") && (nameTable == "flights"))
            {
                npgsqlConnection.Open();
                string strSQL = "INSERT INTO flights(whence, whither, departure_time, arrival_time, company, airplane, platform) values("+"'"+myWhence.Text+"'"+", "+"'"+myWhither.Text+"'"+", " + "'" + myDeparture.Text + "'" + ", " + "'" + myArrival.Text + "'" + ", " + "'" + myCompany.Text + "'" + ", " + "'" + myPlane.Text + "'" + ", " + Convert.ToInt32(myText1.Text) + ")";
                npgComm.CommandText = strSQL;
                try
                {
                    npgComm.ExecuteNonQuery();
                    label4.Text = "Log: Добавление записи успешно выполнено.";
                }
                catch (Exception ex)
                {
                    label4.Text = "Log: Добавление записи провалилось";
                    MessageBox.Show(ex.ToString());
                }
                npgsqlConnection.Close();
                showTable();
            }

            if (nameAct == "Удалить")
            {
                npgsqlConnection.Open();
                string strSQL = "DELETE FROM "+nameTable+" where id IN ("+myText1.Text+")";
                npgComm.CommandText = strSQL;
                try
                {
                    npgComm.ExecuteNonQuery();
                    label4.Text = "Log: Удаление записи успешно выполнено.";
                }
                catch (Exception ex)
                {
                    label4.Text = "Log: Удаление записи провалилось";
                    MessageBox.Show(ex.ToString());
                }
                npgsqlConnection.Close();
                showTable();
            }
        }
    }
}
