using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfSystRGR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int acs_lvl = 1;
            if ((textBox1.Text == "") && (textBox2.Text == ""))
            {
                Form2 newForm = new Form2(acs_lvl);
                newForm.Show();
            } else if ((textBox1.Text == "manager") && (textBox2.Text == "123"))
            {
                acs_lvl = 2;
                Form2 newForm = new Form2(acs_lvl);
                newForm.Show();
            } else if ((textBox1.Text == "admin") && (textBox2.Text == "12213"))
            {
                acs_lvl = 3;
                Form2 newForm = new Form2(acs_lvl);
                newForm.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.UseSystemPasswordChar == false) textBox2.UseSystemPasswordChar = true;
            else textBox2.UseSystemPasswordChar = false;
        }
    }
}
