using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FormUpdater
{
    public partial class FormUpdater : Form
    {
        public FormUpdater()
        {
            InitializeComponent();
        }

        private string zipFile, appDir, mainExe;

        public FormUpdater(string[] args)
        {
            InitializeComponent();

            if (args.Length != 3)
            {
                MessageBox.Show("Wrong Parameters!");
                Environment.Exit(1); // Dừng 
                return;
            }

            zipFile = args[0];
            appDir = args[1];
            mainExe = args[2];
        }

        private async void FormUpdater_Load(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Updating...";
                progressBar1.Style = ProgressBarStyle.Marquee;

                await Task.Run(() =>
                {
                    // 1. Xóa file cũ (tùy chọn)
                    // 2. Giải nén
                    if (Directory.Exists(appDir))
                    {
                        foreach (var file in Directory.GetFiles(appDir))
                        {
                            File.Delete(file); // chú ý file đang chạy không được xóa
                        }
                    }
                    
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, appDir);
                });

                // 3. Xóa file zip
                try { File.Delete(zipFile); } catch { }

                lblStatus.Text = "Update completed. Restarting...";
                progressBar1.Style = ProgressBarStyle.Continuous;

                // 3. Chạy lại ứng dụng chính
                Process.Start(Path.Combine(appDir, @"Release\Bearing Enhancer CAN.exe"));

                // 4. Thoát
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update Error: " + ex.Message);
            }
        }
    }
}
