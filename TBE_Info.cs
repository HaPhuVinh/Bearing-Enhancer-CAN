using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    public class TBE_Info
    {
        public string ImpOrMet = "Imperial";
        //Get Imperial or Metric value
        double[,] tbe4_dfl;
        public double[,] TBE4_DFL 
        {
            get
            {
                if (ImpOrMet == "Imperial")
                {
                    return new double [,] 
                    {
                        { 1,35407,7800 },
                        { 2,3660,12180},
                        { 3,3660,16445},
                        { 4,3660,20705}
                    };
                }
                else
                {
                    return new double[,]
                    {
                        {1,15.75,34.70 },
                        {2,16.28,54.18},
                        {3,16.28,73.15},
                        {4,16.28,92.10}
                    };
                }
            }
            //set { tbe4_dfl = value; }
        }

        double[,] tbe6_dfl;
        public double[,] TBE6_DFL
        {
            get
            {
                if (ImpOrMet == "Imperial")
                {
                    return new double[,]
                    {
                        {1,3540,10235 },
                        {2,3860,17250},
                        {3,3860,23945},
                        {4,3860,30640}
                    };
                }
                else
                {
                    return new double[,]
                    {
                        {1,15.75,45.53 },
                        {2,17.17,76.73},
                        {3,17.17,106.52},
                        {4,17.17,136.30}
                    };
                }
            }
            //set { tbe6_dfl = value; }
        }

        double[,] tbe4_spf;
        public double[,] TBE4_SPF
        {
            get
            {
                if (ImpOrMet == "Imperial")
                {
                    return new double[,]
                    {
                        {1,3220,6445 },
                        {2,3440,9890},
                        {3,3440,13120},
                        {4,3440,16345}
                    };
                }
                else
                {
                    return new double[,]
                    {
                        {1,14.32,28.67 },
                        {2,15.30,43.99},
                        {3,15.30,58.36},
                        {4,15.30,72.71}
                    };
                }
            }
            //set { tbe4_spf = value; }
        }

        double[,] tbe6_spf;
        public double[,] TBE6_SPF
        {
            get
            {
                if (ImpOrMet == "Imperial")
                {
                    return new double[,]
                    {
                        {1,3220,8290 },
                        {2,3540,13680},
                        {3,3540,18750},
                        {4,3540,23820}
                    };
                }
                else
                {
                    return new double[,]
                    {
                        {1,14.32,36.88 },
                        {2,15.75,60.85},
                        {3,15.75,83.41},
                        {4,15.75,105.96}
                    };
                }
            }
            //set { tbe6_spf = value; }
        }

        //Metric
        //public double[,] kN_TBE4_DFL = {
        //    //{ply, TBEOnly, TBEandWoodBearing}
        //    {1,15.75,34.70 },
        //    {2,16.28,54.18},
        //    {3,16.28,73.15},
        //    {4,16.28,92.10}
        //};
        //public double[,] kN_TBE6_DFL = {
        //    {1,15.75,45.53 },
        //    {2,17.17,76.73},
        //    {3,17.17,106.52},
        //    {4,17.17,136.30}
        //};
        //public double[,] kN_TBE4_SPF = {
        //    {1,14.32,28.67 },
        //    {2,15.30,43.99},
        //    {3,15.30,58.36},
        //    {4,15.30,72.71}
        //};
        //public double[,] kN_TBE6_SPF = {
        //    {1,14.32,36.88 },
        //    {2,15.75,60.85},
        //    {3,15.75,83.41},
        //    {4,15.75,105.96}
        //};
        public TBE_Info(string imormet)
        {
            ImpOrMet = imormet;
        }
        //public double[,] Get_TBE_Data()
        //{

        //    return new double[,] { };
        //}
    }
}
