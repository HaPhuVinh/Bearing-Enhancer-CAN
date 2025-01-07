using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            string path = String.Concat("C:\\SST-EA\\Client\\Projects\\",PJN,"\\Temp");
            string[] txtFiles = Directory.GetFiles(path,"*.txt");
            bool exist = txtFiles.Count() > 0;

            //if (existFile)
            //{

            //    Top_Plate_Info TOP = new Top_Plate_Info();
            //    TOP.Get_TOP_Info(PJN, "A");
            //}
            //else
            //{
            //    MessageBox.Show($"Please create {PJN}.txt file!");
            //}
            
            
           
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
