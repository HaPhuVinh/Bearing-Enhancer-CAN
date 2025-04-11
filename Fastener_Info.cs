using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    public enum eLumberSpecie
    {
        SP,
        SYP,
        DFL,
        DFLN,
        SPF,
        HF,
    }
    public enum eFastenerName
    {
        Nail_Common_Wire_10d,
        SDW22300,
        SDW22458,
        SDW22638,
        SDS25300,
        SDS25412,
        SDS25600,
    }
    //public class Fastener_Lateral_Design_Value
    //{
    //    public string FastenerType { get; set; }=eFastenerName.Nail_Common_Wire_10d.ToString();
    //    (string Type, int SP, int DFL, int HF, int SPF, double APF) design_value;
    //    public (string Type, int SP, int DFL, int HF, int SPF, double APF) Design_Value
    //    {
    //        get 
    //        {
    //            if (FastenerType == "SDW22300")
    //                return ("SDW22300 Screw [.22\"x3\"]", 355, 355, 290, 290, 6.6);
    //            else if (FastenerType == "SDW22458")
    //                return ("SDW22458 Screw [.22\"x4 5/8\"]", 455, 455, 405, 405, 6.6);
    //            else if (FastenerType == "SDW22638")
    //                return ("SDW22638 Screw [.22\"x6 3/8\"]", 455, 455, 405, 405, 6.6);
    //            else if (FastenerType == "SDS25300")
    //                return ("SDS25300 Screw [.25\"x3\"]", 370, 370, 320, 320, 5.0);
    //            else if (FastenerType == "SDS25412")
    //                return ("SDS25412 Screw [.25\"x4 1/2\"]", 475, 475, 420, 420, 5.0);
    //            else if(FastenerType == "SDS25600")
    //                return ("SDS25600 Screw [.25\"x6\"]", 475, 475, 420, 420, 5.0);
    //            else
    //                return ("3\" Common Wire 10d [min .144\"x3\"]", 191, 177, 161, 159, 2.5);
    //        }
    //        set { design_value = value; }
    //    }
    //    public Fastener_Lateral_Design_Value(string fastenerType)
    //    {
    //        FastenerType = fastenerType;
    //    }
        
    //}
    public static class Fastener_Design_Value
    {
        public static List<string[]> Lateral_Design_Value { get; set; }
            = new List<string[]>()
            {
                new string[] { "Name", "Description", "SP","SYP", "DFL","DFLN", "HF", "SPF", "APF" },
                new string[] { "SDW22300", "SDW22300 Screw [.22\"x3\"]", "355","355", "355", "355", "290", "290", "6.6" },
                new string[] { "SDW22458", "SDW22458 Screw [.22\"x4 5/8\"]", "455", "455","455", "455", "405", "405", "6.6" },
                new string[] { "SDW22638", "SDW22638 Screw [.22\"x6 3/8\"]", "455", "455", "455", "455", "405", "405", "6.6" },
                new string[] { "SDS25300", "SDS25300 Screw [.25\"x3\"]", "370", "370", "370", "370", "320", "320", "5.0" },
                new string[] { "SDS25412", "SDS25412 Screw [.25\"x4 1/2\"]", "475", "475", "475", "475", "420", "420", "5.0" },
                new string[] { "SDS25600", "SDS25600 Screw [.25\"x6\"]", "475", "475", "475", "475", "420", "420", "5.0" },
                new string[] { "Nail", "3\" Common Wire 10d [min .144\"x3\"]", "191", "191", "177", "177", "161", "159", "2.5" }
            };
    }
    public static class Fastener_Rows_Infor
    {
        public static (string LumberSize, int MinrowNail, int MaxrowNail, int MinrowSDW, int MaxrowSDW, int MinrowSDS, int MaxrowSDS) Lumber_2x4 { get; set; }
            = ("2x4", 1, 2, 1, 2, 1, 2);
        public static (string LumberSize, int MinrowNail, int MaxrowNail, int MinrowSDW, int MaxrowSDW, int MinrowSDS, int MaxrowSDS) Lumber_2x6 { get; set; }
            = ("2x6", 2, 3, 2, 5, 2, 2);
        public static (string LumberSize, int MinrowNail, int MaxrowNail, int MinrowSDW, int MaxrowSDW, int MinrowSDS, int MaxrowSDS) Lumber_2x8 { get; set; }
            = ("2x8", 2, 4, 2, 8, 2, 3);
        public static (string LumberSize, int MinrowNail, int MaxrowNail, int MinrowSDW, int MaxrowSDW, int MinrowSDS, int MaxrowSDS) Lumber_2x10 { get; set; }
            = ("2x10", 3, 5, 2, 10, 2, 5);
        public static (string LumberSize, int MinrowNail, int MaxrowNail, int MinrowSDW, int MaxrowSDW, int MinrowSDS, int MaxrowSDS) Lumber_2x12 { get; set; }
            = ("2x12", 3, 6, 2, 12, 2, 6);
    }
    public static class Fastener_Spacing_Info
    {
        public static (string FastenerType, double MinSpacing, double MaxSpacing, double EndDistance) Nail { get; set; }
            = ("Nail", 3.0, 12.0, 1.5);
        public static (string FastenerType, double MinSpacing, double MaxSpacing, double EndDistance) SDW { get; set; }
            = ("SDW", 6.0, 24.0, 4.0);
        public static (string FastenerType, double MinSpacing, double MaxSpacing, double EndDistance) SDS { get; set; }
            = ("SDS", 3.0, 24.0, 4.0);
    }
    
}
