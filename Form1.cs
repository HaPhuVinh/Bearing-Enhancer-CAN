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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;

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
            comboBox_Language.Items.Add("French");
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
            dataGridView_Table.CellValueChanged += DataGridViewTable_CellValueChanged;
            dataGridView_Table.CurrentCellDirtyStateChanged += (s, ev) =>
            {
                if (dataGridView_Table.IsCurrentCellDirty)
                    dataGridView_Table.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            // Đăng ký sự kiện hover chuột
            dataGridView_Table.CellMouseEnter += dataGridView_CellMouseEnter;
            dataGridView_Table.CellMouseLeave += dataGridView_CellMouseLeave;

            // Đăng ký sự kiện kiểm tra ô
            dataGridView_Table.CellValidating += dataGridView_CellValidating;
            //Đăng kí sự kiện Double_Click
            this.dataGridView_Table.CellDoubleClick += dataGridViewTable_CellDoubleClick;
            //Đăng kí sự kiện Double_RightClick
            dataGridView_Table.MouseDown += dataGridViewTable_MouseDown;
        }

        private void button1_Click(object sender, EventArgs e)// Button Check
        {
            dataGridView_Table.Rows.Clear();
            //Imperial_Or_Metric convert_Factor = new Imperial_Or_Metric(comboBox_Unit.Text);
            //TBE_Info tBE_Info = new TBE_Info(comboBox_Unit.Text);
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
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

}

        private void DataGridViewTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)// Sự kiện Ô thay đổi
        {
                if (e.ColumnIndex == 15 && e.RowIndex >= 0) // Cột Vertical_Block CheckBox
                {

                    bool isChecked = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells[15].Value);
                    DataGridViewRow row = dataGridView_Table.Rows[e.RowIndex];
                    string[] columnsToCheck = { "No_Ply", "Location_Type", "Reaction", "Brg_Width", "Req_Width", "Material" };
                    var cell = dataGridView_Table.Rows[e.RowIndex].Cells["Vertical_Block"];
                    if (isChecked)
                    {
                        bool checkNull = false;
                        string cellValue;
                        foreach (string colName in columnsToCheck)
                        {
                            cellValue = dataGridView_Table.Rows[e.RowIndex].Cells[colName].Value?.ToString();
                            checkNull = string.IsNullOrWhiteSpace(cellValue);
                            if (checkNull)
                            {
                                break;
                            }

                        }

                        if (checkNull)
                        {
                            MessageBox.Show("Please check and input data into columns: No.-Ply, Location-Type, Reaction, Bearing-Width, Required-Width, Material");
                        }
                        else
                        {
                            using (Form_Vertical_Block_Info f2 = new Form_Vertical_Block_Info())
                            {
                                f2.Language = comboBox_Language.Text;
                                f2.Unit = comboBox_Unit.Text;
                                f2.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells[3].Value?.ToString() ?? "2x4";
                                f2.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "SPF";
                                f2.ContactLength = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value?.ToString();

                                if (f2.ShowDialog() == DialogResult.OK)
                                {
                                    Imperial_Or_Metric iom = new Imperial_Or_Metric(comboBox_Unit.Text,comboBox_Language.Text);
                                    dataGridView_Table.Rows[e.RowIndex].Cells[3].Value = f2.LumSize;
                                    dataGridView_Table.Rows[e.RowIndex].Cells[2].Value = f2.LumSpecie;
                                    dataGridView_Table.Rows[e.RowIndex].Cells[16].Value = iom.Unit=="Imperial"? f2.ContactLength: Convert.ToString(Convert_String_Inch(f2.ContactLength)*iom.miliFactor);

                                    Bearing_Enhancer BE = new Bearing_Enhancer();
                                    BE.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells[0].Value?.ToString();
                                    BE.Ply = dataGridView_Table.Rows[e.RowIndex].Cells[1].Value?.ToString();
                                    BE.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells[2].Value?.ToString();
                                    BE.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells[3].Value?.ToString();
                                    Top_Plate_Info topPlate = new Top_Plate_Info();
                                    topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells[5].Value?.ToString();
                                    topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells[6].Value?.ToString();
                                    topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells[7].Value?.ToString();
                                    topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells[8].Value?.ToString();
                                    topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells[9].Value?.ToString());
                                    topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value?.ToString();
                                    topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells[11].Value?.ToString();
                                    topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells[12].Value?.ToString();
                                    topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells[13].Value.ToString());
                                    BE.TopPlateInfo = topPlate;
                                    string contLength = dataGridView_Table.Rows[e.RowIndex].Cells[16].Value.ToString();

                                    List<string> listVerBBlock = BE.Check_Bearing_Solution(BE.Ply, BE.LumSize, BE.LumSpecie, BE.TopPlateInfo, comboBox_Unit.Text,comboBox_Language.Text, true, contLength);
                                    (dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = listVerBBlock;
                                    dataGridView_Table.Rows[e.RowIndex].Cells[14].Value = listVerBBlock[0];

                                    row.DefaultCellStyle.BackColor = Color.AntiqueWhite; // Đổi màu dòng
                                }
                                else
                                {
                                    // Nếu người dùng đóng form mà không nhập, bỏ tick
                                    dataGridView_Table.Rows[e.RowIndex].Cells[15].Value = false;

                                }
                            }

                        }

                    }
                    else
                    {
                        bool checkNull = false;
                        string cellValue;
                        foreach (string colName in columnsToCheck)
                        {
                            cellValue = dataGridView_Table.Rows[e.RowIndex].Cells[colName].Value?.ToString();
                            checkNull = string.IsNullOrWhiteSpace(cellValue);
                            if (checkNull)
                            {
                                break;
                            }
                        }

                        if (checkNull)
                        {
                            MessageBox.Show("Please check and input data into columns: No.-Ply, Location-Type, Reaction, Bearing-Width, Required-Width, Material");
                        }
                        else
                        {
                            // Nếu bỏ tick, xóa giá trị trong ô Contact Length và đổi màu dòng về mặc định
                            dataGridView_Table.Rows[e.RowIndex].Cells[16].Value = "";
                            row.DefaultCellStyle.BackColor = default;

                            Bearing_Enhancer BE = new Bearing_Enhancer();
                            BE.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells[0].Value?.ToString();
                            BE.Ply = dataGridView_Table.Rows[e.RowIndex].Cells[1].Value?.ToString();
                            BE.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells[2].Value?.ToString();
                            BE.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells[3].Value?.ToString();
                            Top_Plate_Info topPlate = new Top_Plate_Info();
                            topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells[5].Value?.ToString();
                            topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells[6].Value?.ToString();
                            topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells[7].Value?.ToString();
                            topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells[8].Value?.ToString();
                            topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells[9].Value?.ToString());
                            topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value?.ToString();
                            topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells[11].Value?.ToString();
                            topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells[12].Value?.ToString();
                            topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells[13].Value.ToString());
                            BE.TopPlateInfo = topPlate;

                            List<string> listHorBBlock = BE.Check_Bearing_Solution(BE.Ply, BE.LumSize, BE.LumSpecie, BE.TopPlateInfo, comboBox_Unit.Text,comboBox_Language.Text, false);
                            (dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = listHorBBlock;
                            dataGridView_Table.Rows[e.RowIndex].Cells[14].Value = listHorBBlock[0];
                        }
                    }
                }

                if (e.ColumnIndex == 17 || e.ColumnIndex == 15 || e.ColumnIndex == 14 && e.RowIndex >= 0) // Cột Checked || Vertical-Block || Bearing-Solution
            {
                    bool isChecked17 = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells[17].Value);
                    DataGridViewRow row = dataGridView_Table.Rows[e.RowIndex];
                    var cell = dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"];
                    if (isChecked17)
                    {
                        if (string.IsNullOrWhiteSpace(cell.FormattedValue.ToString()))
                        {
                            MessageBox.Show("Please check and input relevant data!");
                            row.Cells[17].Value = false;
                        }
                        else
                        {

                            string chosenSolution = cell.Value.ToString();
                            Bearing_Enhancer beItem;

                            if (chosenSolution.Contains("5%"))
                            {
                                beItem = new Bearing_Enhancer_5Percent(chosenSolution);
                            }
                            else if (chosenSolution.Contains("Hor"))
                            {
                                beItem = new Bearing_Enhancer_HorBlock(chosenSolution);
                            }
                            else if (chosenSolution.Contains("Ver"))
                            {
                                beItem = new Bearing_Enhancer_VerBlock(chosenSolution);
                            }
                            else if (chosenSolution.Contains("TBE"))
                            {
                                beItem = new Bearing_Enhancer_TBE(chosenSolution);
                            }
                            else
                            {
                                return;
                            }

                            beItem.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells[0].Value?.ToString();
                            beItem.Ply = dataGridView_Table.Rows[e.RowIndex].Cells[1].Value?.ToString();
                            beItem.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells[2].Value?.ToString();
                            beItem.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells[3].Value?.ToString();
                            Top_Plate_Info topPlate = new Top_Plate_Info();
                            topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells[5].Value?.ToString();
                            topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells[6].Value?.ToString();
                            topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells[7].Value?.ToString();
                            topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells[8].Value?.ToString();
                            topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells[9].Value?.ToString());
                            topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value?.ToString();
                            topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells[11].Value?.ToString();
                            topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells[12].Value?.ToString();
                            topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells[13].Value.ToString());
                            beItem.TopPlateInfo = topPlate;
                            string contactLength = dataGridView_Table.Rows[e.RowIndex].Cells[16].Value?.ToString();


                            string theNote = $"Jnt_{beItem.TopPlateInfo.JointID}: {beItem.Generate_Enhancer_Note(chosenSolution, comboBox_Language.Text, comboBox_Unit.Text)}";
                            row.Cells[18].Value = theNote;
                        }
                    }
                    else
                    {
                        row.Cells[18].Value = "";
                    }

                }
        }
        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)//Sự kiện rời khỏi Ô
        {
            // Lấy tên cột
            string headerText = dataGridView_Table.Columns[e.ColumnIndex].HeaderText;

            // Kiểm tra cột "No.-Ply"
            if (headerText.Equals("No.-Ply"))
            {
                if (e.FormattedValue.ToString() == "")
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "No.-Ply is not empty!";
                    e.Cancel = true;
                }
                else
                {
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = string.Empty;
                }
            }
            // Kiểm tra cột "Lumber-Specie"
            if (headerText.Equals("Lumber-Specie"))
            {
                if (e.FormattedValue.ToString() == "")
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "Lumber-Specie is not empty!";
                    e.Cancel = true;
                }
                else
                {
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = string.Empty;
                }
            }
            // Kiểm tra cột "Lumber-Size"
            if (headerText.Equals("Lumber-Size"))
            {
                if (e.FormattedValue.ToString() == "")
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "Lumber-Size is not empty!";
                    e.Cancel = true;
                }
                else
                {
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = string.Empty;
                }
            }
            // Kiểm tra cột "Location-Type"
            if (headerText.Equals("Location-Type"))
            {
                if (e.FormattedValue.ToString() == "")
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "Location-Type is not empty!";
                    e.Cancel = true;
                }
                else
                {
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = string.Empty;
                }
            }
            // Kiểm tra cột "Reaction"
            if (headerText.Equals("Reaction"))
            {
                Regex regexReaction = new Regex(@"^\d+$");
                string input = e.FormattedValue.ToString();
                if (!regexReaction.IsMatch(input))
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "Please input Reaction as interger!";
                    e.Cancel = true;
                }
                else
                {
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = string.Empty;
                }
            }
            // Kiểm tra cột "Bearing-Width"
            if (headerText.Equals("Bearing-Width"))
            {
                Regex regexBearingWidth = new Regex(@"^\d+(-\d+){1,2}$");
                string input = e.FormattedValue.ToString();
                if (!regexBearingWidth.IsMatch(input))
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "Please input Bearing-Width as the format: 3-08, 5-08, 1-00-02!";
                    e.Cancel = true;
                }
                else
                {
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = string.Empty;
                }
            }
            // Kiểm tra cột "Required-Width"
            if (headerText.Equals("Required-Width"))
            {
                Regex regexRequiredWidth = new Regex(@"^\d+(-\d+){1,2}$");
                string input = e.FormattedValue.ToString();
                if (!regexRequiredWidth.IsMatch(input))
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "Please input Required-Width as the format: 3-08, 5-08, 1-00-00!";
                    e.Cancel = true;
                }
                else
                {
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = string.Empty;
                }
            }
            // Kiểm tra cột "Material"
            if (headerText.Equals("Material"))
            {
                if (e.FormattedValue.ToString() == "")
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "Material is not empty!";
                    e.Cancel = true;
                }
                else
                {
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = string.Empty;
                }
            }

            if ((e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11) && e.RowIndex >= 0)// Cột Reaction||Bearing-Width||Required-Width
            {
                string inputR = dataGridView_Table.Rows[e.RowIndex].Cells[9].Value?.ToString();
                string inputBW = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value?.ToString();
                string inputRW = dataGridView_Table.Rows[e.RowIndex].Cells[11].Value?.ToString();

                // Kiểm tra null / rỗng trước
                if (!string.IsNullOrWhiteSpace(inputR) && !string.IsNullOrWhiteSpace(inputBW) && !string.IsNullOrWhiteSpace(inputRW))
                {
                    Regex regexBearing = new Regex(@"^\d+(-\d+){1,2}$");
                    Regex regexInteger = new Regex(@"^\d+$");

                    // Chỉ xử lý nếu Reaction là số nguyên, và BW & RW đúng định dạng
                    if (regexInteger.IsMatch(inputR) && regexBearing.IsMatch(inputBW) && regexBearing.IsMatch(inputRW))
                    {
                        try
                        {
                            double react = double.Parse(inputR);
                            double bear_W = double.TryParse(inputBW, out double resultW) ? resultW : Convert_String_Inch(inputBW);
                            double bear_Wrq = double.TryParse(inputRW, out double resultR) ? resultR : Convert_String_Inch(inputRW);

                            if (bear_Wrq != 0)
                            {
                                double loadTransfer = Math.Round((react - react * bear_W / bear_Wrq), 0);
                                dataGridView_Table.Rows[e.RowIndex].Cells[13].Value = loadTransfer;
                            }
                            else
                            {
                                dataGridView_Table.Rows[e.RowIndex].ErrorText = "Required-Width must not be zero.";
                                e.Cancel = true;
                            }
                        }
                        catch
                        {
                            dataGridView_Table.Rows[e.RowIndex].ErrorText = "Error calculating Load Transfer.";
                            e.Cancel = true;
                        }


                    }
                }
            }

            int[] columnsNumber = { 1, 2, 3, 8, 9, 10, 11, 12, 13 };
            bool isColumn = columnsNumber.Any(c => c == e.ColumnIndex);
            if (isColumn && e.RowIndex >= 0) // Cột No.-Ply, Lumber-Spiecie, Lumber-Size, Location-Type, Reaction, Bearing-Width, Required-Width, Material
            {
                bool checkNullBS = false;
                string cellValueBS;
                foreach (int colName in columnsNumber)
                {
                    cellValueBS = dataGridView_Table.Rows[e.RowIndex].Cells[colName].Value?.ToString();
                    checkNullBS = string.IsNullOrWhiteSpace(cellValueBS);

                    if (checkNullBS)
                    {
                        break;
                    }
                }

                if (!checkNullBS)
                {
                    string inputR = dataGridView_Table.Rows[e.RowIndex].Cells[9].Value?.ToString();
                    string inputBW = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value?.ToString();
                    string inputRW = dataGridView_Table.Rows[e.RowIndex].Cells[11].Value?.ToString();

                    // Kiểm tra null / rỗng trước
                    if (!string.IsNullOrWhiteSpace(inputR) && !string.IsNullOrWhiteSpace(inputBW) && !string.IsNullOrWhiteSpace(inputRW))
                    {
                        Regex regexBearing = new Regex(@"^\d+(-\d+){1,2}$");
                        Regex regexInteger = new Regex(@"^\d+$");

                        // Chỉ xử lý nếu Reaction là số nguyên, và BW & RW đúng định dạng
                        if (regexInteger.IsMatch(inputR) && regexBearing.IsMatch(inputBW) && regexBearing.IsMatch(inputRW))
                        {
                            Bearing_Enhancer BE = new Bearing_Enhancer();
                            BE.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells[0].Value?.ToString();
                            BE.Ply = dataGridView_Table.Rows[e.RowIndex].Cells[1].Value?.ToString();
                            BE.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells[2].Value?.ToString();
                            BE.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells[3].Value?.ToString();
                            Top_Plate_Info topPlate = new Top_Plate_Info();
                            topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells[5].Value?.ToString();
                            topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells[6].Value?.ToString();
                            topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells[7].Value?.ToString();
                            topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells[8].Value?.ToString();
                            topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells[9].Value?.ToString());
                            topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells[10].Value?.ToString();
                            topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells[11].Value?.ToString();
                            topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells[12].Value?.ToString();
                            topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells[13].Value.ToString());
                            BE.TopPlateInfo = topPlate;

                            List<string> listHorBBlock = BE.Check_Bearing_Solution(BE.Ply, BE.LumSize, BE.LumSpecie, BE.TopPlateInfo, comboBox_Unit.Text, comboBox_Language.Text, false);
                            (dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = listHorBBlock;
                            dataGridView_Table.Rows[e.RowIndex].Cells[14].Value = listHorBBlock[0];
                        }
                    }
                                        
                }
            }
        }

        private int hoveredRowIndex = -1;
        private void dataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex != hoveredRowIndex)
            {
                // Nếu đã có dòng đang hover, trả lại màu cũ
                if (hoveredRowIndex >= 0)
                {
                    ResetRowStyle(hoveredRowIndex);
                }

                hoveredRowIndex = e.RowIndex;

                var row = dataGridView_Table.Rows[e.RowIndex];
                row.DefaultCellStyle.ForeColor = Color.Blue;
            }
        }

        private void dataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == hoveredRowIndex && e.RowIndex >= 0)
            {
                var row = dataGridView_Table.Rows[e.RowIndex];
                row.DefaultCellStyle.ForeColor = dataGridView_Table.DefaultCellStyle.ForeColor;
                hoveredRowIndex = -1;
            }
        }
        private void ResetRowStyle(int rowIndex)
        {
            var row = dataGridView_Table.Rows[rowIndex];
            row.DefaultCellStyle.Font = dataGridView_Table.DefaultCellStyle.Font;
        }

        private void dataGridViewTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 17) // Cột Checked
            {
                bool currentValue = true;
                foreach (DataGridViewRow row in dataGridView_Table.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        row.Cells[17].Value = currentValue;
                    }
                }
            }
        }

        private void dataGridViewTable_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Clicks == 2)//Double rightclick
            {
                var hit = dataGridView_Table.HitTest(e.X, e.Y);
                int rowIndex = hit.RowIndex;
                int colIndex = hit.ColumnIndex;
                bool currentValue = false;
                if(colIndex == 17) // Cột Checked
                {
                    foreach (DataGridViewRow row in dataGridView_Table.Rows)
                    {
                        row.Cells[17].Value = currentValue;
                    }
                }
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
        double Convert_String_Inch(string xxx)
        {
            string s = xxx.Trim();
            string[] ss = s.Split('-');
            double q;
            if (ss.Length > 2)
            {
                q = double.Parse(ss[0]) * 12 + double.Parse(ss[1]) + double.Parse(ss[2]) / 16;
            }
            else if (ss.Length > 1)
            {
                q = double.Parse(ss[0]) + double.Parse(ss[1]) / 16;
            }
            else
            {
                q = double.Parse(ss[0]) / 16;
            }

            return q;
        }

        private void btn_export_data_Click(object sender, EventArgs e)
        {
            ExportToExcel(dataGridView_Table);
        }

        private void ExportToExcel(DataGridView dgv)
        {
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = "Bearing Enhancer Report.xlsx"
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Excel.Application excelApp = new Excel.Application();
                        Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
                        Excel.Worksheet worksheet = workbook.Sheets[1];
                        worksheet.Name = "Bearing Enhancer Data";

                        // Ghi tiêu đề cột
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1] = dgv.Columns[i].HeaderText;
                        }

                        // Ghi dữ liệu
                        for (int i = 0; i < dgv.Rows.Count; i++)
                        {
                            for (int j = 0; j < dgv.Columns.Count; j++)
                            {
                                object val = dgv.Rows[i].Cells[j].Value;
                                Excel.Range cell = (Excel.Range)worksheet.Cells[i + 2, j + 1];
                                cell.NumberFormat = "@"; // Định dạng text
                                cell.Value2 = val?.ToString();
                            }
                        }
                        //Tô màu dòng tiêu đề
                        Excel.Range headerRange = worksheet.get_Range("A1", "S1"); // Giả sử 4 cột
                        headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        //Auto fit cột
                        worksheet.Columns.AutoFit();
                        // Lưu file tại vị trí đã chọn
                        workbook.SaveAs(sfd.FileName);
                        workbook.Close();
                        excelApp.Quit();

                        MessageBox.Show("Excel export successful!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error when exporting Excel: " + ex.Message);
                    }
                }
            }
        }

        private void btn_Add_Note_Click(object sender, EventArgs e)
        {
            List<(string TrussName, string JointID, string Note)> listItem = new List<(string TrussName, string JointID, string Note)> ();
            foreach (DataGridViewRow row in dataGridView_Table.Rows)
            {
                bool valueCol17 = Convert.ToBoolean(row.Cells[17].Value);
                if (!row.IsNewRow && valueCol17)
                {
                    (string TrussName, string JointID, string Note) theNoteItem = (row.Cells[0].Value?.ToString(), row.Cells[5].Value?.ToString(), row.Cells[18].Value?.ToString());
                    if (!string.IsNullOrEmpty(theNoteItem.TrussName)&&!string.IsNullOrEmpty(theNoteItem.Note))
                    {
                        listItem.Add(theNoteItem);
                    }
                }
            }
            if (listItem.Count != 0)
            {
                string mssg = "Are you sure? The below trusses will be added:\n";
                foreach (var item in listItem)
                {
                    mssg += $"Truss: {item.TrussName} - Joint: {item.JointID},\n";
                }
                DialogResult result = MessageBox.Show(mssg, "Notification", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        foreach (DataGridViewRow row in dataGridView_Table.Rows)
                        {
                            bool valueCol17 = Convert.ToBoolean(row.Cells[17].Value);
                            if (!row.IsNewRow && valueCol17)
                            {
                                (string TrussName, string Note) theNoteItem = (row.Cells[0].Value?.ToString(), row.Cells[18].Value?.ToString());
                                if (!string.IsNullOrEmpty(theNoteItem.TrussName) && !string.IsNullOrEmpty(theNoteItem.Note))
                                {
                                    // Gọi hàm thêm vào XML
                                    Add_Note_ToTruss(textBox_PJNum.Text, theNoteItem);
                                }
                            }
                        }
                        MessageBox.Show("Add note successful!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error when adding note: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Not found any items!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        void Add_Note_ToTruss(string path, (string TrussName, string Note) item)
        {
            string projectPath = textBox_PJNum.Text;
            string trussesPath = $"{textBox_PJNum.Text}\\Trusses";
            string[] arrPath = projectPath.Split('\\');
            string projectID = arrPath[arrPath.Length - 1];
            string xmlFilePath = Path.Combine(trussesPath, $"{item.TrussName}.tdlTruss");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode rootNode, noteNode;
            rootNode = xmlDoc.DocumentElement;
            noteNode = rootNode.SelectSingleNode("//Notes");
            XmlElement newNote = xmlDoc.CreateElement("Note");
            newNote.SetAttribute("OutputGroup", "5");
            newNote.InnerText = item.Note;
            noteNode.AppendChild(newNote);

            // Lưu lại tài liệu XML
            xmlDoc.Save(xmlFilePath);
        }
    }

}
