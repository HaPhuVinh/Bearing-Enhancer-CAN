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
            this.button_Check = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_PJNum = new System.Windows.Forms.TextBox();
            this.dataGridView_Truss = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Truss)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Check
            // 
            this.button_Check.BackColor = System.Drawing.Color.White;
            this.button_Check.Font = new System.Drawing.Font("Arial Black", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Check.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button_Check.Location = new System.Drawing.Point(587, 11);
            this.button_Check.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_Check.Name = "button_Check";
            this.button_Check.Size = new System.Drawing.Size(121, 45);
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
            this.label1.Size = new System.Drawing.Size(187, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Project Number path:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBox_PJNum
            // 
            this.textBox_PJNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_PJNum.Location = new System.Drawing.Point(207, 15);
            this.textBox_PJNum.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_PJNum.Name = "textBox_PJNum";
            this.textBox_PJNum.Size = new System.Drawing.Size(306, 24);
            this.textBox_PJNum.TabIndex = 2;
            this.textBox_PJNum.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // dataGridView_Truss
            // 
            this.dataGridView_Truss.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Truss.Location = new System.Drawing.Point(10, 126);
            this.dataGridView_Truss.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dataGridView_Truss.Name = "dataGridView_Truss";
            this.dataGridView_Truss.RowHeadersWidth = 51;
            this.dataGridView_Truss.RowTemplate.Height = 24;
            this.dataGridView_Truss.Size = new System.Drawing.Size(1306, 477);
            this.dataGridView_Truss.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.label2.Location = new System.Drawing.Point(8, 105);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Trusses need to be considered:";
            // 
            // Form_BE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1324, 613);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView_Truss);
            this.Controls.Add(this.textBox_PJNum);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Check);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form_BE";
            this.Text = "Bearing Enhancer CAN";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Truss)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Check;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_PJNum;
        private System.Windows.Forms.DataGridView dataGridView_Truss;
        private System.Windows.Forms.Label label2;
    }
}

