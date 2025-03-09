namespace Bearing_Enhancer_CAN
{
    partial class Form_BE
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BE));
            this.button_Check = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_PJNum = new System.Windows.Forms.TextBox();
            this.dataGridView_Table = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.bearingEnhancerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Truss_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.No_Ply = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Lumber_Specie = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Lumber_Size = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DOL_Column = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Joint_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.X_Location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y_Location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reaction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Brg_Width = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Req_Width = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Material = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Load_Transfer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bearing_Solution = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Chosen_Solution = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Table)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bearingEnhancerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Check
            // 
            this.button_Check.BackColor = System.Drawing.Color.White;
            this.button_Check.Font = new System.Drawing.Font("Arial Black", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Check.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button_Check.Location = new System.Drawing.Point(783, 14);
            this.button_Check.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Check.Name = "button_Check";
            this.button_Check.Size = new System.Drawing.Size(161, 55);
            this.button_Check.TabIndex = 0;
            this.button_Check.Text = "CHECK";
            this.button_Check.UseVisualStyleBackColor = false;
            this.button_Check.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Black", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(222, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Project Number path:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBox_PJNum
            // 
            this.textBox_PJNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_PJNum.Location = new System.Drawing.Point(276, 18);
            this.textBox_PJNum.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_PJNum.Name = "textBox_PJNum";
            this.textBox_PJNum.Size = new System.Drawing.Size(407, 28);
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
            this.Reaction,
            this.Brg_Width,
            this.Req_Width,
            this.Material,
            this.Load_Transfer,
            this.Bearing_Solution,
            this.Checked,
            this.Chosen_Solution});
            this.dataGridView_Table.Location = new System.Drawing.Point(3, 107);
            this.dataGridView_Table.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView_Table.Name = "dataGridView_Table";
            this.dataGridView_Table.RowHeadersWidth = 51;
            this.dataGridView_Table.RowTemplate.Height = 24;
            this.dataGridView_Table.Size = new System.Drawing.Size(1916, 772);
            this.dataGridView_Table.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.label2.Location = new System.Drawing.Point(11, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Data Table";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // bearingEnhancerBindingSource
            // 
            this.bearingEnhancerBindingSource.DataSource = typeof(Bearing_Enhancer_CAN.Bearing_Enhancer);
            // 
            // Truss_Name
            // 
            this.Truss_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Truss_Name.DataPropertyName = "TrussName";
            this.Truss_Name.HeaderText = "Truss-Name";
            this.Truss_Name.MinimumWidth = 6;
            this.Truss_Name.Name = "Truss_Name";
            this.Truss_Name.Width = 111;
            // 
            // No_Ply
            // 
            this.No_Ply.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.No_Ply.DataPropertyName = "Ply";
            this.No_Ply.HeaderText = "No.-Ply";
            this.No_Ply.MinimumWidth = 6;
            this.No_Ply.Name = "No_Ply";
            this.No_Ply.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.No_Ply.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.No_Ply.Width = 80;
            // 
            // Lumber_Specie
            // 
            this.Lumber_Specie.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Lumber_Specie.DataPropertyName = "LumSpecie";
            this.Lumber_Specie.HeaderText = "Lumber-Specie";
            this.Lumber_Specie.MinimumWidth = 6;
            this.Lumber_Specie.Name = "Lumber_Specie";
            this.Lumber_Specie.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Lumber_Specie.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Lumber_Specie.Width = 128;
            // 
            // Lumber_Size
            // 
            this.Lumber_Size.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Lumber_Size.DataPropertyName = "LumSize";
            this.Lumber_Size.HeaderText = "Lumber-Size";
            this.Lumber_Size.MinimumWidth = 6;
            this.Lumber_Size.Name = "Lumber_Size";
            this.Lumber_Size.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Lumber_Size.Width = 88;
            // 
            // DOL_Column
            // 
            this.DOL_Column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DOL_Column.DataPropertyName = "DOL";
            this.DOL_Column.HeaderText = "D-O-L";
            this.DOL_Column.MinimumWidth = 6;
            this.DOL_Column.Name = "DOL_Column";
            this.DOL_Column.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DOL_Column.Width = 48;
            // 
            // Joint_ID
            // 
            this.Joint_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Joint_ID.DataPropertyName = "JointID";
            this.Joint_ID.HeaderText = "Joint-ID";
            this.Joint_ID.MinimumWidth = 6;
            this.Joint_ID.Name = "Joint_ID";
            this.Joint_ID.Width = 81;
            // 
            // X_Location
            // 
            this.X_Location.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.X_Location.HeaderText = "X-Location";
            this.X_Location.MinimumWidth = 6;
            this.X_Location.Name = "X_Location";
            this.X_Location.Width = 99;
            // 
            // Y_Location
            // 
            this.Y_Location.HeaderText = "Y-Location";
            this.Y_Location.MinimumWidth = 6;
            this.Y_Location.Name = "Y_Location";
            this.Y_Location.Width = 125;
            // 
            // Reaction
            // 
            this.Reaction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Reaction.HeaderText = "Reaction";
            this.Reaction.MinimumWidth = 6;
            this.Reaction.Name = "Reaction";
            this.Reaction.Width = 90;
            // 
            // Brg_Width
            // 
            this.Brg_Width.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Brg_Width.HeaderText = "Bearing-Width";
            this.Brg_Width.MinimumWidth = 6;
            this.Brg_Width.Name = "Brg_Width";
            this.Brg_Width.Width = 121;
            // 
            // Req_Width
            // 
            this.Req_Width.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Req_Width.HeaderText = "Required-Width";
            this.Req_Width.MinimumWidth = 6;
            this.Req_Width.Name = "Req_Width";
            this.Req_Width.Width = 130;
            // 
            // Material
            // 
            this.Material.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Material.HeaderText = "Material";
            this.Material.MinimumWidth = 6;
            this.Material.Name = "Material";
            this.Material.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Material.Width = 61;
            // 
            // Load_Transfer
            // 
            this.Load_Transfer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Load_Transfer.HeaderText = "Load-Transfer";
            this.Load_Transfer.MinimumWidth = 6;
            this.Load_Transfer.Name = "Load_Transfer";
            this.Load_Transfer.ReadOnly = true;
            this.Load_Transfer.Width = 121;
            // 
            // Bearing_Solution
            // 
            this.Bearing_Solution.HeaderText = "Bearing-Solution";
            this.Bearing_Solution.MinimumWidth = 6;
            this.Bearing_Solution.Name = "Bearing_Solution";
            this.Bearing_Solution.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Bearing_Solution.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Bearing_Solution.Width = 125;
            // 
            // Checked
            // 
            this.Checked.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Checked.FalseValue = "0";
            this.Checked.HeaderText = "Checked";
            this.Checked.MinimumWidth = 6;
            this.Checked.Name = "Checked";
            this.Checked.TrueValue = "1";
            this.Checked.Width = 67;
            // 
            // Chosen_Solution
            // 
            this.Chosen_Solution.HeaderText = "Chosen-Solution";
            this.Chosen_Solution.MinimumWidth = 6;
            this.Chosen_Solution.Name = "Chosen_Solution";
            this.Chosen_Solution.Width = 125;
            // 
            // Form_BE
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 895);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView_Table);
            this.Controls.Add(this.textBox_PJNum);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Check);
            this.ForeColor = System.Drawing.Color.OrangeRed;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form_BE";
            this.Text = "Bearing Enhancer CAN";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Table)).EndInit();
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
        private System.Windows.Forms.DataGridViewTextBoxColumn Truss_Name;
        private System.Windows.Forms.DataGridViewComboBoxColumn No_Ply;
        private System.Windows.Forms.DataGridViewComboBoxColumn Lumber_Specie;
        private System.Windows.Forms.DataGridViewComboBoxColumn Lumber_Size;
        private System.Windows.Forms.DataGridViewComboBoxColumn DOL_Column;
        private System.Windows.Forms.DataGridViewTextBoxColumn Joint_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn X_Location;
        private System.Windows.Forms.DataGridViewTextBoxColumn Y_Location;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reaction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Brg_Width;
        private System.Windows.Forms.DataGridViewTextBoxColumn Req_Width;
        private System.Windows.Forms.DataGridViewComboBoxColumn Material;
        private System.Windows.Forms.DataGridViewTextBoxColumn Load_Transfer;
        private System.Windows.Forms.DataGridViewComboBoxColumn Bearing_Solution;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Checked;
        private System.Windows.Forms.DataGridViewTextBoxColumn Chosen_Solution;
    }
}

