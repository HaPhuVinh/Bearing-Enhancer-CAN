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

                LumberInventory lumberI = new LumberInventory();
                List<LumberInventory> list_Lumber = lumberI.Get_Lumber_Inv(projectID);

                List<string> list_Mat = list_BE.Select(x=>x.TopPlateInfo.Material).Distinct().ToList();
                List<string> list_Ply = list_BE.Select(x=>x.Ply).Distinct().ToList();
                List<string> list_LumSize = list_Lumber.Select(x=>x.Lumber_Size).Distinct().ToList();
                List<string> list_Specie = list_Lumber.Select(x => x.Lumber_SpeciesName).Distinct().ToList();

                No_Ply.DataSource = list_Ply;
                Lumber_Specie.DataSource = list_Specie;
                Lumber_Size.DataSource = list_LumSize;
                Material.DataSource = list_Mat;

                foreach (Bearing_Enhancer be in list_BE)
                {
                    List<string> durFactors = new List<string>();
                    durFactors.Add( be.TopPlateInfo.DOL.DOL_Snow);
                    durFactors.Add(be.TopPlateInfo.DOL.DOL_Live);
                    durFactors.Add(be.TopPlateInfo.DOL.DOL_Wind);
                    DOL_Column.DataSource = durFactors;

                    dataGridView_Table.Rows.Add(be.TrussName,be.Ply,be.LumSpecie,be.LumSize, be.TopPlateInfo.DOL.DOL_Snow, 
                        be.TopPlateInfo.JointID, be.TopPlateInfo.XLocation, be.TopPlateInfo.Reaction, be.TopPlateInfo.BearingWidth,
                        be.TopPlateInfo.RequireWidth, be.TopPlateInfo.Material, be.TopPlateInfo.LoadTransfer);
                }
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

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
