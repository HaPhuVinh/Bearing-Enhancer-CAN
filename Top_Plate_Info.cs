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
        Duration_Factor DOL {  get; set; }
        public string JointID { get; set; }
        public string XLocation { get; set; }
        public string YLocation { get; set; }
        public double Reaction { get; set; }
        public string BearingWidth { get; set; }
        public string RequireWidth { get; set; }
        public string Material { get; set; }
        public string LoadTransfer { get; set; }

        public Dictionary<int,Top_Plate_Info> Get_TopPlate_Info(string txtpath,string trussname)
        {
            //Get data from .txt file
            Dictionary<int,Top_Plate_Info> dictTopPlate = new Dictionary<int, Top_Plate_Info>();
            string txtPath = txtpath;
            string temName="";
            string[] arrLine = {};
            string[] arrLine2 = {};
            string content = File.ReadAllText(txtPath);
            string[] arrFile = content.Split('\n');
            int i = 0;
            bool pickup_React = false;
            bool pickup_Loading = false;
            Duration_Factor kD = new Duration_Factor();
            foreach (string line in arrFile)//Pick up Loading and Reaction Summary
            {
                Array.Resize(ref arrLine, 0);
                arrLine = line.Split(':');
                

                if (line.Contains("Truss:") && line.Contains("Qty"))
                {
                    temName = $"Truss:{arrLine[arrLine.Length - 1]}";
                    temName = temName.TrimEnd('\r');
                }

                if(line.Contains("Jnt")&& line.Contains("X-Loc") && line.Contains("React") && line.Contains("Up") && line.Contains("Width") && line.Contains("Reqd") && line.Contains("Mat"))
                {
                    pickup_React = true;
                    continue;
                }

                if (line.Contains("Building Code:"))
                {
                    pickup_Loading = true;
                    continue;
                }

                if ($"Truss:  {trussname}" == temName && pickup_Loading == true)
                {
                    Array.Resize(ref arrLine2, 0);
                    string line2 = line.TrimEnd('\r', '\n', '\t');
                    line2 = Regex.Replace(line, @"\s+", " ");
                    arrLine2 = line2.Split();
                    if (line.Contains(@"Kd") && line.Contains(@"(Snow)"))
                    {
                        
                        kD.DOL_Snow = arrLine2[6];
                    }

                    if (line.Contains(@"Kd") && line.Contains(@"(Live)"))
                    {
                        kD.DOL_Live = arrLine2[4];
                    }

                    if (line.Contains(@"Kd") && line.Contains(@"(Wind)"))
                    {
                        kD.DOL_Wind = arrLine2[4];
                    }
                }

                if ($"Truss:  {trussname}" == temName && pickup_React == true)
                {
                    if (line.Contains("Max Horiz ="))
                    {
                        break;
                    }
                    
                    Array.Resize(ref arrLine2, 0);
                    string line2 = line.TrimEnd('\r', '\n', '\t');
                    line2 = Regex.Replace(line, @"\s+", " ");
                    arrLine2 = line2.Split();
                    if (arrLine2.Length==9 && arrLine2[6].Contains(@"**"))
                    {
                        Top_Plate_Info tPI = new Top_Plate_Info();
                        tPI.JointID = arrLine2[1];
                        tPI.XLocation = arrLine2[2];
                        tPI.Reaction = double.Parse(arrLine2[3]);
                        tPI.BearingWidth = arrLine2[5];
                        tPI.RequireWidth = arrLine2[6];
                        tPI.Material = arrLine2[7];
                        tPI.DOL = kD;
                        dictTopPlate.Add(i,tPI);
                        i = i + 1;
                    }

                }
                
                
            }

            return dictTopPlate;
        }
    }

    
}
