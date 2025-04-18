using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    public enum No_Solution_Enum
    {
        [Description("Please-Check-And-Input-Relevant-Data")]
        PleaseCheckAndInputRelevantData,

        [Description("Bearing-Enhancer-Is-Not-Required")]
        BearingEnhancerIsNotRequired,

        [Description("Within-5%")]
        Within5Percent,
    }
    public static class GetDescriptionExtension
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                        .GetField(value.ToString())
                        ?.GetCustomAttribute<DescriptionAttribute>()
                        ?.Description ?? value.ToString();
        }
    }
    public class Block_Info
    {
        public int Ply { get; set; } = 1;
        public bool Vertical { get; set; } = true;
        public int NumberBlock { get; set; } = 1;
        public string BlockSize { get; set; }
        public double BlockLength { get; set; }
        public string FastenerType { get; set; } = "Nail";
        public int NumberFastener { get; set; }

        double maxNumberFastener;
        public double MaxNumberFastener
        {
            get 
            {
                if (FastenerType.Contains("Nail"))
                {
                    if (BlockSize == "2x4")
                      return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x4.MaxrowNail*(NumberBlock<3?NumberBlock:1);
                    else if (BlockSize == "2x6")
                        return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x6.MaxrowNail * (NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x8")
                        return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x8.MaxrowNail * (NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x10")
                        return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x10.MaxrowNail * (NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x12")
                        return (BlockLength - Fastener_Spacing_Info.Nail.EndDistance * 2) / Fastener_Spacing_Info.Nail.MinSpacing * Fastener_Rows_Infor.Lumber_2x12.MaxrowNail * (NumberBlock < 3 ? NumberBlock : 1);
                    else
                        return 0;
                }
                else if(FastenerType.Contains("SDW22300"))
                {
                    if (BlockSize == "2x4")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x4.MaxrowSDW*(Ply==2&&NumberBlock<3?NumberBlock:1);
                    else if (BlockSize == "2x6")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x6.MaxrowSDW * (Ply == 2 && NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x8")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x8.MaxrowSDW * (Ply == 2 && NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x10")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x10.MaxrowSDW * (Ply == 2 && NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x12")
                        return (BlockLength - Fastener_Spacing_Info.SDW.EndDistance * 2) / Fastener_Spacing_Info.SDW.MinSpacing * Fastener_Rows_Infor.Lumber_2x12.MaxrowSDW * (Ply == 2 && NumberBlock < 3 ? NumberBlock : 1);
                    else
                        return 0;
                }
                else if (FastenerType.Contains("SDS25300"))
                {
                    if (BlockSize == "2x4")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x4.MaxrowSDS * (Ply == 2 && NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x6")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x6.MaxrowSDS * (Ply == 2 && NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x8")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x8.MaxrowSDS * (Ply == 2 && NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x10")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x10.MaxrowSDS * (Ply == 2 && NumberBlock < 3 ? NumberBlock : 1);
                    else if (BlockSize == "2x12")
                        return (BlockLength - Fastener_Spacing_Info.SDS.EndDistance * 2) / Fastener_Spacing_Info.SDS.MinSpacing * Fastener_Rows_Infor.Lumber_2x12.MaxrowSDS * (Ply == 2 && NumberBlock < 3 ? NumberBlock : 1);
                    else
                        return 0;
                }
                else if (FastenerType.Contains("SDW22458"))
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
                else if (FastenerType.Contains("SDS25412"))
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
                else if (FastenerType.Contains("SDW22638"))
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
                else if (FastenerType.Contains("SDS25600"))
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
        public Block_Info(int ply, bool vertical, int numBlock, string blockSize, double blockLength, string fastenerType)
        {
            Ply = ply;
            Vertical = vertical;
            NumberBlock = numBlock;
            BlockSize = blockSize;
            BlockLength = blockLength;
            FastenerType = fastenerType;
        }
        
    }
}
    
