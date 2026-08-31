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
                    throw new Exception(
                        "update.json not found.");
                }

                UpdateRequest request =
                    JsonConvert.DeserializeObject<UpdateRequest>(
                        File.ReadAllText(updateRequestFile));

                zipFile = request.ZipFile;
                mainExe = request.MainExe;

                DirectoryInfo updaterFolder =
                    new DirectoryInfo(
                        AppDomain.CurrentDomain.BaseDirectory);

                string rootFolder =
                    updaterFolder.Parent.FullName;

                WriteLog(
                    $"BaseDirectory = {AppDomain.CurrentDomain.BaseDirectory}");

                WriteLog(
                    $"RootFolder = {rootFolder}");

                lblStatus.Text = "Updating...";
                progressBar1.Style = ProgressBarStyle.Marquee;

                await Task.Run(() =>
                {
                    WriteLog("Update started");

                    WaitForMainAppToClose();

                    Thread.Sleep(2000);

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

                    WriteLog("STEP A - Before Extract");

                    ZipFile.ExtractToDirectory(
                        zipFile,
                        extractFolder);

                    WriteLog("STEP A - After Extract");

                    string extractedReleaseFolder =
                        Path.Combine(
                            extractFolder,
                            "Release");

                    if (!Directory.Exists(extractedReleaseFolder))
                    {
                        throw new Exception(
                            "Release folder not found in update package.");
                    }

                    WriteLog("STEP B - Before Find EXE");

                    string extractedExe =
                        Directory.GetFiles(
                            extractedReleaseFolder,
                            "*.exe",
                            SearchOption.TopDirectoryOnly)
                        .FirstOrDefault();

                    WriteLog("STEP B - After Find EXE");

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
                        $"New Release Folder = {newReleaseFolder}");

                    if (Directory.Exists(newReleaseFolder))
                    {
                        Directory.Delete(
                            newReleaseFolder,
                            true);
                    }

                    WriteLog("STEP C - Before DirectoryCopy");

                    DirectoryCopy(
                        extractedReleaseFolder,
                        newReleaseFolder,
                        true);

                    WriteLog("STEP C - After DirectoryCopy");

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
                    "Starting launcher...";

                progressBar1.Style =
                    ProgressBarStyle.Continuous;

                string exeToStart =
                    Path.Combine(
                        rootFolder,
                        $"Release_{request.NewVersion}",
                        Path.GetFileName(mainExe));

                if (!File.Exists(exeToStart))
                {
                    throw new Exception(
                        $"Cannot find updated executable:\n{exeToStart}");
                }

                try
                {
                    string launcherPath =
                        Path.Combine(
                            rootFolder,
                            "Bearing Enhancer Launcher.exe");

                    WriteLog(
                        $"Launcher Path = {launcherPath}");

                    if (!File.Exists(launcherPath))
                    {
                        throw new Exception(
                            $"Launcher not found:\r\n{launcherPath}");
                    }

                    WriteLog("STEP D - Before Start Launcher");

                    Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = launcherPath,
                            UseShellExecute = true
                        });

                    WriteLog("STEP D - After Start Launcher");
                }
                catch (Exception ex)
                {
                    WriteLog(ex.ToString());

                    MessageBox.Show(
                        "Unable to start Launcher.",
                        "Update Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

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

        private static void DirectoryCopy(string sourceDir, string destDir, bool copySubDirs)
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
                        AppDomain.CurrentDomain.BaseDirectory,
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
