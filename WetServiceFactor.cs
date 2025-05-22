using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    public class WetServiceFactor
    {
        public bool WetService { get; set; }
        public bool GreenLumber { get; set; }
        public double Ksf => WetService ? 0.67 : (GreenLumber ? 0.8 : 1.0);
        public WetServiceFactor(bool wetService, bool greenLumber)
        {
            WetService = wetService;
            GreenLumber = greenLumber;
        }
    }
}
