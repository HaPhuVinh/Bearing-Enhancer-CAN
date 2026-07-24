using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    public class VerticalWebCandidate
    {
        public string Name { get; set; }
        public bool Web_PassThrough { get; set; } = false;
        public string Ver_Web_Size { get; set; }
        public string Ver_Lumber_Width { get; set; }
        public string Ver_Lumber_Thickness { get; set; }
        public string Ver_Web_Specie { get; set; }
        public double Contact_Length { get; set; }
        public List<string[]> Left_Coordinates { get; set; }
        public List<string[]> Right_Coordinates { get; set; }
        public (int No_, string Name, string key, string[] Cordinates_LeftEnd, string[] Cordinates_RightEnd) Vertical_Web_Member;
        public (double A, double B, double C) Bottom_Line { get; set; }
        public VerticalWebCandidate() { }
        public VerticalWebCandidate ((int No_, string Name, string key, string[] Cordinates_LeftEnd, string[] Cordinates_RightEnd) web, List<LumberInventory> listlumberinventory)
        {
            foreach (LumberInventory LI in listlumberinventory)
            {
                if (web.key == LI.Lumber_Key)
                {
                    Ver_Web_Specie = LI.Lumber_SpeciesName;
                    Ver_Web_Size = LI.Lumber_Size;
                    Ver_Lumber_Width = LI.Lumber_Width;
                    Ver_Lumber_Thickness = LI.Lumber_Thickness;
                    for (int i = 0; i < web.Cordinates_LeftEnd.Length; i += 3)
                    {
                        string[] LL = new string[] { web.Cordinates_LeftEnd[i].Trim(), web.Cordinates_LeftEnd[i + 1].Trim(), web.Cordinates_LeftEnd[i + 2].Trim() };
                        Left_Coordinates.Add(LL);
                    }
                    for (int i = 0; i < web.Cordinates_RightEnd.Length; i += 3)
                    {
                        string[] RR = new string[] { web.Cordinates_RightEnd[i].Trim(), web.Cordinates_RightEnd[i + 1].Trim(), web.Cordinates_RightEnd[i + 2].Trim() };
                        Right_Coordinates.Add(RR);
                    }
                }
            }
        }
    }
}
