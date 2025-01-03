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
        public double Reaction { get; set; }
        public string TOPSpecie { get; set; }
        public string BearingWidth { get; set; }
        public string InterfaceWidth { get; set; }
        public string RequireWidth { get; set; }
        public string LoadTransfer { get; set; }

        public void Get_TOP_Info(string PJN,string trussname)
        {
            string proNumber = PJN;
            string trussName = trussname;
            string path = @"";
            List<string> txtFile = new List<string>();
            string[] arrLine;
             
            path = @"C:\SST-EA\Client\Projects\"+ proNumber + @"\Temp\" + proNumber + ".txt";//Need to see more
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line=reader.ReadLine()) != null)
                {
                    if (line.Contains("Qty:") && line.Contains("Truss:")&&line.Contains(trussName))
                    {

                        arrLine = line.Split();

                    }
                    //txtFile.Add(line);

                }
            }
            
        }
    }

    
}
