using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    class CrushPlates_Info
    {
        public string ImpOrMet = "Imperial";
        //Get Imperial or Metric value
        double[,] cp4_dfl_spf;
        public double[,] CP4_DFL_SPF
        {
            get
            {
                if (ImpOrMet == "Imperial")
                {
                    return new double[,]
                    {
                        { 1, 4685, 3550},
                        { 2, 9370, 7100},
                        { 3, 14055, 10650},
                        { 4, 18750, 14195}
                    };
                }
                else
                {
                    return new double[,]
                    {
                        {1, 20.87, 15.81},
                        {2, 41.74, 31.63},
                        {3, 62.61, 47.44},
                        {4, 83.52, 63.23}
                    };
                }
            }
            //set { cp4_dfl_spf = value; }
        }

        double[,] cp6_dfl_spf;
        public double[,] CP6_DFL_SPF
        {
            get
            {
                if (ImpOrMet == "Imperial")
                {
                    return new double[,]
                    {
                        {1, 7365, 5575},
                        {2, 14730, 11150},
                        {3, 22095, 16725},
                        {4, 29460, 22305}
                    };
                }
                else
                {
                    return new double[,]
                    {
                        {1, 32.81, 24.83},
                        {2, 65.61, 49.67},
                        {3, 98.42, 74.50},
                        {4, 131.22, 99.35}
                    };
                }
            }
            //set { cp6_dfl_spf = value; }
        }

        public CrushPlates_Info(string imormet)
        {
            ImpOrMet = imormet;
        }
    }
}
