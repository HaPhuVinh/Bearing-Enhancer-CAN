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
            this.dataGridView_CADMarkup = new System.Windows.Forms.DataGridView();
            this.AddTDL = new System.Windows.Forms.Button();
            this.Truss_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Joint_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bearing_Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Y_Location = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.X_Location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Chosen_Solution = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Get_Script_Note = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.The_Script_Note = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CADMarkup)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_CADMarkup
            // 
            this.dataGridView_CADMarkup.BackgroundColor = System.Drawing.Color.DarkSeaGreen;
            this.dataGridView_CADMarkup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_CADMarkup.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Truss_Name,
            this.Joint_ID,
            this.Bearing_Type,
            this.Y_Location,
            this.X_Location,
            this.Chosen_Solution,
            this.Get_Script_Note,
            this.The_Script_Note});
            this.dataGridView_CADMarkup.Location = new System.Drawing.Point(3, 23);
            this.dataGridView_CADMarkup.Name = "dataGridView_CADMarkup";
            this.dataGridView_CADMarkup.Size = new System.Drawing.Size(1243, 616);
            this.dataGridView_CADMarkup.TabIndex = 0;
            // 
            // AddTDL
            // 
            this.AddTDL.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddTDL.Location = new System.Drawing.Point(1267, 23);
            this.AddTDL.Name = "AddTDL";
            this.AddTDL.Size = new System.Drawing.Size(133, 47);
            this.AddTDL.TabIndex = 1;
            this.AddTDL.Text = "Add to TDL";
            this.AddTDL.UseVisualStyleBackColor = true;
            // 
            // Truss_Name
            // 
            this.Truss_Name.HeaderText = "Truss-Name";
            this.Truss_Name.Name = "Truss_Name";
            this.Truss_Name.ReadOnly = true;
            // 
            // Joint_ID
            // 
            this.Joint_ID.HeaderText = "Joint-ID";
            this.Joint_ID.Name = "Joint_ID";
            this.Joint_ID.ReadOnly = true;
            // 
            // Bearing_Type
            // 
            this.Bearing_Type.HeaderText = "Bearing-Type";
            this.Bearing_Type.Name = "Bearing_Type";
            this.Bearing_Type.ReadOnly = true;
            // 
            // Y_Location
            // 
            this.Y_Location.HeaderText = "Y-Location";
            this.Y_Location.Name = "Y_Location";
            // 
            // X_Location
            // 
            this.X_Location.HeaderText = "X-Location";
            this.X_Location.Name = "X_Location";
            this.X_Location.ReadOnly = true;
            // 
            // Chosen_Solution
            // 
            this.Chosen_Solution.HeaderText = "Chosen-Solution";
            this.Chosen_Solution.Name = "Chosen_Solution";
            this.Chosen_Solution.ReadOnly = true;
            this.Chosen_Solution.Width = 200;
            // 
            // Get_Script_Note
            // 
            this.Get_Script_Note.HeaderText = "Get-Script-Note";
            this.Get_Script_Note.Name = "Get_Script_Note";
            // 
            // The_Script_Note
            // 
            this.The_Script_Note.HeaderText = "The-Script-Note";
            this.The_Script_Note.Name = "The_Script_Note";
            this.The_Script_Note.Width = 500;
            // 
            // Form_CAD_Markup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1412, 640);
            this.Controls.Add(this.AddTDL);
            this.Controls.Add(this.dataGridView_CADMarkup);
            this.ForeColor = System.Drawing.Color.Green;
            this.Name = "Form_CAD_Markup";
            this.Text = "CAD Markup";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CADMarkup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_CADMarkup;
        private System.Windows.Forms.Button AddTDL;
        private System.Windows.Forms.DataGridViewTextBoxColumn Truss_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Joint_ID;
        private System.Windows.Forms.DataGridViewComboBoxColumn Bearing_Type;
        private System.Windows.Forms.DataGridViewComboBoxColumn Y_Location;
        private System.Windows.Forms.DataGridViewTextBoxColumn X_Location;
        private System.Windows.Forms.DataGridViewTextBoxColumn Chosen_Solution;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Get_Script_Note;
        private System.Windows.Forms.DataGridViewTextBoxColumn The_Script_Note;
    }
}