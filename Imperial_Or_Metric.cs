using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bearing_Enhancer_CAN
{
    public class Imperial_Or_Metric
    {
        public string ImperialOrMetric = "Imperial";
        public double miliFactor => ImperialOrMetric == "Imperial" ? 1 : 25.4;
        public double kNFactor => ImperialOrMetric == "Imperial" ? 1 : 0.00444822;

        public Imperial_Or_Metric(string imp_met) 
        {
            ImperialOrMetric = imp_met;
        }
    }
}
