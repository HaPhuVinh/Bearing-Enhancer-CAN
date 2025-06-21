using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    class CrushPlatesAndFlushPlate_Info
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
                        { 1, 5965, 4515},
                        { 2, 11390, 9030},
                        { 3, 17895, 13545},
                        { 4, 23860, 18065}
                    };
                }
                else
                {
                    return new double[,]
                    {
                        {1, 26.57, 20.11},
                        {2, 53.14, 40.22},
                        {3, 79.71, 60.33},
                        {4, 106.28, 80.47}
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
                        {1, 9370, 7095},
                        {2, 18740, 14190},
                        {3, 28110, 21285},
                        {4, 37495, 28390}
                    };
                }
                else
                {
                    return new double[,]
                    {
                        {1, 41.47, 31.60},
                        {2, 83.47, 63.21},
                        {3, 125.21, 94.81},
                        {4, 167.02, 126.46}
                    };
                }
            }
            //set { cp6_dfl_spf = value; }
        }

        public CrushPlatesAndFlushPlate_Info(string imormet)
        {
            ImpOrMet = imormet;
        }
    }
}
