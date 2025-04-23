namespace Bearing_Enhancer_CAN
{
    partial class Form_BearingEnhacerCAN
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BearingEnhacerCAN));
            this.button_Check = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_PJNum = new System.Windows.Forms.TextBox();
            this.dataGridView_Table = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_Language = new System.Windows.Forms.ComboBox();
            this.comboBox_Unit = new System.Windows.Forms.ComboBox();
            this.label_Language = new System.Windows.Forms.Label();
            this.label_Unit = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.btn_export_data = new System.Windows.Forms.Button();
            this.bearingEnhancerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Truss_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.No_Ply = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Lumber_Specie = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Lumber_Size = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DOL_Column = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Joint_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.X_Location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y_Location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Location_Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Reaction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Brg_Width = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Req_Width = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Material = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Load_Transfer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bearing_Solution = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Vertical_Block = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Contact_Length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.The_Note = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Table)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bearingEnhancerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Check
            // 
            this.button_Check.BackColor = System.Drawing.Color.White;
            this.button_Check.Font = new System.Drawing.Font("Lucida Console", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Check.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button_Check.Location = new System.Drawing.Point(463, 52);
            this.button_Check.Margin = new System.Windows.Forms.Padding(2);
            this.button_Check.Name = "button_Check";
            this.button_Check.Size = new System.Drawing.Size(126, 49);
            this.button_Check.TabIndex = 0;
            this.button_Check.Text = "CHECK";
            this.button_Check.UseVisualStyleBackColor = false;
            this.button_Check.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Black", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Project Number path";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBox_PJNum
            // 
            this.textBox_PJNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_PJNum.Location = new System.Drawing.Point(207, 15);
            this.textBox_PJNum.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_PJNum.Name = "textBox_PJNum";
            this.textBox_PJNum.Size = new System.Drawing.Size(382, 24);
            this.textBox_PJNum.TabIndex = 2;
            this.textBox_PJNum.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // dataGridView_Table
            // 
            this.dataGridView_Table.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.dataGridView_Table.BackgroundColor = System.Drawing.Color.Linen;
            this.dataGridView_Table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Table.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Truss_Name,
            this.No_Ply,
            this.Lumber_Specie,
            this.Lumber_Size,
            this.DOL_Column,
            this.Joint_ID,
            this.X_Location,
            this.Y_Location,
            this.Location_Type,
            this.Reaction,
            this.Brg_Width,
            this.Req_Width,
            this.Material,
            this.Load_Transfer,
            this.Bearing_Solution,
            this.Vertical_Block,
            this.Contact_Length,
            this.Checked,
            this.The_Note});
            this.dataGridView_Table.Location = new System.Drawing.Point(2, 127);
            this.dataGridView_Table.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView_Table.Name = "dataGridView_Table";
            this.dataGridView_Table.RowHeadersWidth = 51;
            this.dataGridView_Table.RowTemplate.Height = 24;
            this.dataGridView_Table.Size = new System.Drawing.Size(1798, 612);
            this.dataGridView_Table.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.label2.Location = new System.Drawing.Point(7, 109);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Data Table";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // comboBox_Language
            // 
            this.comboBox_Language.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_Language.FormattingEnabled = true;
            this.comboBox_Language.Location = new System.Drawing.Point(207, 52);
            this.comboBox_Language.Name = "comboBox_Language";
            this.comboBox_Language.Size = new System.Drawing.Size(139, 23);
            this.comboBox_Language.TabIndex = 5;
            // 
            // comboBox_Unit
            // 
            this.comboBox_Unit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_Unit.FormattingEnabled = true;
            this.comboBox_Unit.Location = new System.Drawing.Point(207, 86);
            this.comboBox_Unit.Name = "comboBox_Unit";
            this.comboBox_Unit.Size = new System.Drawing.Size(139, 23);
            this.comboBox_Unit.TabIndex = 5;
            // 
            // label_Language
            // 
            this.label_Language.AutoSize = true;
            this.label_Language.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Language.Location = new System.Drawing.Point(125, 53);
            this.label_Language.Name = "label_Language";
            this.label_Language.Size = new System.Drawing.Size(72, 17);
            this.label_Language.TabIndex = 6;
            this.label_Language.Text = "Language";
            // 
            // label_Unit
            // 
            this.label_Unit.AutoSize = true;
            this.label_Unit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Unit.Location = new System.Drawing.Point(125, 85);
            this.label_Unit.Name = "label_Unit";
            this.label_Unit.Size = new System.Drawing.Size(33, 17);
            this.label_Unit.TabIndex = 6;
            this.label_Unit.Text = "Unit";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // btn_export_data
            // 
            this.btn_export_data.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_export_data.ForeColor = System.Drawing.Color.Black;
            this.btn_export_data.Location = new System.Drawing.Point(625, 53);
            this.btn_export_data.Name = "btn_export_data";
            this.btn_export_data.Size = new System.Drawing.Size(107, 48);
            this.btn_export_data.TabIndex = 7;
            this.btn_export_data.Text = "Export Data";
            this.btn_export_data.UseVisualStyleBackColor = true;
            this.btn_export_data.Click += new System.EventHandler(this.btn_export_data_Click);
            // 
            // bearingEnhancerBindingSource
            // 
            this.bearingEnhancerBindingSource.DataSource = typeof(Bearing_Enhancer_CAN.Bearing_Enhancer);
            // 
            // Truss_Name
            // 
            this.Truss_Name.DataPropertyName = "TrussName";
            this.Truss_Name.HeaderText = "Truss-Name";
            this.Truss_Name.MinimumWidth = 6;
            this.Truss_Name.Name = "Truss_Name";
            // 
            // No_Ply
            // 
            this.No_Ply.DataPropertyName = "Ply";
            this.No_Ply.HeaderText = "No.-Ply";
            this.No_Ply.MinimumWidth = 6;
            this.No_Ply.Name = "No_Ply";
            this.No_Ply.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.No_Ply.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.No_Ply.Width = 66;
            // 
            // Lumber_Specie
            // 
            this.Lumber_Specie.DataPropertyName = "LumSpecie";
            this.Lumber_Specie.HeaderText = "Lumber-Specie";
            this.Lumber_Specie.MinimumWidth = 6;
            this.Lumber_Specie.Name = "Lumber_Specie";
            this.Lumber_Specie.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Lumber_Specie.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Lumber_Specie.Width = 103;
            // 
            // Lumber_Size
            // 
            this.Lumber_Size.DataPropertyName = "LumSize";
            this.Lumber_Size.HeaderText = "Lumber-Size";
            this.Lumber_Size.MinimumWidth = 6;
            this.Lumber_Size.Name = "Lumber_Size";
            this.Lumber_Size.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Lumber_Size.Width = 71;
            // 
            // DOL_Column
            // 
            this.DOL_Column.DataPropertyName = "DOL";
            this.DOL_Column.HeaderText = "D-O-L";
            this.DOL_Column.MinimumWidth = 6;
            this.DOL_Column.Name = "DOL_Column";
            this.DOL_Column.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DOL_Column.Width = 50;
            // 
            // Joint_ID
            // 
            this.Joint_ID.DataPropertyName = "JointID";
            this.Joint_ID.HeaderText = "Joint-ID";
            this.Joint_ID.MinimumWidth = 6;
            this.Joint_ID.Name = "Joint_ID";
            this.Joint_ID.Width = 68;
            // 
            // X_Location
            // 
            this.X_Location.HeaderText = "X-Location";
            this.X_Location.MinimumWidth = 6;
            this.X_Location.Name = "X_Location";
            this.X_Location.Width = 83;
            // 
            // Y_Location
            // 
            this.Y_Location.HeaderText = "Y-Location";
            this.Y_Location.MinimumWidth = 6;
            this.Y_Location.Name = "Y_Location";
            this.Y_Location.Width = 83;
            // 
            // Location_Type
            // 
            this.Location_Type.HeaderText = "Location-Type";
            this.Location_Type.MinimumWidth = 6;
            this.Location_Type.Name = "Location_Type";
            this.Location_Type.Width = 125;
            // 
            // Reaction
            // 
            this.Reaction.HeaderText = "Reaction";
            this.Reaction.MinimumWidth = 6;
            this.Reaction.Name = "Reaction";
            this.Reaction.Width = 75;
            // 
            // Brg_Width
            // 
            this.Brg_Width.HeaderText = "Bearing-Width";
            this.Brg_Width.MinimumWidth = 6;
            this.Brg_Width.Name = "Brg_Width";
            this.Brg_Width.Width = 99;
            // 
            // Req_Width
            // 
            this.Req_Width.HeaderText = "Required-Width";
            this.Req_Width.MinimumWidth = 6;
            this.Req_Width.Name = "Req_Width";
            this.Req_Width.Width = 106;
            // 
            // Material
            // 
            this.Material.HeaderText = "Material";
            this.Material.MinimumWidth = 6;
            this.Material.Name = "Material";
            this.Material.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Material.Width = 50;
            // 
            // Load_Transfer
            // 
            this.Load_Transfer.HeaderText = "Load-Transfer";
            this.Load_Transfer.MinimumWidth = 6;
            this.Load_Transfer.Name = "Load_Transfer";
            this.Load_Transfer.ReadOnly = true;
            this.Load_Transfer.Width = 98;
            // 
            // Bearing_Solution
            // 
            this.Bearing_Solution.HeaderText = "Bearing-Solution";
            this.Bearing_Solution.MinimumWidth = 6;
            this.Bearing_Solution.Name = "Bearing_Solution";
            this.Bearing_Solution.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Bearing_Solution.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Bearing_Solution.Width = 270;
            // 
            // Vertical_Block
            // 
            this.Vertical_Block.FalseValue = "False";
            this.Vertical_Block.HeaderText = "Vertical-Block?";
            this.Vertical_Block.MinimumWidth = 6;
            this.Vertical_Block.Name = "Vertical_Block";
            this.Vertical_Block.TrueValue = "True";
            this.Vertical_Block.Width = 84;
            // 
            // Contact_Length
            // 
            this.Contact_Length.HeaderText = "Contact-Length";
            this.Contact_Length.MinimumWidth = 6;
            this.Contact_Length.Name = "Contact_Length";
            this.Contact_Length.ReadOnly = true;
            this.Contact_Length.Width = 105;
            // 
            // Checked
            // 
            this.Checked.FalseValue = "False";
            this.Checked.HeaderText = "Checked";
            this.Checked.MinimumWidth = 6;
            this.Checked.Name = "Checked";
            this.Checked.TrueValue = "True";
            this.Checked.Width = 56;
            // 
            // The_Note
            // 
            this.The_Note.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.The_Note.HeaderText = "The-Note";
            this.The_Note.MinimumWidth = 6;
            this.The_Note.Name = "The_Note";
            this.The_Note.Width = 77;
            // 
            // Form_BearingEnhacerCAN
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1803, 737);
            this.Controls.Add(this.btn_export_data);
            this.Controls.Add(this.label_Unit);
            this.Controls.Add(this.label_Language);
            this.Controls.Add(this.comboBox_Unit);
            this.Controls.Add(this.comboBox_Language);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView_Table);
            this.Controls.Add(this.textBox_PJNum);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Check);
            this.ForeColor = System.Drawing.Color.OrangeRed;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form_BearingEnhacerCAN";
            this.Text = "Bearing Enhancer CAN";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Table)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bearingEnhancerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Check;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_PJNum;
        private System.Windows.Forms.DataGridView dataGridView_Table;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.BindingSource bearingEnhancerBindingSource;
        private System.Windows.Forms.ComboBox comboBox_Language;
        private System.Windows.Forms.ComboBox comboBox_Unit;
        private System.Windows.Forms.Label label_Language;
        private System.Windows.Forms.Label label_Unit;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btn_export_data;
        private System.Windows.Forms.DataGridViewTextBoxColumn Truss_Name;
        private System.Windows.Forms.DataGridViewComboBoxColumn No_Ply;
        private System.Windows.Forms.DataGridViewComboBoxColumn Lumber_Specie;
        private System.Windows.Forms.DataGridViewComboBoxColumn Lumber_Size;
        private System.Windows.Forms.DataGridViewComboBoxColumn DOL_Column;
        private System.Windows.Forms.DataGridViewTextBoxColumn Joint_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn X_Location;
        private System.Windows.Forms.DataGridViewTextBoxColumn Y_Location;
        private System.Windows.Forms.DataGridViewComboBoxColumn Location_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reaction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Brg_Width;
        private System.Windows.Forms.DataGridViewTextBoxColumn Req_Width;
        private System.Windows.Forms.DataGridViewComboBoxColumn Material;
        private System.Windows.Forms.DataGridViewTextBoxColumn Load_Transfer;
        private System.Windows.Forms.DataGridViewComboBoxColumn Bearing_Solution;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Vertical_Block;
        private System.Windows.Forms.DataGridViewTextBoxColumn Contact_Length;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Checked;
        private System.Windows.Forms.DataGridViewTextBoxColumn The_Note;
    }
}

