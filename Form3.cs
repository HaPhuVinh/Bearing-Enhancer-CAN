using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bearing_Enhancer_CAN
{
    public partial class Form_CAD_Markup : Form
    {
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
        }

        private void Form3_Load(object sender, EventArgs e)
        {

            List<string> list_LocationType = new List<string> { "Interior", "Exterior" };
            List<string> list_YLocation = new List<string> { "BotChd", "TopChd", "Web", "" };

            Bearing_Type.DataSource = list_LocationType;
            Y_Location.DataSource = list_YLocation;

            if (listBearingEnhancers != null)
            {
                dataGridView_CADMarkup.Rows.Clear();
                for (int i = 0; i < listBearingEnhancers.Count; i++)
                {
                    dataGridView_CADMarkup.Rows.Add();
                    dataGridView_CADMarkup.Rows[i].Cells["Truss_Name"].Value = listBearingEnhancers[i].TrussName;
                    dataGridView_CADMarkup.Rows[i].Cells["Joint_ID"].Value = listBearingEnhancers[i].TopPlateInfo.JointID;
                    dataGridView_CADMarkup.Rows[i].Cells["Bearing_Type"].Value = listBearingEnhancers[i].TopPlateInfo.Location_Type;
                    dataGridView_CADMarkup.Rows[i].Cells["Y_Location"].Value = listBearingEnhancers[i].TopPlateInfo.YLocation;
                    dataGridView_CADMarkup.Rows[i].Cells["X_Location"].Value = listBearingEnhancers[i].TopPlateInfo.XLocation;
                    dataGridView_CADMarkup.Rows[i].Cells["Chosen_Solution"].Value = listBearingEnhancers[i].Chosen_Solution;
                }
            }
        }
        private void DataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0 && e.ColumnIndex == 6)
            {
                DataGridViewRow row = dataGridView_CADMarkup.Rows[e.RowIndex];
                bool isChecked = Convert.ToBoolean(row.Cells["Get_Script_Note"].Value);
                if(isChecked)
                {
                    row.Cells["The_Script_Note"].Value = "The_Script_Note";
                }
                else
                {
                    row.Cells["The_Script_Note"].Value = "";
                }
            }
        }
    }
}
