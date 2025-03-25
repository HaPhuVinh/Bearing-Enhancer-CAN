using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    public class Block_Info
    {
        public bool Vertical { get; set; } = true;
        public int NumberBlock { get; set; } = 1;
        public string BlockSize { get; set; }
        public double BlockLength { get; set; }
        public string FastenerType { get; set; } = "Nail";

        double maxNumberFastener;
        public double MaxNumberFastener
        {
            get 
            {
                if (FastenerType.Contains("Nail"))
                {
                    if (BlockSize == "2x4")
                      return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x4.MaxrowNail*(NumberBlock<3?NumberBlock:0);
                    else if (BlockSize == "2x6")
                        return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x6.MaxrowNail * (NumberBlock < 3 ? NumberBlock : 0);
                    else if (BlockSize == "2x8")
                        return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x8.MaxrowNail * (NumberBlock < 3 ? NumberBlock : 0);
                    else if (BlockSize == "2x10")
                        return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x10.MaxrowNail * (NumberBlock < 3 ? NumberBlock : 0);
                    else if (BlockSize == "2x12")
                        return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x12.MaxrowNail * (NumberBlock < 3 ? NumberBlock : 0);
                    else
                        return 0;
                }
                else if(FastenerType.Contains("SDW"))
                {
                    if (BlockSize == "2x4")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x4.MaxrowSDW;
                    else if (BlockSize == "2x6")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x6.MaxrowSDW;
                    else if (BlockSize == "2x8")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x8.MaxrowSDW;
                    else if (BlockSize == "2x10")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x10.MaxrowSDW;
                    else if (BlockSize == "2x12")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x12.MaxrowSDW;
                    else
                        return 0;
                }
                else if (FastenerType.Contains("SDS"))
                {
                    if (BlockSize == "2x4")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x4.MaxrowSDS;
                    else if (BlockSize == "2x6")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x6.MaxrowSDS;
                    else if (BlockSize == "2x8")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x8.MaxrowSDS;
                    else if (BlockSize == "2x10")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x10.MaxrowSDS;
                    else if (BlockSize == "2x12")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x12.MaxrowSDS;
                    else
                        return 0;
                }
                else
                    return 0;

            }
            set { maxNumberFastener = value; }
        }
        //public int MinNumberFastener { get; set; }
        public Block_Info(bool vertical, int numBlock, string blockSize, double blockLength, string fastenerType)
        {
            Vertical = vertical;
            NumberBlock = numBlock;
            BlockSize = blockSize;
            BlockLength = blockLength;
            FastenerType = fastenerType;
        }
    }
}
    
