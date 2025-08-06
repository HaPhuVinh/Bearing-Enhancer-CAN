using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        ComboBox currentComboBox = null;
        ToolTip warningToolTip = new ToolTip
        {
            IsBalloon = false,
            ToolTipIcon = ToolTipIcon.Warning,
            ToolTipTitle = "Warning",
            UseFading = true,
            UseAnimation = true,
            AutoPopDelay = 0,
            InitialDelay = 0,
            ReshowDelay = 0,
            ShowAlways = true
        };

        public List<Bearing_Enhancer> list_Original_Bearing;
        public Form_BearingEnhacerCAN()
        {
            InitializeComponent();
            this.Load += Form_BearingEnhacerCAN_Load;
        }
        
        private void Form_BearingEnhacerCAN_Load(object sender, EventArgs e)
        {
            this.Text = $"Bearing Enhancer CAN {Application.ProductVersion}";

            comboBox_Language.Items.Add("English");
            comboBox_Language.Items.Add("French");
            comboBox_Language.Text = "English";

            comboBox_Unit.Items.Add("Imperial");
            comboBox_Unit.Items.Add("Metric");
            comboBox_Unit.Text = "Imperial";
            
            List<string> list_Mat = new List<string> { "SPF", "DFL", "DFLN", "SP", "SYP", "HF" };
            List<string> list_Ply = new List<string> { "1", "2", "3", "4", "5", "6" };
            List<string> list_LumSize = new List<string> { "2x4", "2x6", "2x8", "2x10", "2x12" };
            List<string> list_Specie = new List<string> { "SPF", "DFL", "DFLN", "SP", "SYP", "HF" };
            List<string> list_DurationFactor = new List<string>() {"0.90","1.00", "1.05","1.10", "1.15", "1.25", "1.33", "1.60" };
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

            //Gắn sự kiện EditingControlShowing
            dataGridView_Table.EditingControlShowing += dataGridView_Table_EditingControlShowing;

            //Đăng kí sự kiện Đóng Form
            this.FormClosing += Form_BearingEnhacerCAN_FormClosing;

            //Kiểm tra cập nhật phiên bản
            try
            {
                string currentVersion = Application.ProductVersion;
                using (var client = new System.Net.WebClient())
                {
                    string latestVersion = client.DownloadString(@"S:\@ICS Engineering\04. Spreadsheet & Tool\Bearing Calculator CAN\Bearing Enhancer CAN_New\Latest Version\Latest_Version.txt").Trim();
                    if (new Version(currentVersion) < new Version(latestVersion))
                    {
                        var result = MessageBox.Show(
                        $"A new version ({latestVersion}) is available. Do you want to update now?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            string updateUrl = @"S:\@ICS Engineering\04. Spreadsheet & Tool\Bearing Calculator CAN\Bearing Enhancer CAN_New\Latest Version\Bearing Enhancer CAN.zip";
                            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            var file = new FileInfo(appDirectory);
                            string tempFile = Path.Combine(file.Directory.Parent.FullName, "Bearing Enhancer CAN.zip");

                            // Tải file zip về thư mục tạm
                            using (var client1 = new System.Net.WebClient())
                            {
                                client1.DownloadFile(updateUrl, tempFile);
                            }

                            // Gọi FormUpdater.exe
                            string updaterPath = Path.Combine(appDirectory, "FormUpdater.exe");
                            string mainExePath = Application.ExecutablePath;
                            string CleanPath(string path) => path.Replace("\r", "").Replace("\n", "").Trim().TrimEnd('\\');
                            string arguments = $"\"{CleanPath(tempFile)}\" \"{CleanPath(appDirectory)}\" \"{CleanPath(mainExePath)}\"";

                            var startInfo = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = updaterPath,
                                Arguments = arguments,
                                UseShellExecute = true,
                                //Verb = "runas" // yêu cầu quyền admin
                            };

                            Process.Start(startInfo);
                            Application.Exit(); // thoát app chính để cập nhật
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during processing:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DownloadAndUpdate()
        {
            try
            {
                string updateUrl = @"S:\@ICS Engineering\04. Spreadsheet & Tool\Bearing Calculator CAN\Bearing Enhancer CAN_New\Latest Version\Bearing Enhancer CAN.zip";
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var file = new FileInfo(appDirectory);
                string tempFile = Path.Combine(file.Directory.Parent.FullName, "Bearing Enhancer CAN.zip");

                // Tải file zip về thư mục tạm
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(updateUrl, tempFile);
                }

                // Gọi Updater.exe
                string updaterPath = Path.Combine(appDirectory, "Updater.exe");

                //if (!File.Exists(updaterPath))
                //{
                //    MessageBox.Show("Không tìm thấy Updater.exe tại: " + updaterPath);
                //}

                string mainExePath = Application.ExecutablePath;
                string CleanPath(string path) => path.Replace("\r", "").Replace("\n", "").Trim().TrimEnd('\\');
                string arguments = $"\"{CleanPath(tempFile)}\" \"{CleanPath(appDirectory)}\" \"{CleanPath(mainExePath)}\"";

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = updaterPath,
                    Arguments = arguments,
                    UseShellExecute = true,
                    //Verb = "runas" // yêu cầu quyền admin
                };

                //MessageBox.Show(arguments);
                //MessageBox.Show($"Updater Path: {updaterPath}\nArguments: {arguments}");
                //MessageBox.Show($"tempFile: {tempFile}\nappDirectory: {appDirectory}\nmainExePath: {mainExePath}");

                System.Diagnostics.Process.Start(startInfo);

                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)// Button Check
        {
            list_Original_Bearing?.Clear();
            dataGridView_Table.Rows.Clear();
            string rootFolder = tbx_ProjectNumberPath.Text.Trim();

            if (string.IsNullOrEmpty(rootFolder) || !Directory.Exists(rootFolder))
            {
                MessageBox.Show("Please select a valid Project folder.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string attachmentsFolder = Path.Combine(rootFolder, "Attachments");
            string unsealedFolder = Path.Combine(attachmentsFolder, "Unsealed Engineering");
            string tempFolder = Path.Combine(rootFolder, "Temp");

            if (!Directory.Exists(unsealedFolder))
            {
                MessageBox.Show("The folder 'Attachments/Unsealed Engineering' was not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            var pdfFiles = Directory.GetFiles(unsealedFolder, "*.pdf");

            if (pdfFiles.Length == 0)
            {
                MessageBox.Show("No PDF files found in 'Unsealed Engineering' folder.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string folderName = new DirectoryInfo(rootFolder).Name;
            string outputTxtFile = Path.Combine(tempFolder, folderName + ".txt");

            try
            {
                using (StreamWriter writer = new StreamWriter(outputTxtFile))
                {
                    foreach (var pdfFile in pdfFiles)
                    {
                        using (var reader = new iText.Kernel.Pdf.PdfReader(pdfFile))
                        using (var pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader))
                        {
                            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
                            {
                                var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.SimpleTextExtractionStrategy();
                                string text = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                                writer.WriteLine(text);
                            }
                        }
                    }
                }

                List<Bearing_Enhancer> list_BE = new List<Bearing_Enhancer>();
                Bearing_Enhancer BE = new Bearing_Enhancer();

                list_BE = BE.Get_Bearing_Info(outputTxtFile, comboBox_Language.Text, comboBox_Unit.Text);
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
                //List<string> list_DurationFactor = new List<string>() { "0.90", "1.00", "1.05", "1.10", "1.15", "1.25", "1.33", "1.60" };

                if (list_BE.Count == 0)
                {
                    MessageBox.Show("Not found any bearing failure. Please recheck the input data!");
                }
                else
                {
                    List<double> DurationFactors = new List<double>() { 0.90, 1.00, 1.05, 1.10, 1.15, 1.25, 1.33, 1.60 };
                    List<double> durationFactors = new List<double>();

                    PropertyInfo[] props = typeof(Duration_Factor).GetProperties();
                    string durationFactor;

                    int i = 0;
                    foreach (Bearing_Enhancer be in list_BE)
                    {
                        foreach (PropertyInfo prop in props)
                        {
                            var value = prop.GetValue(be.TopPlateInfo.DOL);
                            bool bNumber = double.TryParse(value.ToString(), out double isNumber);
                            if (bNumber)
                            {
                                durationFactors.Add(isNumber);
                            }
                        }
                        durationFactors = durationFactors.Distinct().ToList();
                        DurationFactors.AddRange(durationFactors);

                        durationFactor = durationFactors.Min().ToString("F2");
                        List<string> list_BearingSolution = be.BearingSolution;

                        (dataGridView_Table.Rows[i].Cells["DOL_Column"] as DataGridViewComboBoxCell).DataSource = durationFactors;
                        dataGridView_Table.Rows.Add(be.TrussName, be.Ply, be.LumSpecie, be.LumSize, durationFactor, be.TopPlateInfo.WetService,be.TopPlateInfo.GreenLumber,
                            be.TopPlateInfo.JointID, be.TopPlateInfo.XLocation, be.TopPlateInfo.YLocation, be.TopPlateInfo.Location_Type, be.TopPlateInfo.Reaction, be.TopPlateInfo.BearingWidth,
                            be.TopPlateInfo.RequireWidth, be.TopPlateInfo.Material, be.TopPlateInfo.LoadTransfer, false, "", list_BearingSolution[0]);
                        (dataGridView_Table.Rows[i].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = list_BearingSolution;

                        i++;
                    }
                    DurationFactors = DurationFactors.Distinct().ToList();
                    DurationFactors.Sort();
                    List<string> list_DurationFactor = DurationFactors.Select(x => x.ToString("F2")).ToList();
                    DOL_Column.DataSource = list_DurationFactor;
                }

                list_Original_Bearing = list_BE;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during processing:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void DataGridViewTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)// Sự kiện Ô thay đổi
        {
            
            if (e.ColumnIndex == 16 && e.RowIndex >= 0) // Cột Vertical_Block CheckBox
            {
                
                bool isChecked = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells["Vertical_Block"].Value);
                DataGridViewRow row = dataGridView_Table.Rows[e.RowIndex];
                string[] columnsToCheck = { "No_Ply", "DOL_Column", "Location_Type", "Reaction", "Brg_Width", "Req_Width", "Material" };
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
                        MessageBox.Show("Please check and input data into columns: No.-Ply, D-O-L, Location-Type, Reaction, Bearing-Width, Required-Width, Material");
                    }
                    else
                    {
                        using (Form_Vertical_Block_Info f2 = new Form_Vertical_Block_Info())
                        {
                            f2.Language = comboBox_Language.Text;
                            f2.Unit = comboBox_Unit.Text;
                            f2.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Size"].Value?.ToString() ?? "2x4";
                            f2.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Specie"].Value?.ToString() ?? "SPF";
                            f2.ContactLength = dataGridView_Table.Rows[e.RowIndex].Cells["Brg_Width"].Value?.ToString();

                            if (f2.ShowDialog() == DialogResult.OK)
                            {
                                Imperial_Or_Metric iom = new Imperial_Or_Metric(comboBox_Unit.Text, comboBox_Language.Text);
                                dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Size"].Value = f2.LumSize;
                                dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Specie"].Value = f2.LumSpecie;
                                dataGridView_Table.Rows[e.RowIndex].Cells["Contact_Length"].Value = iom.Unit == "Imperial" ? f2.ContactLength : Convert.ToString(Convert_String_Inch(f2.ContactLength) * iom.miliFactor);

                                Bearing_Enhancer BE = new Bearing_Enhancer();
                                BE.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells["Truss_Name"].Value?.ToString();
                                BE.Ply = dataGridView_Table.Rows[e.RowIndex].Cells["No_Ply"].Value?.ToString();
                                BE.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Specie"].Value?.ToString();
                                BE.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Size"].Value?.ToString();
                                Top_Plate_Info topPlate = new Top_Plate_Info();
                                topPlate.DOL = new Duration_Factor();
                                topPlate.DOL.DOL_Snow = dataGridView_Table.Rows[e.RowIndex].Cells["DOL_Column"].Value?.ToString();
                                topPlate.DOL.DOL_Live = "N/A";
                                topPlate.DOL.DOL_Wind = "N/A";
                                topPlate.WetService = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells["Wet_Service"].Value);
                                topPlate.GreenLumber = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells["Green_Lumber"].Value);
                                topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells["Joint_ID"].Value?.ToString();
                                topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells["X_Location"].Value?.ToString();
                                topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells["Y_Location"].Value?.ToString();
                                topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells["Location_Type"].Value?.ToString();
                                topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells["Reaction"].Value?.ToString());
                                topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells["Brg_Width"].Value?.ToString();
                                topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells["Req_Width"].Value?.ToString();
                                topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells["Material"].Value?.ToString();
                                topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells["Load_Transfer"].Value.ToString());
                                BE.TopPlateInfo = topPlate;
                                string contLength = dataGridView_Table.Rows[e.RowIndex].Cells["Contact_Length"].Value.ToString();

                                List<string> listVerBBlock = BE.Check_Bearing_Solution(BE.Ply, BE.LumSize, BE.LumSpecie, BE.TopPlateInfo, comboBox_Unit.Text, comboBox_Language.Text, true, contLength);
                                (dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = listVerBBlock;
                                dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"].Value = listVerBBlock[0];

                                row.DefaultCellStyle.BackColor = Color.AntiqueWhite; // Đổi màu dòng
                            }
                            else
                            {
                                // Nếu người dùng đóng form mà không nhập, bỏ tick
                                dataGridView_Table.Rows[e.RowIndex].Cells["Vertical_Block"].Value = false;

                            }
                        }

                    }

                }
                else // Nếu bỏ tick 
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
                        string chordSize = list_Original_Bearing?.ElementAtOrDefault(e.RowIndex)?.LumSize; //Lấy lại giá trị ban đầu nếu bỏ tick Vertical-Block
                        string chordSpecie = list_Original_Bearing?.ElementAtOrDefault(e.RowIndex)?.LumSpecie;

                        if(chordSize!=null)
                        dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Size"].Value = chordSize;

                        if(chordSpecie != null)
                        dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Specie"].Value = chordSpecie;

                        dataGridView_Table.Rows[e.RowIndex].Cells["Contact_Length"].Value = ""; //Xóa giá trị trong ô Contact Length
                        row.DefaultCellStyle.BackColor = default; //Đổi màu dòng về mặc định

                        Bearing_Enhancer BE = new Bearing_Enhancer();
                        BE.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells["Truss_Name"].Value?.ToString();
                        BE.Ply = dataGridView_Table.Rows[e.RowIndex].Cells["No_Ply"].Value?.ToString();
                        BE.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Specie"].Value?.ToString();
                        BE.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Size"].Value?.ToString();
                        Top_Plate_Info topPlate = new Top_Plate_Info();
                        topPlate.DOL = new Duration_Factor();
                        topPlate.DOL.DOL_Snow = dataGridView_Table.Rows[e.RowIndex].Cells["DOL_Column"].Value?.ToString();
                        topPlate.DOL.DOL_Live = "N/A";
                        topPlate.DOL.DOL_Wind = "N/A";
                        topPlate.WetService = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells["Wet_Service"].Value);
                        topPlate.GreenLumber = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells["Green_Lumber"].Value);
                        topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells["Joint_ID"].Value?.ToString();
                        topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells["X_Location"].Value?.ToString();
                        topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells["Y_Location"].Value?.ToString();
                        topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells["Location_Type"].Value?.ToString();
                        topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells["Reaction"].Value?.ToString());
                        topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells["Brg_Width"].Value?.ToString();
                        topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells["Req_Width"].Value?.ToString();
                        topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells["Material"].Value?.ToString();
                        topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells["Load_Transfer"].Value.ToString());
                        BE.TopPlateInfo = topPlate;

                        List<string> listHorBBlock = BE.Check_Bearing_Solution(BE.Ply, BE.LumSize, BE.LumSpecie, BE.TopPlateInfo, comboBox_Unit.Text, comboBox_Language.Text, false);
                        (dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = listHorBBlock;
                        dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"].Value = listHorBBlock[0];
                    }
                }
            }

            if (e.ColumnIndex == 19 || e.ColumnIndex == 18 || e.ColumnIndex == 16 && e.RowIndex >= 0) // Cột Checked || Bearing-Solution || Vertical-Block 
            {
                bool isChecked19 = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells["Checked"].Value);
                DataGridViewRow row = dataGridView_Table.Rows[e.RowIndex];
                var cell = dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"];
                if (isChecked19)
                {
                    if (string.IsNullOrWhiteSpace(cell.FormattedValue.ToString()))
                    {
                        MessageBox.Show("Please check and input relevant data!");
                        row.Cells["Checked"].Value = false;
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
                        else if(chosenSolution.Contains("CP"))
                        {
                            beItem = new Bearing_Enhancer_CP(chosenSolution);
                        }
                        else
                        {
                            beItem = new Bearing_Enhancer_BuildingDesigner(chosenSolution);
                        }

                        beItem.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells["Truss_Name"].Value?.ToString();
                        beItem.Ply = dataGridView_Table.Rows[e.RowIndex].Cells["No_Ply"].Value?.ToString();
                        beItem.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Specie"].Value?.ToString();
                        beItem.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Size"].Value?.ToString();
                        Top_Plate_Info topPlate = new Top_Plate_Info();
                        topPlate.DOL = new Duration_Factor();
                        topPlate.DOL.DOL_Snow = dataGridView_Table.Rows[e.RowIndex].Cells["DOL_Column"].Value?.ToString();
                        topPlate.DOL.DOL_Live = "N/A";
                        topPlate.DOL.DOL_Wind = "N/A";
                        topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells["Joint_ID"].Value?.ToString();
                        topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells["X_Location"].Value?.ToString();
                        topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells["Y_Location"].Value?.ToString();
                        topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells["Location_Type"].Value?.ToString();
                        topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells["Reaction"].Value?.ToString());
                        topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells["Brg_Width"].Value?.ToString();
                        topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells["Req_Width"].Value?.ToString();
                        topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells["Material"].Value?.ToString();
                        topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells["Load_Transfer"].Value.ToString());
                        beItem.TopPlateInfo = topPlate;
                        string contactLength = dataGridView_Table.Rows[e.RowIndex].Cells["Contact_Length"].Value?.ToString();


                        string theNote = $"Jnt {beItem.TopPlateInfo.JointID}: {beItem.Generate_Enhancer_Note(chosenSolution, comboBox_Language.Text, comboBox_Unit.Text)}";
                        row.Cells["The_Note"].Value = theNote;
                    }
                }
                else
                {
                    row.Cells["The_Note"].Value = "";
                }

            }
        }
        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)//Sự kiện rời khỏi Ô
        {
            Imperial_Or_Metric iom = new Imperial_Or_Metric(comboBox_Unit.Text, comboBox_Language.Text);
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
            // Kiểm tra cột "D-O-L"
            if (headerText.Equals("D-O-L"))
            {
                if (e.FormattedValue.ToString() == "")
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "Duration factor is not empty!";
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
                Regex regexReaction = new Regex(@"^(?!0+(\.0+)?$)\d+(\.\d+)?$");
                string input = e.FormattedValue.ToString();
                if (!regexReaction.IsMatch(input))
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = "Please input Reaction as Integer or Decimal!";
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
                Regex regexBearingWidth = iom.Unit == "Imperial" ? new Regex(@"^\d+(-\d+){1,2}$") : new Regex(@"^(?!0+(\.0+)?$)\d+(\.\d+)?$");
                string input = e.FormattedValue.ToString();
                if (!regexBearingWidth.IsMatch(input))
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = iom.Unit == "Imperial" ? "Please input Bearing-Width as the format: 3-08, 5-08, 1-00-02!" : "Please input Bearing-Width as Integer or Decimal!";
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
                Regex regexRequiredWidth = iom.Unit == "Imperial" ? new Regex(@"^\d+(-\d+){1,2}$") : new Regex(@"^(?!0+(\.0+)?$)\d+(\.\d+)?$");
                string input = e.FormattedValue.ToString();
                if (!regexRequiredWidth.IsMatch(input))
                {
                    // Hiển thị lỗi tại hàng
                    dataGridView_Table.Rows[e.RowIndex].ErrorText = iom.Unit == "Imperial" ? "Please input Required-Width as the format: 3-08, 5-08, 1-00-00!" : "Please input Required-Width as Integer or Decimal!";
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

            if ((e.ColumnIndex == 11 || e.ColumnIndex == 12 || e.ColumnIndex == 13) && e.RowIndex >= 0)// Cột Reaction||Bearing-Width||Required-Width
            {
                string inputR = dataGridView_Table.Rows[e.RowIndex].Cells["Reaction"].Value?.ToString();
                string inputBW = dataGridView_Table.Rows[e.RowIndex].Cells["Brg_Width"].Value?.ToString();
                string inputRW = dataGridView_Table.Rows[e.RowIndex].Cells["Req_Width"].Value?.ToString();

                // Kiểm tra null / rỗng trước
                if (!string.IsNullOrWhiteSpace(inputR) && !string.IsNullOrWhiteSpace(inputBW) && !string.IsNullOrWhiteSpace(inputRW))
                {
                    Regex regexBearing = iom.Unit == "Imperial" ? new Regex(@"^\d+(-\d+){1,2}$") : new Regex(@"^(?!0+(\.0+)?$)\d+(\.\d+)?$");//-------------------------------------------------------------------------------
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
                                dataGridView_Table.Rows[e.RowIndex].Cells["Load_Transfer"].Value = loadTransfer;
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

            int[] columnsNumber = { 1, 2, 3, 4, 5, 6, 10, 11, 12, 13, 14 };
            bool isColumn = columnsNumber.Any(c => c == e.ColumnIndex);
            if (isColumn && e.RowIndex >= 0) // Cột No.-Ply, Lumber-Spiecie, Lumber-Size, D-O-L, Wet-Servive, Green-Lumber, Location-Type, Reaction, Bearing-Width, Required-Width, Material
            {
                bool checkNullBS = false;
                string cellValueBS;
                foreach (int colName in columnsNumber.Where(x=>x!=5 && x!=6))
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
                    string inputR = dataGridView_Table.Rows[e.RowIndex].Cells["Reaction"].Value?.ToString();
                    string inputBW = dataGridView_Table.Rows[e.RowIndex].Cells["Brg_Width"].Value?.ToString();
                    string inputRW = dataGridView_Table.Rows[e.RowIndex].Cells["Req_Width"].Value?.ToString();

                    // Kiểm tra null / rỗng trước
                    if (!string.IsNullOrWhiteSpace(inputR) && !string.IsNullOrWhiteSpace(inputBW) && !string.IsNullOrWhiteSpace(inputRW))
                    {
                        Regex regexBearing = iom.Unit == "Imperial" ? new Regex(@"^\d+(-\d+){1,2}$") : new Regex(@"^(?!0+(\.0+)?$)\d+(\.\d+)?$");//--------------------------------------------------------------------------
                        Regex regexInteger = new Regex(@"^\d+$");

                        // Chỉ xử lý nếu Reaction là số nguyên, và BW & RW đúng định dạng
                        if (regexInteger.IsMatch(inputR) && regexBearing.IsMatch(inputBW) && regexBearing.IsMatch(inputRW))
                        {
                            Bearing_Enhancer BE = new Bearing_Enhancer();
                            BE.TrussName = dataGridView_Table.Rows[e.RowIndex].Cells["Truss_Name"].Value?.ToString();
                            BE.Ply = dataGridView_Table.Rows[e.RowIndex].Cells["No_Ply"].Value?.ToString();
                            BE.LumSpecie = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Specie"].Value?.ToString();
                            BE.LumSize = dataGridView_Table.Rows[e.RowIndex].Cells["Lumber_Size"].Value?.ToString();
                            Top_Plate_Info topPlate = new Top_Plate_Info();
                            topPlate.DOL = new Duration_Factor();
                            topPlate.DOL.DOL_Snow = dataGridView_Table.Rows[e.RowIndex].Cells["DOL_Column"].Value?.ToString();
                            topPlate.DOL.DOL_Live = "N/A";
                            topPlate.DOL.DOL_Wind = "N/A";
                            topPlate.WetService = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells["Wet_Service"].Value);
                            topPlate.GreenLumber = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells["Green_Lumber"].Value);
                            topPlate.JointID = dataGridView_Table.Rows[e.RowIndex].Cells["Joint_ID"].Value?.ToString();
                            topPlate.XLocation = dataGridView_Table.Rows[e.RowIndex].Cells["X_Location"].Value?.ToString();
                            topPlate.YLocation = dataGridView_Table.Rows[e.RowIndex].Cells["Y_Location"].Value?.ToString();
                            topPlate.Location_Type = dataGridView_Table.Rows[e.RowIndex].Cells["Location_Type"].Value?.ToString();
                            topPlate.Reaction = double.Parse(dataGridView_Table.Rows[e.RowIndex].Cells["Reaction"].Value?.ToString());
                            topPlate.BearingWidth = dataGridView_Table.Rows[e.RowIndex].Cells["Brg_Width"].Value?.ToString();
                            topPlate.RequireWidth = dataGridView_Table.Rows[e.RowIndex].Cells["Req_Width"].Value?.ToString();
                            topPlate.Material = dataGridView_Table.Rows[e.RowIndex].Cells["Material"].Value?.ToString();
                            topPlate.LoadTransfer = Convert.ToDouble(dataGridView_Table.Rows[e.RowIndex].Cells["Load_Transfer"].Value.ToString());
                            BE.TopPlateInfo = topPlate;
                            string contLength = dataGridView_Table.Rows[e.RowIndex].Cells["Contact_Length"].Value?.ToString();

                            bool bVerBlock = Convert.ToBoolean(dataGridView_Table.Rows[e.RowIndex].Cells["Vertical_Block"].Value?.ToString());
                            List<string> listBBlock = BE.Check_Bearing_Solution(BE.Ply, BE.LumSize, BE.LumSpecie, BE.TopPlateInfo, comboBox_Unit.Text, comboBox_Language.Text, bVerBlock, contLength);
                            (dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"] as DataGridViewComboBoxCell).DataSource = listBBlock;
                            dataGridView_Table.Rows[e.RowIndex].Cells["Bearing_Solution"].Value = listBBlock[0];
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
            if (e.ColumnIndex == 19) // Cột Checked
            {
                bool currentValue = true;
                foreach (DataGridViewRow row in dataGridView_Table.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        row.Cells[19].Value = currentValue;
                    }
                }
            }
        }

        private void dataGridViewTable_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Clicks == 2)//Double RightClick
            {
                var hit = dataGridView_Table.HitTest(e.X, e.Y);
                int rowIndex = hit.RowIndex;
                int colIndex = hit.ColumnIndex;
                bool currentValue = false;
                if(colIndex == 19) // Cột Checked
                {
                    foreach (DataGridViewRow row in dataGridView_Table.Rows)
                    {
                        row.Cells[19].Value = currentValue;
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
            string compReviewPath = Path.Combine(tbx_ProjectNumberPath.Text, "Attachments", "CompReview");
            string projectPath = tbx_ProjectNumberPath.Text;
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = "Bearing Enhancer Report.xlsx",
                InitialDirectory = ((Directory.Exists(compReviewPath)) ? compReviewPath : projectPath)
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
                        Excel.Range headerRange = worksheet.get_Range("A1", "U1");
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
                bool valueCol19 = Convert.ToBoolean(row.Cells[19].Value);
                if (!row.IsNewRow && valueCol19)
                {
                    (string TrussName, string JointID, string Note) theNoteItem = (row.Cells[0].Value?.ToString(), row.Cells[7].Value?.ToString(), row.Cells[20].Value?.ToString());
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
                            bool valueCol19 = Convert.ToBoolean(row.Cells[19].Value);
                            if (!row.IsNewRow && valueCol19)
                            {
                                (string TrussName, string Note) theNoteItem = (row.Cells[0].Value?.ToString(), row.Cells[20].Value?.ToString());
                                if (!string.IsNullOrEmpty(theNoteItem.TrussName) && !string.IsNullOrEmpty(theNoteItem.Note))
                                {
                                    // Gọi hàm thêm vào XML
                                    Add_Note_ToTruss(tbx_ProjectNumberPath.Text, theNoteItem);
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
            string projectPath = tbx_ProjectNumberPath.Text;
            string trussesPath = $"{tbx_ProjectNumberPath.Text}\\Trusses";
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

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            string defaultPath = @"C:\SST-EA\Client\Projects";

            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the Project folder (e.g., 255159)";

                // Kiểm tra nếu đường dẫn mặc định tồn tại, sẽ set là đường dẫn mặc định
                if (Directory.Exists(defaultPath))
                {
                    folderDialog.SelectedPath = defaultPath;
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    tbx_ProjectNumberPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form_BearingEnhacerCAN_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if there is any data (excluding the new row)
            bool hasData = dataGridView_Table.Rows
                .Cast<DataGridViewRow>()
                .Any(row => !row.IsNewRow && row.Cells.Cast<DataGridViewCell>().Any(cell => cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString())));

            if (hasData)
            {
                var result = MessageBox.Show(
                    "Data exists in the table. Do you want to export before closing?",
                    "Confirm Export",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ExportToExcel(dataGridView_Table);
                    // Allow closing after export
                }
                else if (result == DialogResult.No)
                {
                    // Allow closing without export
                }
                else // Cancel
                {
                    e.Cancel = true;
                }
            }
        }

        

        private void dataGridView_Table_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int columnIndex = dataGridView_Table.Columns["Bearing_Solution"].Index;

            if (dataGridView_Table.CurrentCell.ColumnIndex == columnIndex && e.Control is ComboBox comboBox)
            {
                if (currentComboBox != null)
                {
                    currentComboBox.SelectionChangeCommitted -= ComboBox_SelectionChangeCommitted;
                }

                currentComboBox = comboBox;
                currentComboBox.SelectionChangeCommitted += ComboBox_SelectionChangeCommitted;
            }
        }

        private void ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                string selectedValue = comboBox.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(selectedValue) && selectedValue.Contains("FlushPlate"))
                {
                    var currentRow = dataGridView_Table.CurrentRow;
                    string trussName = currentRow.Cells["Truss_Name"].Value?.ToString();
                    string jointID = currentRow.Cells["Joint_ID"].Value?.ToString();
                    currentRow.DefaultCellStyle.BackColor = Color.Silver;

                    Point cursorPos = Cursor.Position;
                    //Point relativePos = this.PointToClient(cursorPos);

                    warningToolTip.Show($"Truss {trussName}-Jnt {jointID}: Flush Plate needs to be considered in Truss Studio!",
                                        this,
                                        cursorPos.X-500,
                                        cursorPos.Y-50,
                                        3000); // Show in 3 seconds
                    //MessageBox.Show(comboBox.SelectedItem?.ToString());
                }
                else
                {
                    var currentRow = dataGridView_Table.CurrentRow;
                    bool bVerticalBlock = Convert.ToBoolean(currentRow.Cells["Vertical_Block"].Value?.ToString());

                    if (!bVerticalBlock)
                    currentRow.DefaultCellStyle.BackColor = dataGridView_Table.DefaultCellStyle.BackColor;

                    warningToolTip.Hide(dataGridView_Table); // Hide if not Flush Plate

                }
            }
        }
    }

}
