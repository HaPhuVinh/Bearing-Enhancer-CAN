using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Bearing_Enhancer_CAN
{
    public partial class Form_BE : Form
    {
        public Form_BE()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string PJN = textBox_PJNum.Text;
            Bearing_Enhancer brgblock = new Bearing_Enhancer();
            dataGridView_Truss.DataSource = brgblock.Get_Lumber_Inv(PJN);
           
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
