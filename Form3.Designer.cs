namespace Bearing_Enhancer_CAN
{
    partial class Form_CAD_Markup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TrussName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JointID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BearingType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.YLocation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.XLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScriptNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TrussName,
            this.JointID,
            this.BearingType,
            this.YLocation,
            this.XLocation,
            this.ScriptNote});
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1043, 616);
            this.dataGridView1.TabIndex = 0;
            // 
            // TrussName
            // 
            this.TrussName.HeaderText = "Truss-Name";
            this.TrussName.Name = "TrussName";
            // 
            // JointID
            // 
            this.JointID.HeaderText = "Joint-ID";
            this.JointID.Name = "JointID";
            // 
            // BearingType
            // 
            this.BearingType.HeaderText = "Bearing-Type";
            this.BearingType.Name = "BearingType";
            // 
            // YLocation
            // 
            this.YLocation.HeaderText = "Y-Location";
            this.YLocation.Name = "YLocation";
            // 
            // XLocation
            // 
            this.XLocation.HeaderText = "X-Location";
            this.XLocation.Name = "XLocation";
            // 
            // ScriptNote
            // 
            this.ScriptNote.HeaderText = "Script-Note";
            this.ScriptNote.Name = "ScriptNote";
            this.ScriptNote.Width = 500;
            // 
            // Form_CAD_Markup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 640);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form_CAD_Markup";
            this.Text = "CAD Markup";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrussName;
        private System.Windows.Forms.DataGridViewTextBoxColumn JointID;
        private System.Windows.Forms.DataGridViewComboBoxColumn BearingType;
        private System.Windows.Forms.DataGridViewComboBoxColumn YLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn XLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScriptNote;
    }
}