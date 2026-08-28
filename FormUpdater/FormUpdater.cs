using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO.Compression;
using Newtonsoft.Json;

namespace FormUpdater
{
    public partial class FormUpdater : Form
    {
        private string zipFile;
        private string appDir;
        private string mainExe;

        public FormUpdater()
        {
            InitializeComponent();
        }

        private async void FormUpdater_Load(object sender, EventArgs e)
        {
            string updateRequestFile = null;
            
            try
            {
                updateRequestFile =
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "update.json");

                if (!File.Exists(updateRequestFile))
                {
                    MessageBox.Show(
                        "update.json not found.",
                        "Update Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                UpdateRequest request =
                    JsonConvert.DeserializeObject<UpdateRequest>(
                        File.ReadAllText(updateRequestFile));

                zipFile = request.ZipFile;
                appDir = request.AppDir;
                mainExe = request.MainExe;

                string releaseFolder =
                    appDir.TrimEnd('\\');

                string rootFolder =
                    Directory
                        .GetParent(releaseFolder)
                        .FullName;

                lblStatus.Text = "Updating...";
                progressBar1.Style = ProgressBarStyle.Marquee;

                await Task.Run(() =>
                {
                    WriteLog("Update started");

                    WaitForMainAppToClose();

                    Thread.Sleep(3000);

                    string extractFolder =
                        Path.Combine(
                            Path.GetTempPath(),
                            "BearingEnhancer_Update");

                    if (Directory.Exists(extractFolder))
                    {
                        Directory.Delete(
                            extractFolder,
                            true);
                    }

                    WriteLog("Extracting update package");

                    ZipFile.ExtractToDirectory(
                        zipFile,
                        extractFolder);

                    string extractedReleaseFolder =
                        Path.Combine(
                            extractFolder,
                            "Release");

                    if (!Directory.Exists(extractedReleaseFolder))
                    {
                        throw new Exception(
                            "Release folder not found in update package.");
                    }

                    WriteLog(
                        $"Extract Folder: {extractFolder}");

                    WriteLog(
                        $"Extracted Release Folder: {extractedReleaseFolder}");

                    string extractedExe =
                        Directory.GetFiles(
                            extractedReleaseFolder,
                            "*.exe",
                            SearchOption.TopDirectoryOnly)
                        .FirstOrDefault();

                    WriteLog(
                        $"Found EXE: {extractedExe}");

                    if (string.IsNullOrEmpty(extractedExe))
                    {
                        throw new Exception(
                            "Executable file not found in update package.");
                    }

                    string newReleaseFolder =
                        Path.Combine(
                            rootFolder,
                            $"Release_{request.NewVersion}");

                    WriteLog(
                        $"Source: {extractedReleaseFolder}");

                    WriteLog(
                        $"Target: {newReleaseFolder}");

                    WriteLog(
                        $"New release folder: {newReleaseFolder}");

                    if (Directory.Exists(newReleaseFolder))
                    {
                        Directory.Delete(
                            newReleaseFolder,
                            true);
                    }

                    DirectoryCopy(
                        extractedReleaseFolder,
                        newReleaseFolder,
                        true);

                    try
                    {
                        File.Delete(zipFile);
                    }
                    catch
                    {
                    }

                    try
                    {
                        Directory.Delete(
                            extractFolder,
                            true);
                    }
                    catch
                    {
                    }

                    WriteLog("Update completed");
                });

                lblStatus.Text =
                    "Update completed. Restarting...";

                progressBar1.Style =
                    ProgressBarStyle.Continuous;

                rootFolder =
                    Directory
                        .GetParent(
                            releaseFolder)
                        .FullName;

                string exeToStart =
                    Path.Combine(
                        rootFolder,
                        $"Release_{request.NewVersion}",
                        Path.GetFileName(mainExe));

                if (!File.Exists(exeToStart))
                {
                    throw new Exception(
                        $"Cannot find updated application:\n{exeToStart}");
                }

                try
                {
                    Process p = Process.Start(exeToStart);

                    if (p == null)
                    {
                        throw new Exception(
                            "Failed to start updated application.");
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(ex.ToString());

                    MessageBox.Show(
                        "Unable to start new version.\r\n" +
                        "Launching previous version.");

                    Process.Start(mainExe);

                    Application.Exit();

                    return;
                }

                try
                {
                    File.Delete(updateRequestFile);
                }
                catch
                {
                }

                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Update Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                WriteLog(ex.ToString());
            }
        }


        private void WaitForMainAppToClose()
        {
            string processName =
                Path.GetFileNameWithoutExtension(mainExe);

            while (true)
            {
                Process[] processes =
                    Process.GetProcessesByName(processName);

                if (!processes.Any())
                {
                    break;
                }

                Thread.Sleep(1000);
            }

        }

        private static void DirectoryCopy(string sourceDir,string destDir,bool copySubDirs)
        {
            DirectoryInfo dir =
                new DirectoryInfo(sourceDir);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(sourceDir);
            }

            Directory.CreateDirectory(destDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFile =
                    Path.Combine(destDir, file.Name);

                file.CopyTo(targetFile, true);
            }

            if (!copySubDirs)
                return;

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                string targetDir =
                    Path.Combine(destDir, subDir.Name);

                DirectoryCopy(
                    subDir.FullName,
                    targetDir,
                    true);
            }
        }

        private void WriteLog(string message)
        {
            try
            {
                string logFile =
                    Path.Combine(
                        appDir,
                        "Update.log");

                File.AppendAllText(
                    logFile,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch
            {
            }
        }
    }
    public class UpdateRequest
    {
        public string ZipFile { get; set; }

        public string AppDir { get; set; }

        public string MainExe { get; set; }

        public string NewVersion { get; set; }
    }
}
