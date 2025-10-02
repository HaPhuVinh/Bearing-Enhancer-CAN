using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            // args[0]: path to zip file
            // args[1]: path to extract (app directory)
            // args[2]: path to main exe

            if (args.Length < 3)
            {
                Console.WriteLine("Usage: Updater.exe <zipPath> <extractPath> <mainExePath>");
                return;
            }

            string zipPath = args[0];
            string extractPath = args[1];
            string mainExePath = args[2];

            // 1. Đợi ứng dụng chính đóng hoàn toàn
            string mainExeName = Path.GetFileName(mainExePath);
            var running = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(mainExeName));
            foreach (var proc in running)
            {
                try
                {
                    if (!proc.HasExited)
                        proc.WaitForExit(10000); // Đợi tối đa 10s
                }
                catch { }
            }

            // 2. Giải nén ghi đè
            try
            {
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true); // Xóa thư mục cũ nếu cần
                }
                ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Overide failed: " + ex.Message);
                Thread.Sleep(3000);
                return;
            }

            // 3. Xóa file zip (nếu muốn)
            try { File.Delete(zipPath); } catch { }

            // 4. Khởi động lại ứng dụng chính
            try
            {
                Console.WriteLine($"The program is up to date!");
                Process.Start(mainExePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot restart main app: " + ex.Message);
            }
        }
    }
}
