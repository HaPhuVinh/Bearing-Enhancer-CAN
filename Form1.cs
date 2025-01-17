using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
            string projectPath = textBox_PJNum.Text;
            string trussesPath = $"{textBox_PJNum.Text}\\Trusses";
            string tempPath = $"{projectPath}\\Temp";
            string[] arrPath = projectPath.Split('\\');
            string projectID = arrPath[arrPath.Length-1];
            string[] txtPathes = Directory.GetFiles(tempPath, "*.txt");
            bool existFile = txtPathes.Count() == 1;
            if (existFile)
            {
                string txtName = Path.GetFileNameWithoutExtension(txtPathes[0]);
                List<Bearing_Enhancer>list_BE = new List<Bearing_Enhancer>();
                Bearing_Enhancer BE = new Bearing_Enhancer();
                list_BE=BE.Get_Bearing_Info(txtPathes[0]);
                
                //Top_Plate_Info TOP = new Top_Plate_Info();
                //TOP.Get_TOP_Info(txtPath[0], "A");
            }
            else
            {
                MessageBox.Show($"You need to choose the Open All icon in CS Engineer and export a .txt file via Bluebeam!");
            }



        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
