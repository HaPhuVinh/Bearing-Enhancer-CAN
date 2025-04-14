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
            
            List<string> list_Mat = new List<string> { "SPF", "DFL", "DFLN", "SP", "SYP", "HF" };
            List<string> list_Ply = new List<string> { "1", "2", "3", "4" };
            List<string> list_LumSize = new List<string> { "2x4", "2x6", "2x8", "2x10", "2x12" };
            List<string> list_Specie = new List<string> { "SPF", "DFL", "DFLN", "SP", "SYP", "HF" };
            List<string> list_DurationFactor = new List<string>() { "1.00", "1.15", "1.25", "1.33", "1.60" };
            list_DurationFactor = list_DurationFactor.Distinct().ToList();
            List<string> list_LocationType = new List<string> { "Interior", "Exterior" };

            No_Ply.DataSource = list_Ply;
            Lumber_Specie.DataSource = list_Specie;
            Lumber_Size.DataSource = list_LumSize;
            DOL_Column.DataSource = list_DurationFactor;
            Location_Type.DataSource = list_LocationType;
            Material.DataSource = list_Mat;

            // Đăng ký sự kiện thay đổi trạng thái ô
            dataGridView_Table.CellValueChanged += DataGridView_Table_CellValueChanged;
            dataGridView_Table.CurrentCellDirtyStateChanged += (s, ev) =>
            {
                if (dataGridView_Table.IsCurrentCellDirty)
                    dataGridView_Table.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            
        }
        
        private void DataGridView_Table_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 15 && e.RowIndex >= 0) // Cột Vertical_Block CheckBox
            {
                
                bool isChecked = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells[15].Value);
                DataGridViewRow row = dataGridView_Table.Rows[e.RowIndex];

                if (isChecked)
                {
                    using (Form_Vertical_Block_Info f2 = new Form_Vertical_Block_Info())
                    {
                        f2.Language = comboBox_Language.Text;
                        f2.Unit = comboBox_Unit.Text;
                        f2.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells[3].Value?.ToString()??"2x4";
                        f2.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells[2].Value?.ToString()??"SPF";
                        f2.ContactLength = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value?.ToString()??"3-08";

                        if (f2.ShowDialog() == DialogResult.OK)
                        {
                            dataGridView_Table.Rows[e.RowIndex].Cells[3].Value = f2.LumSize;
                            dataGridView_Table.Rows[e.RowIndex].Cells[2].Value = f2.LumSpecie;
                            dataGridView_Table.Rows[e.RowIndex].Cells[16].Value = f2.ContactLength;

                            Bearing_Enhancer BE = new Bearing_Enhancer();
                            BE.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells[0].Value.ToString();
                            BE.Ply = dataGridView_Table.Rows[e.RowIndex].Cells[1].Value.ToString();
                            BE.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells[2].Value.ToString();
                            BE.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells[3].Value.ToString();
                            Top_Plate_Info topPlate = new Top_Plate_Info();
                            topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells[5].Value.ToString();
                            topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells[6].Value.ToString();
                            topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells[7].Value.ToString();
                            topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells[8].Value.ToString();
                            topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells[9].Value.ToString());
                            topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value.ToString();
                            topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells[11].Value.ToString();
                            topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells[12].Value.ToString();
                            topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells[13].Value.ToString());
                            BE.TopPlateInfo = topPlate;

                            List<string> listVerBBlock = BE.Check_Bearing_Solution(BE.Ply,BE.LumSize,BE.LumSpecie, BE.TopPlateInfo,comboBox_Unit.Text,true);
                            (dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = listVerBBlock;
                            dataGridView_Table.Rows[e.RowIndex].Cells[14].Value = listVerBBlock[0];

                            row.DefaultCellStyle.BackColor = Color.Silver; // Đổi màu dòng
                        }
                        else
                        {
                            // Nếu người dùng đóng form mà không nhập, bỏ tick
                            dataGridView_Table.Rows[e.RowIndex].Cells[15].Value = false;

                            //Test Pull
                            
                        }
                    }
                }
                else
                {
                    dataGridView_Table.Rows[e.RowIndex].Cells[16].Value = "";
                    row.DefaultCellStyle.BackColor = default;

                    Bearing_Enhancer BE = new Bearing_Enhancer();
                    BE.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells[0].Value.ToString();
                    BE.Ply = dataGridView_Table.Rows[e.RowIndex].Cells[1].Value.ToString();
                    BE.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells[2].Value.ToString();
                    BE.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells[3].Value.ToString();
                    Top_Plate_Info topPlate = new Top_Plate_Info();
                    topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells[5].Value.ToString();
                    topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells[6].Value.ToString();
                    topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells[7].Value.ToString();
                    topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells[8].Value.ToString();
                    topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells[9].Value.ToString());
                    topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value.ToString();
                    topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells[11].Value.ToString();
                    topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells[12].Value.ToString();
                    topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells[13].Value.ToString());
                    BE.TopPlateInfo = topPlate;

                    List<string> listVerBBlock = BE.Check_Bearing_Solution(BE.Ply, BE.LumSize, BE.LumSpecie, BE.TopPlateInfo, comboBox_Unit.Text, false);
                    (dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = listVerBBlock;
                    dataGridView_Table.Rows[e.RowIndex].Cells[14].Value = listVerBBlock[0];
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView_Table.Rows.Clear();
            //Imperial_Or_Metric convert_Factor = new Imperial_Or_Metric(comboBox_Unit.Text);
            //TBE_Info tBE_Info = new TBE_Info(comboBox_Unit.Text);
            //try
            //{
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

                //LumberInventory lumberI = new LumberInventory();
                //List<LumberInventory> list_Lumber = lumberI.Get_Lumber_Inv(projectID);
                //List<string> list_Mat = list_BE.Select(x => x.TopPlateInfo.Material).Distinct().ToList();
                //List<string> list_LumSize = list_Lumber.Select(x => x.Lumber_Size).Distinct().ToList();
               //List<string> list_Specie = list_Lumber.Select(x => x.Lumber_SpeciesName).Distinct().ToList();
                //List<string> snow_DurationFactor = list_BE.Select(x => x.TopPlateInfo.DOL.DOL_Snow).Distinct().ToList();
                //List<string> live_DurationFactor = list_BE.Select(x => x.TopPlateInfo.DOL.DOL_Live).Distinct().ToList();
                //List<string> wind_DurationFactor = list_BE.Select(x => x.TopPlateInfo.DOL.DOL_Wind).Distinct().ToList();
                //list_DurationFactor.AddRange(snow_DurationFactor);
                //list_DurationFactor.AddRange(live_DurationFactor);
                //list_DurationFactor.AddRange(wind_DurationFactor);

                int i = 0;
                foreach (Bearing_Enhancer be in list_BE)
                {
                    List<string> list_BearingSolution = be.BearingSolution;

                    dataGridView_Table.Rows.Add(be.TrussName, be.Ply, be.LumSpecie, be.LumSize, be.TopPlateInfo.DOL.DOL_Snow,
                        be.TopPlateInfo.JointID, be.TopPlateInfo.XLocation, be.TopPlateInfo.YLocation, be.TopPlateInfo.Location_Type, be.TopPlateInfo.Reaction, be.TopPlateInfo.BearingWidth,
                        be.TopPlateInfo.RequireWidth, be.TopPlateInfo.Material, be.TopPlateInfo.LoadTransfer, list_BearingSolution[0]);
                    (dataGridView_Table.Rows[i].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = list_BearingSolution;
                    
                    i++;
                }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            
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
