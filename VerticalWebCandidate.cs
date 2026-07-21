using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    public class VerticalWebCandidate
    {
        public bool Web_PassThrough { get; set; } = false;
        public string Ver_Web_Size { get; set; }
        public string Ver_Lumber_Width { get; set; }
        public string Ver_Lumber_Thickness { get; set; }
        public string Ver_Web_Specie { get; set; }
        public double Contact_Length { get; set; }
        public (int No_, string Name, string key, string[] Cordinates_LeftEnd, string[] Cordinates_RightEnd) Vertical_Web_Memnber;
        public VerticalWebCandidate() { }
    }
}
