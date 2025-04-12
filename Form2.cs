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
    public partial class Form_Vertical_Block_Info: Form
    {
        public Form_Vertical_Block_Info()
        {
            InitializeComponent();
            this.Load += Form_Vertical_Block_Info_Load;
        }
        private void Form_Vertical_Block_Info_Load(object sender, EventArgs e)
        {
            // Load the form and set default values
            List<string> list_LumSize = new List<string> { "2x4", "2x6", "2x8", "2x10", "2x12" };
            List<string> list_Specie = new List<string> { "SPF", "DFL", "DFLN", "SP", "SYP", "HF" };
            cbx_Lumber_Size.DataSource = list_LumSize;
            cbx_Lumber_Specie.DataSource = list_Specie;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
