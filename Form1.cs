using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Bearing_Enhancer_CAN
{
    public partial class Form_BearingEnhacerCAN : Form
    {
        public Form_BearingEnhacerCAN()
        {
            InitializeComponent();
            this.Load += Form_BearingEnhacerCAN_Load;
        }
        
        private void Form_BearingEnhacerCAN_Load(object sender, EventArgs e)
        {
            comboBox_Language.Items.Add("English");
            comboBox_Language.Items.Add("France");
            comboBox_Language.Text = "English";

            comboBox_Unit.Items.Add("Imperial");
            comboBox_Unit.Items.Add("Metric");
            comboBox_Unit.Text = "Imperial";
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView_Table.Rows.Clear();
            //Imperial_Or_Metric convert_Factor = new Imperial_Or_Metric(comboBox_Unit.Text);
            TBE_Info tBE_Info = new TBE_Info(comboBox_Unit.Text);
            try
            {
                string projectPath = textBox_PJNum.Text;
                string trussesPath = $"{textBox_PJNum.Text}\\Trusses";
                string tempPath = $"{projectPath}\\Temp";
                string[] arrPath = projectPath.Split('\\');
                string projectID = arrPath[arrPath.Length - 1];
                string[] txtPathes = Directory.GetFiles(tempPath, "*.txt");

                string txtName = Path.GetFileNameWithoutExtension(txtPathes[0]);
                List<Bearing_Enhancer> list_BE = new List<Bearing_Enhancer>();
                Bearing_Enhancer BE = new Bearing_Enhancer();
                
                list_BE = BE.Get_Bearing_Info(txtPathes[0],comboBox_Language.Text, comboBox_Unit.Text);

                LumberInventory lumberI = new LumberInventory();
                List<LumberInventory> list_Lumber = lumberI.Get_Lumber_Inv(projectID);

                List<string> list_Mat = list_BE.Select(x => x.TopPlateInfo.Material).Distinct().ToList();
                List<string> list_Ply =new List<string> {"1","2","3","4"};
                List<string> list_LumSize = list_Lumber.Select(x => x.Lumber_Size).Distinct().ToList();
                List<string> list_Specie = list_Lumber.Select(x => x.Lumber_SpeciesName).Distinct().ToList();
                List<string> snow_DurationFactor = list_BE.Select(x => x.TopPlateInfo.DOL.DOL_Snow).Distinct().ToList();
                List<string> live_DurationFactor = list_BE.Select(x => x.TopPlateInfo.DOL.DOL_Live).Distinct().ToList();
                List<string> wind_DurationFactor = list_BE.Select(x => x.TopPlateInfo.DOL.DOL_Wind).Distinct().ToList();
                List<string> list_DurationFactor = new List<string>();
                list_DurationFactor.AddRange(snow_DurationFactor);
                list_DurationFactor.AddRange(live_DurationFactor);
                list_DurationFactor.AddRange(wind_DurationFactor);
                list_DurationFactor = list_DurationFactor.Distinct().ToList();

                No_Ply.DataSource = list_Ply;
                Lumber_Specie.DataSource = list_Specie;
                Lumber_Size.DataSource = list_LumSize;
                DOL_Column.DataSource = list_DurationFactor;
                Material.DataSource = list_Mat;
                int i = 0;
                foreach (Bearing_Enhancer be in list_BE)
                {
                    List<string> list_BearingSolution = be.BearingSolution;

                    dataGridView_Table.Rows.Add(be.TrussName, be.Ply, be.LumSpecie, be.LumSize, be.TopPlateInfo.DOL.DOL_Snow,
                        be.TopPlateInfo.JointID, be.TopPlateInfo.XLocation, be.TopPlateInfo.YLocation, be.TopPlateInfo.Reaction, be.TopPlateInfo.BearingWidth,
                        be.TopPlateInfo.RequireWidth, be.TopPlateInfo.Material, be.TopPlateInfo.LoadTransfer);
                    (dataGridView_Table.Rows[i].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = list_BearingSolution;
                    i++;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
