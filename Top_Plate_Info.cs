using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Get_TOP_Info(string PJN,string trussname)
        {
            string proNumber = PJN;
            //string trussName = trussname;
            string tempName="";
            string path = "";
            List<string> txtFile = new List<string>();
            string[] arrLine = {""};
            path = @"C:\SST-EA\Client\Projects\"+ proNumber + @"\Temp\" + proNumber + ".txt";//Need to see more
            string content = File.ReadAllText(path);
            string[] arrFile = content.Split('\n');
            int i = 0;
            bool pickup = false;
            foreach (string line in arrFile)
            {
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
                    string line2 = line.Trim();
                    string[] arrLine2 = line2.Split();
                    Top_Plate_Info tPI = new Top_Plate_Info();
                    tPI.JointID = arrLine2[0];
                    tPI.XLocation = arrLine2[1];
                    tPI.Reaction = double.Parse(arrLine2[2]);
                    tPI.BearingWidth = arrLine2[3];
                    tPI.RequireWidth = arrLine2[4];
                    tPI.Material = arrLine2[5];
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
