﻿using System;
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
        public Duration_Factor DOL {  get; set; }
        public bool GreenLumber { get; set; } = false;
        public bool WetService { get; set; } = false;
        public string JointID { get; set; }
        public string XLocation { get; set; }
        public string YLocation { get; set; }
        public string Location_Type { get; set; }
        public double Reaction { get; set; }
        public string BearingWidth { get; set; }
        public string RequireWidth { get; set; }
        public string Material { get; set; }
        public double LoadTransfer { get; set; }

        public Dictionary<int,Top_Plate_Info> Get_TopPlate_Info(string txtpath,string trussname, string language, string unit)
        {
            //Get data from .txt file
            English_Or_French langText = new English_Or_French(language);
            Dictionary<int,Top_Plate_Info> dictTopPlate = new Dictionary<int, Top_Plate_Info>();
            string txtPath = txtpath;
            string temName="";
            string[] arrLine = {};
            string[] arrLine2 = {};
            string content = File.ReadAllText(txtPath);
            string[] arrFile = content.Split('\n');
            int i = 0;
            bool thisTruss = false;
            bool pickup_React = false;
            bool pickup_Loading = false;
            Duration_Factor kD = new Duration_Factor();
            foreach (string line in arrFile)//Pick up Loading and Reaction Summary
            {
                if (line.Contains(langText.Truss) && line.Contains(langText.Qty))
                {
                    Array.Resize(ref arrLine, 0);
                    arrLine = line.Split(':');
                    temName = $"{langText.Truss}{arrLine[arrLine.Length - 1]}";
                    temName = temName.Trim();
                    if (temName == $"{langText.Truss}  {trussname}")
                    {
                        thisTruss = true;
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                
                if(thisTruss) 
                {
                    if (line.Contains(langText.BuildingCode))
                    {
                        pickup_Loading = true;
                        continue;
                    }
                    if(pickup_Loading)
                    {
                        Array.Resize(ref arrLine2, 0);
                        string line2 = Regex.Replace(line, @"\s+", " ");
                        line2 = line2.Trim();
                        arrLine2 = line2.Split();
                        if (line.Contains(langText.Kd) && line.Contains(langText.Snow))
                        {
                            kD.DOL_Snow = arrLine2[6];
                        }

                        if (line.Contains(langText.Kd) && line.Contains(langText.Live))
                        {
                            kD.DOL_Live = arrLine2[4];
                        }

                        if (line.Contains(langText.Kd) && line.Contains(langText.Wind))
                        {
                            kD.DOL_Wind = arrLine2[4];
                        }
                    }

                    if (line.Contains(langText.Jnt) && line.Contains(langText.XLoc) && line.Contains(langText.React) && line.Contains(langText.Up) && line.Contains(langText.Width) && line.Contains(langText.Reqd) && line.Contains(langText.Mat))
                    {
                        pickup_React = true;
                        continue;
                    }
                    
                    if (pickup_React)
                    {
                        Array.Resize(ref arrLine2, 0);
                        string line2 = Regex.Replace(line, @"\s+", " ");
                        line2 = line2.Trim();
                        arrLine2 = line2.Split();
                        bool isLine = false;
                        if (arrLine2.Length == 7)
                        {
                            isLine = new[] { "DFL", "DFLN", "SP", "SYP", "SPF", "HF", "USER" }.Any(s => arrLine2[6].Contains(s));
                        }
                        
                        if (arrLine2.Length == 7 && isLine)
                        {
                            if (arrLine2[5].Contains(@"**"))
                            {
                                Top_Plate_Info tPI = new Top_Plate_Info();
                                tPI.JointID = arrLine2[0];
                                tPI.XLocation = arrLine2[1];
                                tPI.Reaction = double.Parse(arrLine2[2]);
                                tPI.BearingWidth = arrLine2[4];
                                tPI.RequireWidth = arrLine2[5].Trim('*');
                                tPI.Material = arrLine2[6];
                                tPI.DOL = kD;
                                dictTopPlate.Add(i, tPI);
                            }
                            i = i + 1;
                        }
                        if (line.Contains(langText.UnfactoredReactionSummary))
                        {
                            break;
                        }
                    }
                    if (line.Contains(langText.UnfactoredReactionSummary))
                    {
                        break;
                    }
                }

            }

            return dictTopPlate;
        }
    }

    
}
