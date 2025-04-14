using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bearing_Enhancer_CAN
{
    public partial class Form_Vertical_Block_Info: Form
    {
        public string Language { get; set; }
        public string Unit { get; set; }
        public string LumSize { get; set; }
        public string LumSpecie { get; set; }
        public string ContactLength { get; set; }
        public List<string> BlockSolution { get; set; }

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
            cbx_Lumber_Size.Text = LumSize;
            cbx_Lumber_Specie.DataSource = list_Specie;
            cbx_Lumber_Specie.Text = LumSpecie;
            txt_Contact_Length.Text = ContactLength;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cbx_VerBlock_Solution_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            string input = txt_Contact_Length.Text.Trim();
            Regex regex = new Regex(Unit == "Imperial" ? @"^\d+(-\d+){1,2}$" : @"^\d+$");
            if (string.IsNullOrWhiteSpace(txt_Contact_Length.Text) || !regex.IsMatch(input))
            {
                errorProvider1.SetError(txt_Contact_Length, "Please enter a valid contact length! Example: 0-12, 3-08, 1-00-00");
                isValid = false;
            }
            else
            {
                errorProvider1.SetError(txt_Contact_Length, "");
            }
            if (isValid)
            {
                LumSize = cbx_Lumber_Size.Text;
                LumSpecie = cbx_Lumber_Specie.Text;
                ContactLength = txt_Contact_Length.Text;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
