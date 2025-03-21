using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    public static class Fastener_Lateral_Design_Value
    {
        public static (string Type, int SP, int DFL, int HF, int SPF, double APF) Common_Wire_10d { get; set; }
            = ("3\" Common Wire 10d [min .144\"x3\"]", 191, 177, 161, 159, 2.5);
        public static (string Type, int SP, int DFL, int HF, int SPF, double APF) Gun_Nails { get; set; }
            = ("3 1/4\" Gun Nails [min .12'\"x3.25\"]", 136, 126, 121, 113, 2.5);
        public static (string Type, int SP, int DFL, int HF, int SPF, double APF) SDW22300_Screw { get; set; }
            = ("SDW22300 Screw [.22\"x3\"]", 355, 355, 290, 290, 6.6);
        public static (string Type, int SP, int DFL, int HF, int SPF, double APF) SDW22458_Screw { get; set; }
            = ("SDW22458 Screw [.22\"x4 5/8\"]", 455, 455, 405, 405, 6.6);
        public static (string Type, int SP, int DFL, int HF, int SPF, double APF) SDW22638_Screw { get; set; }
            = ("SDW22638 Screw [.22\"x6 3/8\"]", 455, 455, 405, 405, 6.6);
        public static (string Type, int SP, int DFL, int HF, int SPF, double APF) SDS25300_Screw { get; set; }
            = ("SDS25300 Screw [.25\"x3\"]", 370, 370, 320, 320, 5.0);
        public static (string Type, int SP, int DFL, int HF, int SPF, double APF) SDS25412_Screw { get; set; }
            = ("SDS25412 Screw [.25\"x4 1/2\"]", 475, 475, 420, 420, 5.0);
        public static (string Type, int SP, int DFL, int HF, int SPF, double APF) SDS25600_Screw { get; set; }
            = ("SDS25600 Screw [.25\"x6\"]", 475, 475, 420, 420, 5.0);
    }

}
