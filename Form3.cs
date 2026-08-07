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
    public partial class Form_CAD_Markup : Form
    {
        public string ProjectPath { get; set; }
        public List<Bearing_Enhancer> listBearingEnhancers = new List<Bearing_Enhancer>();
        public Form_CAD_Markup()
        {
            InitializeComponent();
            // Đăng ký sự kiện thay đổi trạng thái ô
            dataGridView_CADMarkup.CellValueChanged += DataGridViewCellValueChanged;
            dataGridView_CADMarkup.CurrentCellDirtyStateChanged += (s, ev) =>
            {
                if (dataGridView_CADMarkup.IsCurrentCellDirty)
                    dataGridView_CADMarkup.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            //Đăng kí sự kiện Double_Click
            dataGridView_CADMarkup.CellDoubleClick += dataGridViewCADMarkup_CellDoubleClick;

            //Đăng kí sự kiện Double_RightClick

            dataGridView_CADMarkup.MouseDown += dataGridViewCADMarkup_MouseDown;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            dataGridView_CADMarkup.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView_CADMarkup.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //List<string> list_LocationType = new List<string> { "Interior", "Exterior" };
            //List<string> list_YLocation = new List<string> { "BotChd", "TopChd", "Web", "" };

            //Bearing_Type.DataSource = list_LocationType;
            //Y_Location.DataSource = list_YLocation;

            if (listBearingEnhancers != null)
            {
                dataGridView_CADMarkup.Rows.Clear();
                dataGridView_CADMarkup.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                for (int i = 0; i < listBearingEnhancers.Count; i++)
                {
                    dataGridView_CADMarkup.Rows.Add();
                    dataGridView_CADMarkup.Rows[i].Cells["Truss_Name"].Value = listBearingEnhancers[i].TrussName;
                    dataGridView_CADMarkup.Rows[i].Cells["Joint_ID"].Value = listBearingEnhancers[i].TopPlateInfo.JointID;
                    dataGridView_CADMarkup.Rows[i].Cells["Bearing_Type"].Value = listBearingEnhancers[i].TopPlateInfo.Location_Type;
                    dataGridView_CADMarkup.Rows[i].Cells["Y_Location"].Value = listBearingEnhancers[i].TopPlateInfo.YLocation;
                    dataGridView_CADMarkup.Rows[i].Cells["X_Location"].Value = listBearingEnhancers[i].TopPlateInfo.XLocation;
                    dataGridView_CADMarkup.Rows[i].Cells["Chosen_Solution"].Value = listBearingEnhancers[i].Chosen_Solution;
                    dataGridView_CADMarkup.Rows[i].Cells["The_Script_Note"].Value = listBearingEnhancers[i].BBlock_Markup_Script;
                }
                dataGridView_CADMarkup.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                dataGridView_CADMarkup.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            }
        }
        private void DataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void AddTDL_Click(object sender, EventArgs e)
        {
            List<(string TrussName, string JointID, string Note)> listItem = new List<(string TrussName, string JointID, string Note)>();
            foreach (DataGridViewRow row in dataGridView_CADMarkup.Rows)
            {
                bool valueCol6 = Convert.ToBoolean(row.Cells[6].Value);
                if (!row.IsNewRow && valueCol6)
                {
                    (string TrussName, string JointID, string Note) theNoteItem = (row.Cells[0].Value?.ToString(), row.Cells[1].Value?.ToString(), row.Cells[7].Value?.ToString());
                    if (!string.IsNullOrEmpty(theNoteItem.TrussName) && !string.IsNullOrEmpty(theNoteItem.Note))
                    {
                        listItem.Add(theNoteItem);
                    }
                }
            }
            if (listItem.Count != 0)
            {
                string mssg = "Please confirm! The below trusses will be added:\n";
                foreach (var item in listItem)
                {
                    mssg += $"Truss: {item.TrussName} - Joint: {item.JointID},\n";
                }
                DialogResult result = MessageBox.Show(mssg, "Notification", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        foreach (DataGridViewRow row in dataGridView_CADMarkup.Rows)
                        {
                            bool valueCol6 = Convert.ToBoolean(row.Cells[6].Value);
                            if (!row.IsNewRow && valueCol6)
                            {
                                (string TrussName, string Note) theNoteItem = (row.Cells[0].Value?.ToString(), row.Cells[7].Value?.ToString());
                                if (!string.IsNullOrEmpty(theNoteItem.TrussName) && !string.IsNullOrEmpty(theNoteItem.Note))
                                {
                                    // Gọi hàm thêm vào XML
                                    Add_Note_ToTruss(ProjectPath, theNoteItem);
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
            string projectPath = path;
            string trussesPath = $"{path}\\Trusses";
            string[] arrPath = projectPath.Split('\\');
            //string projectID = arrPath[arrPath.Length - 1];
            string xmlFilePath = Path.Combine(trussesPath, $"{item.TrussName}.tdlTruss");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            XmlNode rootNode, noteNode;
            rootNode = xmlDoc.DocumentElement;
            noteNode = rootNode.SelectSingleNode("//Script");
            noteNode.InnerText += "\n\n";
            noteNode.InnerText += item.Note;

            xmlDoc.Save(xmlFilePath);
        }
        void dataGridViewCADMarkup_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6)
            {
                bool currentValue = true;
                foreach (DataGridViewRow row in dataGridView_CADMarkup.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        row.Cells[6].Value = currentValue;
                    }
                }
            }
        }
        private void dataGridViewCADMarkup_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Clicks == 2)//Double RightClick
            {
                var hit = dataGridView_CADMarkup.HitTest(e.X, e.Y);
                int rowIndex = hit.RowIndex;
                int colIndex = hit.ColumnIndex;
                bool currentValue = false;
                if (colIndex == 6) 
                {
                    foreach (DataGridViewRow row in dataGridView_CADMarkup.Rows)
                    {
                        row.Cells[6].Value = currentValue;
                    }
                }
            }
        }
    }
}
