using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace Bearing_Enhancer_CAN
{
    public class Top_Plate_Info
    {
        public string JointID { get; set; }
        public string XLocation { get; set; }
        public double Reaction { get; set; }
        public string Material { get; set; }
        public string BearingWidth { get; set; }
        public string RequireWidth { get; set; }
        public string LoadTransfer { get; set; }

        public void Get_TOP_Info(string pjPath,string trussname)
        {
            string projectPath = pjPath;
            string[] arrPath = projectPath.Split('\\');
            string projectID = arrPath[arrPath.Length - 1];
            string tempName="";
            string[] arrLine = {};
            string[] arrLine2 = {};
            string content = File.ReadAllText(projectPath);
            string[] arrFile = content.Split('\n');
            int i = 0;
            bool pickup = false;
            foreach (string line in arrFile)
            {
                Array.Resize(ref arrLine, 0);
                arrLine = line.Split(':');
                

                if (line.Contains("Truss:") && line.Contains("Qty"))
                {
                    tempName = $"Truss:{arrLine[arrLine.Length - 1]}";
                    tempName = tempName.TrimEnd('\r');
                }

                if(line.Contains("Jnt")&& line.Contains("X-Loc") && line.Contains("React") && line.Contains("Up") && line.Contains("Width") && line.Contains("Reqd") && line.Contains("Mat"))
                {
                    pickup = true;
                    continue;
                }

                if (line.Contains("Max Horiz ="))
                {
                    break;
                }

                if ($"Truss:  {trussname}" == tempName && pickup == true)
                {
                    Array.Resize(ref arrLine2, 0);
                    string line2 = line.TrimEnd('\r', '\n', '\t');
                    line2 = Regex.Replace(line, @"\s+", " ");
                    arrLine2 = line2.Split();
                    Top_Plate_Info tPI = new Top_Plate_Info();
                    tPI.JointID = arrLine2[1];
                    tPI.XLocation = arrLine2[2];
                    tPI.Reaction = double.Parse(arrLine2[3]);
                    tPI.BearingWidth = arrLine2[5];
                    tPI.RequireWidth = arrLine2[6];
                    tPI.Material = arrLine2[7];
                }

                

                i = i + 1;
            }
            //using (StreamReader reader = new StreamReader(path))
            //{
            //    string line;
            //    while ((line=reader.ReadLine()) != null)
            //    {
            //        if (line.Contains("Truss:")&&line.Contains("Qty"))
            //        {
            //            arrLine = line.Split(':');
            //            string tempName = $"Truss:{arrLine[arrLine.Length-1]}";

            //        }


            //    }
            //}

        }
    }

    
}
