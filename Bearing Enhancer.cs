﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Reflection;
using iText.Layout.Element;
using Microsoft.Office.Interop.Excel;

namespace Bearing_Enhancer_CAN
{
    public class Bearing_Enhancer
    {
        public string TrussName { get; set; }
        public string Ply { get; set; }
        public string LumSpecie { get; set; }
        public string LumSize { get; set; }
        public string LumWidth { get; set; }
        public string LumThick { get; set; }
        public Top_Plate_Info TopPlateInfo { get; set; }
        public List<string> BearingSolution { get; set; }

        public Bearing_Enhancer()
        {
        }

        #region Service Method
        public virtual string Generate_Enhancer_Note(string bearingsolution, string language, string unit)
        {
            return "";
        }
        public List<Bearing_Enhancer> Get_Bearing_Info(string txtpath, string language, string unit)
        {
            //Get Data from tdlTruss file
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit);
            string txtPath = txtpath;
            string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(txtPath));
            string trussesPath = Path.Combine(projectPath,"Trusses");
            //string[] arrPath = trussesPath.Split('\\');
            //string projectID = arrPath[arrPath.Length - 2];
            string fileName = @"";
            string extName = @"";
            int j = 0;
            //List<string> subListTrussName = new List<string>();
            List<string> mainListTrussName = new List<string>();
            List<Bearing_Enhancer> bearingEnhancerItems = new List<Bearing_Enhancer>();
            Dictionary<int, Top_Plate_Info> dictTopPlate = new Dictionary<int, Top_Plate_Info>();
            Top_Plate_Info tpi = new Top_Plate_Info();
            mainListTrussName = Directory.GetFiles(trussesPath).ToList();
            Dictionary<string, int> orderDicTrussName = PickUp_TrussName(txtPath, language);

            mainListTrussName.Sort((a, b) =>
            {
                int indexA = orderDicTrussName.ContainsKey(Path.GetFileNameWithoutExtension(a)) ? orderDicTrussName[Path.GetFileNameWithoutExtension(a)] : int.MaxValue;
                int indexB = orderDicTrussName.ContainsKey(Path.GetFileNameWithoutExtension(b)) ? orderDicTrussName[Path.GetFileNameWithoutExtension(b)] : int.MaxValue;
                return indexA.CompareTo(indexB);
            }); //Order by Comp Review list

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode, elementNode;
            foreach (string Item in mainListTrussName)
            {
                extName = Path.GetExtension(Item);
                if (extName == ".tdlTruss")
                {
                    xmlDoc.Load(Item);
                    rootNode = xmlDoc.DocumentElement;
                    fileName = Path.GetFileNameWithoutExtension(Item);
                    dictTopPlate = tpi.Get_TopPlate_Info(txtPath, fileName, language, unit);
                    if (dictTopPlate != null)
                    {
                        foreach (KeyValuePair<int, Top_Plate_Info> TP in dictTopPlate)
                        {
                            j = 0;
                            Bearing_Enhancer bE = new Bearing_Enhancer();
                            bE.TopPlateInfo = TP.Value;
                            bE.TrussName = fileName;
                            //Get Data in < Script > Node
                            elementNode = rootNode.SelectSingleNode("//Script");
                            string scpt = elementNode.InnerText;
                            string[] Arr = scpt.Split('\n');
                            string[] S;
                            foreach (string I in Arr)
                            {
                                if (I.Contains("plys:"))
                                {
                                    S = I.Split(':');
                                    bE.Ply = S[1].Trim();

                                }
                                if (I.Contains("brg:"))
                                {
                                    if (j == TP.Key)
                                    {
                                        S = I.Split(' ');
                                        bE.TopPlateInfo.YLocation = S.SingleOrDefault(n => n == "BotChd" || n == "TopChd" || n == "Web" || n=="FillerChd" || n == "BrgBlk" || n == "TieBeam" || n == "Wedge");
                                    }
                                    j = j + 1;

                                }
                            }
                            //Check Wet Service Condition and Green Lumber condition
                            elementNode = rootNode.SelectSingleNode("//Settings");
                            XmlNodeList searchNodes = elementNode.ChildNodes;
                            foreach (XmlNode searchNode in searchNodes)
                            {
                                if (searchNode.Name == "OneTrussControlsWetServiceCondition" && searchNode.Attributes["Value"].Value == "true")
                                {
                                    bE.TopPlateInfo.WetService = true;
                                    //break;
                                }
                                if (searchNode.Name == "OneTrussControlsGreenLumber" && searchNode.Attributes["Value"].Value == "true")
                                {
                                    bE.TopPlateInfo.GreenLumber = true;
                                    //break;
                                }
                            }
                            //Get Data in <LumberResults> Node
                            var keyLumber = Get_Lumber(TP.Value.XLocation, TP.Value.YLocation, rootNode, unit);
                            LumberInventory lumI = new LumberInventory();
                            List<LumberInventory> list_lumI = lumI.Get_Lumber_Inv(projectPath);

                            if (keyLumber.key == "")
                            {
                                //Consider if necessary
                            }
                            else
                            {
                                foreach (LumberInventory LI in list_lumI)
                                {
                                    if (keyLumber.key == LI.Lumber_Key)
                                    {
                                        bE.LumSpecie = LI.Lumber_SpeciesName;
                                        bE.LumSize = LI.Lumber_Size;
                                        bE.LumWidth = LI.Lumber_Width;
                                        bE.LumThick = LI.Lumber_Thickness;
                                    }
                                }
                            }
                            //Check Interior or Extorior Bearing
                            double xloc = double.TryParse(TP.Value.XLocation, out double result) ? result / iom.miliFactor : Convert_To_Inch(TP.Value.XLocation);
                            double xleftend = double.Parse(keyLumber.x_leftend);
                            double xrightend = double.Parse(keyLumber.x_rightend);
                            if (xloc > xleftend + 8 && xloc < xrightend - 8)
                            {
                                bE.TopPlateInfo.Location_Type = "Interior";
                            }
                            else
                            {
                                bE.TopPlateInfo.Location_Type = "Exterior";
                            }

                            //Calculate Load Transfer load
                            double react = bE.TopPlateInfo.Reaction;
                            double bear_W = double.TryParse(bE.TopPlateInfo.BearingWidth, out double resultW) ? resultW : Convert_To_Inch(bE.TopPlateInfo.BearingWidth);
                            double bear_Wrq = double.TryParse(bE.TopPlateInfo.RequireWidth, out double resultR) ? resultR : Convert_To_Inch(bE.TopPlateInfo.RequireWidth);
                            bE.TopPlateInfo.LoadTransfer = Math.Round((react - react * bear_W / bear_Wrq), 1);

                            //Get Bearing Solution
                            List<string> bear_Solution = bE.Check_Bearing_Solution(bE.Ply, bE.LumSize, bE.LumSpecie, bE.TopPlateInfo, unit,language);
                            bE.BearingSolution = bear_Solution;
                            bearingEnhancerItems.Add(bE);
                        }
                    }
                }
            }

            return bearingEnhancerItems;
        }

        public List<string> Check_Bearing_Solution(string ply, string lumSize, string lumSpecie, Top_Plate_Info topPlate, string unit, string language, bool bVertical = false, string contactlength = "00")
        {
            List<string> list_Bearing_Solution = new List<string>();

            //Check Horizontal Block
            if (bVertical == false)
            {
                List<string> list_Horizontal_Block = Check_Horizontal_Block(ply, lumSize, lumSpecie, topPlate, unit, language);
                list_Bearing_Solution.AddRange(list_Horizontal_Block);

                //Check TBE and Crush Plate
                bool bCheck_TBECP = list_Horizontal_Block.Count != 0 ? Enum.GetValues(typeof(No_Solution_Enum)).Cast<No_Solution_Enum>().Any(s => s.GetDescription() == list_Horizontal_Block[0]) : false;
                if (bCheck_TBECP == false)
                {
                    list_Bearing_Solution.AddRange(Check_TBE(ply, lumSpecie, topPlate, unit));
                    list_Bearing_Solution.AddRange(Check_Crush_Plate(ply, lumSpecie, topPlate, unit));
                }
            }

            //Check Vertical Block
            if (bVertical == true)
            {
                List<string> list_Vertical_Block = Check_Vertical_Block(ply, lumSize, lumSpecie, topPlate, unit,language, contactlength);
                list_Bearing_Solution.AddRange(list_Vertical_Block);
            }

            if (list_Bearing_Solution.Count == 0)
            {
                list_Bearing_Solution.Add("Not-found-an-appropriate-solution");
            }
            return list_Bearing_Solution;
        }
        #endregion

        #region Support Methods:
        List<string> Check_Horizontal_Block(string ply, string lumSize, string lumSpecie, Top_Plate_Info topPlate, string unit, string language)
        {
            List<double> durationFactors = new List<double>();
            PropertyInfo[] props = typeof(Duration_Factor).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                var value = prop.GetValue(topPlate.DOL);
                bool bNumber = double.TryParse(value.ToString(), out double isNumber);
                if (bNumber)
                {
                    durationFactors.Add(isNumber);
                }
            }
            double durationFactor = durationFactors.Min();
            double wetserviceFactor = new WetServiceFactor(topPlate.WetService,topPlate.GreenLumber).Ksf;
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit,language);
            List<string> list_Horizontal_Block = new List<string>();
            int plies = int.Parse(ply);
            int No_Block = 0;
            double brgWidth = double.TryParse(topPlate.BearingWidth, out double resultB) ? resultB : Convert_To_Inch(topPlate.BearingWidth);
            double rqdWidth = double.TryParse(topPlate.RequireWidth, out double resultR) ? resultR : Convert_To_Inch(topPlate.RequireWidth);
            double rqdArea = 1.5 * iom.miliFactor * plies * rqdWidth;
            double brgArea = 1.5 * iom.miliFactor * plies * brgWidth;
            //Check lumber is null
            if (lumSize is null || lumSpecie is null)
            {
                list_Horizontal_Block.Add(No_Solution_Enum.PleaseCheckAndInputRelevantData.GetDescription());
                return list_Horizontal_Block;
            }
            //Check Number of Block
            if (topPlate.LoadTransfer < 0)
            {
                list_Horizontal_Block.Add(No_Solution_Enum.BearingEnhancerIsNotRequired.GetDescription());
                return list_Horizontal_Block;
            }
            else if ((topPlate.LoadTransfer / topPlate.Reaction) <= 0.05)
            {
                list_Horizontal_Block.Add(No_Solution_Enum.Within5Percent.GetDescription());
                return list_Horizontal_Block;
            }
            else if (rqdArea <= brgArea + 1.5 * iom.miliFactor * 1 * brgWidth)
            {
                if (plies > 3)
                {
                    No_Block = 2; //Alway use blocks on both faces for 4-ply
                }
                else
                {
                    No_Block = 1;
                }
            }
            else if (rqdArea <= brgArea + 1.5 * iom.miliFactor * 2 * brgWidth)
            {
                No_Block = 2;
            }
            else
            {
                No_Block = 3;
            }

            //Check Hor_Block Solution
            if (No_Block < 3)
            {
                switch (plies)
                {
                    case 1://ply = 1
                        if (No_Block == 1)
                        {
                            if (topPlate.Location_Type == "Exterior")//Exterior Bearing
                            {
                                List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                {
                                    (16,"Box10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed)
                                {
                                    Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDesignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col])*durationFactor*wetserviceFactor* iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDesignValue);
                                    if (topPlate.LoadTransfer / latDesignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                        list_Horizontal_Block.Add(suggestSolution);
                                    }
                                    else//Check Hor_Block on each face with nail if one face is not enough 
                                    {
                                        if (item.fastener.Contains("Nail"))
                                        {
                                            int No_Block2 = 2;
                                            Block_Info HBB2 = new Block_Info(plies, false, No_Block2, lumSize, item.length, item.fastener);
                                            if (topPlate.LoadTransfer / latDesignValue <= HBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block2}-Face-{numberFastener}-{item.fastener}";
                                                list_Horizontal_Block.Add(suggestSolution);
                                            }
                                        }
                                    }
                                }
                            }
                            else//Interior Bearing
                            {
                                List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                {
                                    (16,"Box10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"Box10dNail"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"Box10dNail"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed)
                                {
                                    Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                        list_Horizontal_Block.Add(suggestSolution);
                                    }
                                    else//Check Hor_Block on each face with nail if one face is not enough 
                                    {
                                        if (item.fastener.Contains("Nail"))
                                        {
                                            No_Block = 2;
                                            Block_Info HBB2 = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                            if (topPlate.LoadTransfer / latDeignValue <= HBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                                list_Horizontal_Block.Add(suggestSolution);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else//No_Block = 2
                        {
                            if (topPlate.Location_Type == "Exterior")//Exterior Bearing
                            {
                                List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                {
                                    (16,"Box10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed)
                                {
                                    Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                        list_Horizontal_Block.Add(suggestSolution);
                                    }
                                }
                            }
                            else//Interior Bearing
                            {
                                List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                {
                                    (16,"Box10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"Box10dNail"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"Box10dNail"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed)
                                {
                                    Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                        list_Horizontal_Block.Add(suggestSolution);
                                    }
                                }
                            }
                        }

                        break;
                    case 2://ply = 2
                        if (No_Block == 1)
                        {
                            if (topPlate.LoadTransfer < 1000 * iom.kNFactor)
                            {
                                if (topPlate.Location_Type == "Exterior")
                                {
                                    List<(int length, string fastener)> listFasUsed2 = new List<(int, string)>()
                                    {
                                        (16,"Box10dNail"),
                                        (16,"SDW22458"),
                                        (16,"SDS25412"),
                                        (18,"SDW22458"),
                                        (18,"SDS25412"),
                                        (24,"SDW22458"),
                                        (24,"SDS25412")
                                    };
                                    foreach ((int length, string fastener) item in listFasUsed2)
                                    {
                                        Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                        int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                        int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                        double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                        double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                        if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                        {
                                            string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                            list_Horizontal_Block.Add(suggestSolution);
                                        }
                                    }
                                }
                                else//Interior Bearing
                                {
                                    List<(int length, string fastener)> listFasUsed2 = new List<(int, string)>()
                                    {
                                        (16,"Box10dNail"),
                                        (16,"SDW22458"),
                                        (16,"SDS25412"),
                                        (18,"Box10dNail"),
                                        (18,"SDW22458"),
                                        (18,"SDS25412"),
                                        (24,"Box10dNail"),
                                        (24,"SDW22458"),
                                        (24,"SDS25412")
                                    };
                                    foreach ((int length, string fastener) item in listFasUsed2)
                                    {
                                        Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                        int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                        int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                        double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                        double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                        if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                        {
                                            string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                            list_Horizontal_Block.Add(suggestSolution);
                                        }
                                    }
                                }
                            }
                            else//Load Transfer > 1000 Lbs
                            {
                                if (topPlate.Location_Type == "Exterior")
                                {
                                    List<(int length, string fastener)> listFasUsed2 = new List<(int, string)>()
                                    {
                                        (16,"Box10dNail"),
                                        (16,"SDW22458"),
                                        (16,"SDS25412"),
                                        (18,"SDW22458"),
                                        (18,"SDS25412"),
                                        (24,"SDW22458"),
                                        (24,"SDS25412")
                                    };
                                    foreach ((int length, string fastener) item in listFasUsed2)
                                    {
                                        Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                        int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                        int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                        double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                        double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                        if (!item.fastener.Contains("Nail"))
                                        {
                                            if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                                list_Horizontal_Block.Add(suggestSolution);
                                            }
                                        }
                                        else//Check Hor_Block on each face with nail
                                        {
                                            int No_Block2 = 2;
                                            Block_Info HBB2 = new Block_Info(plies, false, No_Block2, lumSize, item.length, item.fastener);
                                            if (topPlate.LoadTransfer / latDeignValue <= HBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block2}-Face-{numberFastener}-{item.fastener}";
                                                list_Horizontal_Block.Add(suggestSolution);
                                            }
                                        }
                                    }
                                }
                                else//Interior Bearing
                                {
                                    List<(int length, string fastener)> listFasUsed2 = new List<(int, string)>()
                                    {
                                        (16,"Box10dNail"),
                                        (16,"SDW22458"),
                                        (16,"SDS25412"),
                                        (18,"Box10dNail"),
                                        (18,"SDW22458"),
                                        (18,"SDS25412"),
                                        (24,"Box10dNail"),
                                        (24,"SDW22458"),
                                        (24,"SDS25412")
                                    };
                                    foreach ((int length, string fastener) item in listFasUsed2)
                                    {
                                        Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                        int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                        int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                        double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                        double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                        if (!item.fastener.Contains("Nail"))
                                        {
                                            if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                                list_Horizontal_Block.Add(suggestSolution);
                                            }
                                        }
                                        else//Check Hor_Block on each face with nail
                                        {
                                            int No_Block2 = 2;
                                            Block_Info HBB2 = new Block_Info(plies, false, No_Block2, lumSize, item.length, item.fastener);
                                            if (topPlate.LoadTransfer / latDeignValue <= HBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block2}-Face-{numberFastener}-{item.fastener}";
                                                list_Horizontal_Block.Add(suggestSolution);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else//No_Block = 2
                        {
                            if (topPlate.Location_Type == "Exterior")
                            {
                                List<(int length, string fastener)> listFasUsed2 = new List<(int, string)>()
                                {
                                    (16,"Box10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed2)
                                {
                                    Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                        list_Horizontal_Block.Add(suggestSolution);
                                    }
                                }
                            }
                            else//Interior Bearing
                            {
                                List<(int length, string fastener)> listFasUsed2 = new List<(int, string)>()
                                {
                                    (16,"Box10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"Box10dNail"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"Box10dNail"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed2)
                                {
                                    Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                        list_Horizontal_Block.Add(suggestSolution);
                                    }
                                }
                            }
                        }
                        break;
                    case 3://ply = 3
                        List<(int length, string fastener)> listFasUsed3 = new List<(int, string)>()
                                    {
                                        (16,No_Block==1?"SDW22638":"SDW22458"),
                                        (16,No_Block==1?"SDS25600":"SDS25412"),
                                        (18,No_Block==1?"SDW22638":"SDW22458"),
                                        (18,No_Block==1?"SDS25600":"SDS25412"),
                                        (24,No_Block==1?"SDW22638":"SDW22458"),
                                        (24,No_Block==1?"SDS25600":"SDS25412")
                                    };
                        foreach ((int length, string fastener) item in listFasUsed3)
                        {
                            Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                            int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                            int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                            double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                            double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                            if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                            {
                                string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                list_Horizontal_Block.Add(suggestSolution);
                            }
                        }
                        break;
                    case 4://ply = 4
                        List<(int length, string fastener)> listFasUsed4 = new List<(int, string)>()
                                    {
                                        (16,"SDW22638"),
                                        (16,"SDS25600"),
                                        (18,"SDW22638"),
                                        (18,"SDS25600"),
                                        (24,"SDW22638"),
                                        (24,"SDS25600")
                                    };
                        foreach ((int length, string fastener) item in listFasUsed4)
                        {
                            Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                            int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                            int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                            double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                            double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                            if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                            {
                                string suggestSolution = $"{Math.Round(item.length * iom.miliFactor)}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                list_Horizontal_Block.Add(suggestSolution);
                            }
                        }
                        break;
                }
            }
            return list_Horizontal_Block;
        }

        List<string> Check_Vertical_Block(string ply, string lumSize, string lumSpecie, Top_Plate_Info topPlate, string unit,string language, string contactLength)
        {
            List<double> durationFactors = new List<double>();
            PropertyInfo[] props = typeof(Duration_Factor).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                var value = prop.GetValue(topPlate.DOL);
                bool bNumber = double.TryParse(value.ToString(), out double isNumber);
                if (bNumber)
                {
                    durationFactors.Add(isNumber);
                }
            }
            double durationFactor = durationFactors.Min();
            double wetserviceFactor = new WetServiceFactor(topPlate.WetService, topPlate.GreenLumber).Ksf;
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit, language);
            List<string> list_Vertical_Block = new List<string>();
            int plies = int.Parse(ply);
            int No_Block = 0;
            double brgWidth = double.TryParse(topPlate.BearingWidth, out double resultB) ? resultB : Convert_To_Inch(topPlate.BearingWidth);
            double rqdWidth = double.TryParse(topPlate.RequireWidth, out double resultR) ? resultR : Convert_To_Inch(topPlate.RequireWidth);
            double contactL = double.TryParse(contactLength,out double resultL) ? resultL : Convert_To_Inch(contactLength);
            double rqdArea = 1.5 * iom.miliFactor * plies * rqdWidth;
            double brgArea = 1.5 * iom.miliFactor * plies * brgWidth;

            //Check Number of Block
            if (topPlate.LoadTransfer < 0)
            {
                list_Vertical_Block.Add(No_Solution_Enum.BearingEnhancerIsNotRequired.GetDescription());
                return list_Vertical_Block;
            }
            else if ((topPlate.LoadTransfer / topPlate.Reaction) <= 0.05)
            {
                list_Vertical_Block.Add(No_Solution_Enum.Within5Percent.GetDescription());
                return list_Vertical_Block;
            }
            else if (rqdArea <= brgArea + 1.5 * iom.miliFactor * 1 * contactL)
            {
                if (plies > 3)
                {
                    No_Block = 2; //Alway use blocks on both faces for 4-ply
                }
                else
                {
                    No_Block = 1;
                }
            }
            else if (rqdArea <= brgArea + 1.5 * iom.miliFactor * 2 * contactL)
            {
                No_Block = 2;
            }
            else
            {
                No_Block = 3;
            }

            //Check Ver_Block Solution
            if (No_Block < 3)
            {
                switch (plies)
                {
                    case 1://ply = 1
                        if (No_Block == 1)
                        {
                            bool exit1 = false;
                            int l1 = 12;
                            List<string> listFasUsed1 = new List<string>()
                            {
                                "Box10dNail",
                                "SDW22300",
                                "SDS25300",
                            };
                            foreach (string item in listFasUsed1)
                            {
                                exit1 = false;
                                l1 = 12;
                                do
                                {
                                    Block_Info VBB = new Block_Info(plies, true, No_Block, lumSize, l1, item);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{Math.Round(l1 * iom.miliFactor)}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                        list_Vertical_Block.Add(suggestSolution);
                                        exit1 = true;
                                    }
                                    if (l1 >= 24 && exit1)//Check Ver_Block on each face with Nail
                                    {
                                        if (item.Contains("Nail"))
                                        {
                                            int No_Block2 = 2;
                                            for (int l2 = 12; l2<=l1-6; l2 += 6)
                                            {
                                                Block_Info VBB2 = new Block_Info(plies, true, No_Block2, lumSize, l2, item);
                                                if (topPlate.LoadTransfer / latDeignValue <= VBB2.MaxNumberFastener)
                                                {
                                                    string suggestSolution = $"{Math.Round(l2 * iom.miliFactor)}{iom.Text}-{(VBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block2}-Face-{numberFastener}-{item}";
                                                    list_Vertical_Block.Add(suggestSolution);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    l1 += 6;
                                } while (exit1 == false);
                            }
                        }
                        else//No_Block = 2
                        {
                            int l1 = 12;
                            bool exit2 = false;
                            List<string> listFasUsed1 = new List<string>()
                            {
                                ("Box10dNail"),
                                ("SDW22300"),
                                ("SDS25300"),
                            };
                            foreach (string item in listFasUsed1)
                            {
                                l1 = 12;
                                exit2 = false;
                                do
                                {
                                    Block_Info VBB = new Block_Info(plies, true, No_Block, lumSize, l1, item);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{Math.Round(l1 * iom.miliFactor)}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                        list_Vertical_Block.Add(suggestSolution);
                                        exit2 = true;
                                    }

                                    l1 += 6;
                                } while (exit2 == false);
                            }
                        }

                        break;
                    case 2://ply = 2
                        if (No_Block == 1)
                        {
                            int l2 = 12;
                            bool exit1 = false;
                            List<string> listFasUsed2 = new List<string>()
                            {
                                ("Box10dNail"),
                                ("SDW22458"),
                                ("SDS25412"),
                            };
                            foreach (string item in listFasUsed2)
                            {
                                l2 = 12;
                                exit1 = false;
                                do
                                {
                                    Block_Info VBB = new Block_Info(plies, true, No_Block, lumSize, l2, item);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer < 1000 * iom.kNFactor)
                                    {
                                        if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                        {
                                            string suggestSolution = $"{Math.Round(l2 * iom.miliFactor)}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                            list_Vertical_Block.Add(suggestSolution);
                                            exit1 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!item.Contains("Nail"))
                                        {
                                            if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{Math.Round(l2 * iom.miliFactor)}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                                list_Vertical_Block.Add(suggestSolution);
                                                exit1 = true;
                                            }
                                        }
                                        else
                                            exit1 = true;
                                    }

                                    if (l2 >= 24 && exit1)//Check Ver_Block on each face if one face is not enough
                                    {
                                        int No_Block2 = 2;
                                        for(int l22 = 12; l22 <= l2 - 6; l22 += 6)
                                        {
                                            if (item.Contains("Nail"))
                                            {
                                                Block_Info VBB2 = new Block_Info(plies, true, No_Block2, lumSize, l22, item);
                                                if (topPlate.LoadTransfer / latDeignValue <= VBB2.MaxNumberFastener)
                                                {
                                                    string suggestSolution = $"{Math.Round(l22 * iom.miliFactor)}{iom.Text}-{(VBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block2}-Face-{numberFastener}-{item}";
                                                    list_Vertical_Block.Add(suggestSolution);
                                                    break;
                                                }
                                            }
                                            if (item == "SDW22458")
                                            {
                                                Block_Info VBB2 = new Block_Info(plies, true, No_Block2, lumSize, l22, "SDW22300");
                                                if (topPlate.LoadTransfer / latDeignValue <= VBB2.MaxNumberFastener)
                                                {
                                                    string suggestSolution = $"{Math.Round(l22 * iom.miliFactor)}{iom.Text}-{(VBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block2}-Face-{numberFastener}-SDW22300";
                                                    list_Vertical_Block.Add(suggestSolution);
                                                    break;
                                                }
                                            }
                                            if (item == "SDS25412")
                                            {
                                                Block_Info VBB2 = new Block_Info(plies, true, No_Block2, lumSize, l22, "SDS25300");
                                                if (topPlate.LoadTransfer / latDeignValue <= VBB2.MaxNumberFastener)
                                                {
                                                    string suggestSolution = $"{Math.Round(l22 * iom.miliFactor)} {iom.Text}-{(VBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block2}-Face-{numberFastener}-SDS25300";
                                                    list_Vertical_Block.Add(suggestSolution);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    l2 += 6;
                                } while (exit1 == false);
                            }
                        }
                        else//No_Block = 2
                        {
                            int l2 = 12;
                            bool exit2 = false;
                            List<string> listFasUsed2 = new List<string>()
                            {
                                ("Box10dNail"),
                                ("SDW22300"),
                                ("SDS25300"),
                            };
                            foreach (string item in listFasUsed2)
                            {
                                l2 = 12;
                                exit2 = false;
                                do
                                {
                                    Block_Info VBB = new Block_Info(plies, true, No_Block, lumSize, l2, item);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{Math.Round(l2 * iom.miliFactor)}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                        list_Vertical_Block.Add(suggestSolution);
                                        exit2 = true;
                                    }
                                    l2 += 6;
                                } while (exit2 == false);
                            }
                        }
                        break;
                    case 3://ply = 3
                        int l3 = 12;
                        bool exit3 = false;
                        List<string> listFasUsed3 = new List<string>()
                            {
                                        (No_Block==1?"SDW22638":"SDW22458"),
                                        (No_Block==1?"SDS25600":"SDS25412"),
                                        
                            };
                        foreach (string item in listFasUsed3)
                        {
                            l3 = 12;
                            exit3 = false;
                            do
                            {
                                Block_Info VBB = new Block_Info(plies, true, No_Block, lumSize, l3, item);
                                int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item));
                                int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                {
                                    string suggestSolution = $"{Math.Round(l3 * iom.miliFactor)}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                    list_Vertical_Block.Add(suggestSolution);
                                    exit3 = true;
                                }

                                l3 += 6;
                            } while (exit3 == false);
                        }
                        break;
                    case 4://ply = 4
                        int l4 = 12;
                        bool exit4 = false;
                        List<string> listFasUsed4 = new List<string>()
                            {
                                        ("SDW22638"),
                                        ("SDS25600"),
                                        
                            };
                        foreach (string item in listFasUsed4)
                        {
                            l4 = 12;
                            exit4 = false;
                            do
                            {
                                Block_Info VBB = new Block_Info(plies, true, No_Block, lumSize, l4, item);
                                int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item));
                                int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * durationFactor * wetserviceFactor * iom.kNFactor;
                                double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                {
                                    string suggestSolution = $"{Math.Round(l4 * iom.miliFactor)}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                    list_Vertical_Block.Add(suggestSolution);
                                    exit4 = true;
                                }

                                l4 += 6;
                            } while (exit4 == false);
                        }
                        break;
                }
            }

            return list_Vertical_Block;
        }

        List<string> Check_TBE(string ply, string lumberSpecie, Top_Plate_Info topPlate, string unit)
        {
            List<double> durationFactors = new List<double>();
            PropertyInfo[] props = typeof(Duration_Factor).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                var value = prop.GetValue(topPlate.DOL);
                bool bNumber = double.TryParse(value.ToString(), out double isNumber);
                if (bNumber)
                {
                    durationFactors.Add(isNumber);
                }
            }
            double durationFactor = durationFactors.Min();
            List<string> list_TBE = new List<string>();
            if (durationFactor >= 1.0)
            {
                Imperial_Or_Metric iom = new Imperial_Or_Metric(unit);
                const double alternateFactor = 0.6;
                int plies = int.Parse(ply);
                double brgWidth = double.TryParse(topPlate.BearingWidth, out double resultB) ? resultB : Convert_To_Inch(topPlate.BearingWidth);
                double rqdWidth = double.TryParse(topPlate.RequireWidth, out double resultR) ? resultR : Convert_To_Inch(topPlate.RequireWidth);
                TBE_Info tbe_Data = new TBE_Info(unit);
                

                List<string[]> listMat = new List<string[]>()
                {
                new string[] {"HF","405"},
                new string[] {"SPF","425"},
                new string[] {"SP","565" },
                new string[] {"SYP","565" },
                new string[] {"DFL","625" },
                new string[] {"DFLN","625" },
                };
                int lumberFcp = int.Parse(listMat.FirstOrDefault(x => x[0] == lumberSpecie)[1]);
                int topPlateFcp = int.Parse(listMat.FirstOrDefault(x => x[0] == topPlate.Material)[1]);
                string tbeLumSpecie = lumberFcp < topPlateFcp ? lumberSpecie : topPlate.Material;

                if (brgWidth >= 3.5 * iom.miliFactor)
                {
                    if (tbeLumSpecie == "SPF")
                    {
                        double allowableValue = tbe_Data.TBE4_SPF[plies - 1, 1] * (brgWidth > 3.5 * iom.miliFactor ? alternateFactor : 1.0);
                        if (topPlate.LoadTransfer <= allowableValue)
                        {
                            list_TBE.Add("TBE4");
                        }
                    }
                    if (tbeLumSpecie == "DFL")
                    {
                        double allowableValue = tbe_Data.TBE4_DFL[plies - 1, 1] * (brgWidth > 3.5 * iom.miliFactor ? alternateFactor : 1.0);
                        if (topPlate.LoadTransfer <= allowableValue)
                        {
                            list_TBE.Add("TBE4");
                        }
                    }
                }
                if (brgWidth >= 5.5 * iom.miliFactor)
                {
                    if (tbeLumSpecie == "SPF")
                    {
                        double allowableValue = tbe_Data.TBE6_SPF[plies - 1, 1] * (brgWidth > 5.5 * iom.miliFactor ? alternateFactor : 1.0);
                        if (topPlate.LoadTransfer <= allowableValue)
                        {
                            list_TBE.Add("TBE6");
                        }
                    }
                    if (tbeLumSpecie == "DFL")
                    {
                        double allowableValue = tbe_Data.TBE6_DFL[plies - 1, 1] * (brgWidth > 5.5 * iom.miliFactor ? alternateFactor : 1.0);
                        if (topPlate.LoadTransfer <= allowableValue)
                        {
                            list_TBE.Add("TBE6");
                        }
                    }
                }
            }

            return list_TBE;
        }

        List<string> Check_Crush_Plate(string ply, string lumberSpecie, Top_Plate_Info topPlate, string unit)
        {
            List<double> durationFactors = new List<double>();
            PropertyInfo[] props = typeof(Duration_Factor).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                var value = prop.GetValue(topPlate.DOL);
                bool bNumber = double.TryParse(value.ToString(), out double isNumber);
                if (bNumber)
                {
                    durationFactors.Add(isNumber);
                }
            }
            double durationFactor = durationFactors.Min();
            List<string> list_CPn = new List<string>();
            if (durationFactor >= 1.0)
            {
                Imperial_Or_Metric iom = new Imperial_Or_Metric(unit);
                int plies = int.Parse(ply);
                double brgWidth = double.TryParse(topPlate.BearingWidth, out double resultB) ? resultB : Convert_To_Inch(topPlate.BearingWidth);
                double rqdWidth = double.TryParse(topPlate.RequireWidth, out double resultR) ? resultR : Convert_To_Inch(topPlate.RequireWidth);
                CrushPlates_Info cp_Data = new CrushPlates_Info(unit);
                CrushPlatesAndFlushPlate_Info cpf_Data = new CrushPlatesAndFlushPlate_Info(unit);

                List<string[]> listMat = new List<string[]>()
                {
                new string[] {"HF","405"},
                new string[] {"SPF","425"},
                new string[] {"SP","565" },
                new string[] {"SYP","565" },
                new string[] {"DFL","625" },
                new string[] {"DFLN","625" },
                };
                int lumberFcp = int.Parse(listMat.FirstOrDefault(x => x[0] == lumberSpecie)[1]);
                int topPlateFcp = int.Parse(listMat.FirstOrDefault(x => x[0] == topPlate.Material)[1]);
                string cpLumSpecie = lumberFcp < topPlateFcp ? lumberSpecie : topPlate.Material;

                if (brgWidth == 3.5 * iom.miliFactor)
                {
                    if (cpLumSpecie == "SPF")
                    {
                        double cp_allowableValue = cp_Data.CP4_DFL_SPF[plies - 1, 2];
                        double cpf_allowableValue = cpf_Data.CP4_DFL_SPF[plies - 1, 2];
                        if (topPlate.Reaction <= cp_allowableValue)
                        {
                            list_CPn.Add($"CP{plies}-4");
                        }
                        else if (topPlate.Reaction <= cpf_allowableValue)
                        {
                            list_CPn.Add($"CP{plies}-4 & FlushPlate");
                        }
                    }

                    if (cpLumSpecie == "DFL")
                    {
                        double cp_allowableValue = cp_Data.CP4_DFL_SPF[plies - 1, 1];
                        double cpf_allowableValue = cpf_Data.CP4_DFL_SPF[plies - 1, 1];
                        if (topPlate.Reaction <= cp_allowableValue)
                        {
                            list_CPn.Add($"CP{plies}-4");
                        }
                        else if (topPlate.Reaction <= cpf_allowableValue)
                        {
                            list_CPn.Add($"CP{plies}-4 & FlushPlate");
                        }
                    }
                }

                if (brgWidth == 5.5 * iom.miliFactor)
                {
                    if (cpLumSpecie == "SPF")
                    {
                        double cp_allowableValue = cp_Data.CP6_DFL_SPF[plies - 1, 2];
                        double cpf_allowableValue = cpf_Data.CP6_DFL_SPF[plies - 1, 2];
                        if (topPlate.Reaction <= cp_allowableValue)
                        {
                            list_CPn.Add($"CP{plies}-6");
                        }
                        else if (topPlate.Reaction <= cpf_allowableValue)
                        {
                            list_CPn.Add($"CP{plies}-6 & FlushPlate");
                        }
                    }

                    if (cpLumSpecie == "DFL")
                    {
                        double cp_allowableValue = cp_Data.CP6_DFL_SPF[plies - 1, 1];
                        double cpf_allowableValue = cpf_Data.CP6_DFL_SPF[plies - 1, 1];
                        if (topPlate.Reaction <= cp_allowableValue)
                        {
                            list_CPn.Add($"CP{plies}-6");
                        }
                        else if (topPlate.Reaction <= cpf_allowableValue)
                        {
                            list_CPn.Add($"CP{plies}-6 & FlushPlate");
                        }
                    }
                }
            }

            return list_CPn;
        }

        (string key, string x_leftend, string x_rightend) Get_Lumber(string x, string y, XmlNode rootNode, string unit)//Get keyLumber grade and lumber size at Xlocation of the bearing
        {
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit);
            int i = 0;
            string[] S, A;
            string s;
            string keyLumber = "";
            List<ArrayList> listArrList = new List<ArrayList>();
            XmlNode elementNode = rootNode.SelectSingleNode("//LumberResults");
            A = elementNode.InnerText.Split('\n');
            foreach (string I in A)
            {
                i++;
                if (y == "BotChd")
                {
                    if (!I.Contains("BC"))
                    {
                        continue;
                    }
                    ArrayList arrList = new ArrayList();
                    s = I.Trim('\r');
                    if (s.Contains("BC"))
                    {
                        S = s.Split(' ');

                        arrList.Add(i);
                        arrList.Add(S[0]);
                        arrList.Add(S[1]);
                    }
                    XmlNodeList nodeList = rootNode.SelectSingleNode("//Members").ChildNodes;
                    int j = 0;
                    foreach (XmlNode N in nodeList)
                    {
                        j++;
                        if (j == i)
                        {
                            string sl = N.Attributes["L"].Value;
                            sl = sl.Trim();
                            string[] ssl = sl.Split();
                            arrList.Add(ssl[0]);

                            string sr = N.Attributes["R"].Value;
                            sr = sr.Trim();
                            string[] ssr = sr.Split();
                            arrList.Add(ssr[ssr.Length - 3]);
                        }
                    }
                    listArrList.Add(arrList);
                }
                if (y == "TopChd")
                {
                    if (!I.Contains("TC"))
                    {
                        continue;
                    }
                    ArrayList arrList = new ArrayList();
                    s = I.Trim('\r');
                    if (s.Contains("TC"))
                    {
                        S = s.Split(' ');

                        arrList.Add(i);
                        arrList.Add(S[0]);
                        arrList.Add(S[1]);
                    }
                    XmlNodeList nodeList = rootNode.SelectSingleNode("//Members").ChildNodes;
                    int j = 0;
                    foreach (XmlNode N in nodeList)
                    {
                        j++;
                        if (j == i)
                        {
                            string sl = N.Attributes["L"].Value;
                            sl = sl.Trim();
                            string[] ssl = sl.Split();
                            arrList.Add(ssl[0]);

                            string sr = N.Attributes["R"].Value;
                            sr = sr.Trim();
                            string[] ssr = sr.Split();
                            arrList.Add(ssr[ssr.Length - 3]);
                        }
                    }
                    listArrList.Add(arrList);//{id, YLocation, Lumber ID, XLocation at the left of member, XLocation at the right of member}
                }

            }
            double xloc = double.TryParse(x, out double result) ? result / iom.miliFactor : Convert_To_Inch(x);

            foreach (ArrayList al in listArrList)
            {
                double rightEndLoc = Double.Parse(al[3].ToString());

                if (xloc <= rightEndLoc)
                {
                    keyLumber = al[2].ToString();
                    break;
                }
                else
                {
                    keyLumber = al[2].ToString();
                }
            }
            return listArrList.Count > 0 ? (keyLumber, listArrList[0][3].ToString(), listArrList[listArrList.Count - 1][4].ToString()) : ("", "0", "0");
        }

        double Convert_To_Inch(string xxx)
        {
            string s = xxx.Trim();
            string[] ss = s.Split('-');
            double q;
            if (ss.Length > 2)
            {
                q = double.Parse(ss[0]) * 12 + double.Parse(ss[1]) + double.Parse(ss[2]) / 16;
            }
            else if (ss.Length > 1)
            {
                q = double.Parse(ss[0]) + double.Parse(ss[1]) / 16;
            }
            else
            {
                q = double.Parse(ss[0]) / 16;
            }

            return q;
        }

        Dictionary<string, int> PickUp_TrussName(string txtPath, string language)
        {
            Dictionary<string, int> dictTrussName = new Dictionary<string, int>();
            List<string> listTrussName = new List<string>();
            string content = File.ReadAllText(txtPath);
            string[] arrFile = content.Split('\n');
            string[] arrLine = { };
            string trussName;
            English_Or_French langText = new English_Or_French(language);

            foreach (string line in arrFile)
            {
                if (line.Contains(langText.Truss) && line.Contains(langText.Qty))
                {
                    Array.Resize(ref arrLine, 0);
                    arrLine = line.Split(':');
                    trussName = arrLine[arrLine.Length - 1].Trim();
                    listTrussName.Add(trussName);
                }
            }
            listTrussName = listTrussName.Distinct().ToList();

            for (int i = 0; i < listTrussName.Count; i++)
            {
                dictTrussName.Add(listTrussName[i], i);
            }

            return dictTrussName;
        }
        #endregion
    }
    public class Bearing_Enhancer_CP : Bearing_Enhancer
    {
        public string Chosen_Solution { get; set; }
        public Bearing_Enhancer_CP (string chosensolution)
        {
            Chosen_Solution = chosensolution;
        }
        public override string Generate_Enhancer_Note(string chosensolution, string language, string unit = "Imperial")
        {
            string crushPlate = chosensolution.Split('&')[0].Trim();
            string theNote = "";

            if (language == "English")
            {
                theNote = $"Simpson {crushPlate} may be used as a bearing enhancer. See the latest Simpson Strong-Tie Construction Connectors catalog for installation instructions.";
            }
            else
            {
                theNote = $"Le renfort d'appuis {crushPlate} de Simpson peut être utilisé. Consultez le plus récent catalogue des connecteurs de Simpson Strong-Tie pour les instructions d’installation.";
            }
            return theNote;
        }
    }
    public class Bearing_Enhancer_TBE : Bearing_Enhancer
    {
        public string Chosen_Solution { get; set; }
        public Bearing_Enhancer_TBE(string chosensolution)
        {
            Chosen_Solution = chosensolution;
        }
        public override string Generate_Enhancer_Note(string chosensolution, string language, string unit = "Imperial")
        {
            string theNote = "";
            if (language == "English")
            {
                theNote = $"Simpson {chosensolution} may be used as a bearing enhancer. See the latest Simpson Strong - Tie Construction Connectors catalog for installation instructions.";
            }
            else
            {
                theNote = $"Le renfort d'appuis {chosensolution} de Simpson peut être utilisé. Consultez le plus récent catalogue des connecteurs de Simpson Strong-Tie pour les instructions d’installation.";
            }
            return theNote;
        }
    }
    public class Bearing_Enhancer_VerBlock : Bearing_Enhancer
    {
        public string Chosen_Solution { get; set; }
        public Bearing_Enhancer_VerBlock(string chosensolution)
        {
            Chosen_Solution = chosensolution;
        }
        public override string Generate_Enhancer_Note(string chosensolution, string language, string unit)
        {
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit,language);
            string[] arrKey = chosensolution.Split('-');
            int numberPly = int.Parse(Ply);
            double blockLength = Math.Round(int.Parse(arrKey[0].Replace(iom.Text, "").Trim()) * iom.miliFactor, 0);
            bool vertical = true;
            int numberBlock = int.Parse(arrKey[3]);
            int numberFastener = int.Parse(arrKey[5]);
            string fastenerType = arrKey[6];

            Block_Info Hor_Block = new Block_Info(numberPly, vertical, numberBlock, LumSize, blockLength, fastenerType);
            double row_Calculate = Math.Ceiling(numberFastener / ((blockLength - 2 * Hor_Block.EndDistance) / Hor_Block.MinSpacing + 1)/numberBlock);
            double row = row_Calculate <= Hor_Block.MinRow ? Hor_Block.MinRow : row_Calculate;
            string fasDescription = Enum.GetValues(typeof(eFastenerName)).Cast<eFastenerName>().FirstOrDefault(e => e.ToString() == fastenerType).GetDescription();

            List<string[]> LumberSizeUnit = new List<string[]>()
            {
                new string[]  { "2x4", "38x89" },
                new string[]  { "2x6", "38x140" },
                new string[]  { "2x8", "38x184" },
                new string[]  { "2x10", "38x235" },
                new string[]  { "2x12", "38x286" },
            };
            string lumSize = iom.Unit == "Imperial" ? LumSize : LumberSizeUnit.FirstOrDefault(e => e[0] == LumSize)[1];

            string theNote = "";
            if (language == "English")
            {
                if (fastenerType.Contains("Nail"))
                {
                    theNote = $"Attach bearing block BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (or better), to {(Hor_Block.NumberBlock == 1 ? "one face" : "both faces")} of the web #-# w/ {(row)} {(row > 1 ? "staggered rows" : "row")} of {fasDescription} @ {Hor_Block.MinSpacing}{iom.Text} o.c. {(row > 1 ? "Stagger rows by 1/2 the nails spacing." : ".")} Install a minimum of ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) nails{(Hor_Block.NumberBlock == 2 ? " per block." : ".")}";
                }
                else if (fastenerType.Contains("SDW"))
                {
                    theNote = $"Attach bearing block BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (or better), to {(Hor_Block.NumberBlock == 1 ? "one face" : "both faces")} of the web #-# w/ {(row)} {(row > 1 ? "staggered rows" : "row")} of Simpson {fasDescription} @ {Hor_Block.MinSpacing}{iom.Text} o.c. Install the screws per Simpson specifications. Install a minimum of ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) screws{(Hor_Block.NumberBlock == 2 ? " per block." : ".")}";
                }
                else if (fastenerType.Contains("SDS"))
                {
                    theNote = $"Attach bearing block BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (or better), to {(Hor_Block.NumberBlock == 1 ? "one face" : "both faces")} of the web #-# w/ ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) Simpson {fasDescription}{(Hor_Block.NumberBlock == 2 ? " per block." : ".")} See ICC-ES Report ESR-2236 for minimum spacing, edge distance, and end distance requirements for SDS screws.";
                }
                else { theNote = ""; }
            }
            else
            {
                if (fastenerType.Contains("Nail"))
                {
                    theNote = $"Attachez le renfort d'appui BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (ou mieux), sur {(Hor_Block.NumberBlock == 1 ? "une face" : "les deux faces")} de la l’âme avec {(row)} {(row > 1 ? "rangée décalées" : "rangée")} de {fasDescription} @ {Hor_Block.MinSpacing}{iom.Text} c.c. {(row > 1 ? "Le décalage des rangées doit être de 1/2 l'espacement." : ".")} Installez un min de ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) clous{(Hor_Block.NumberBlock == 2 ? " par bloc." : ".")}";
                }
                else if (fastenerType.Contains("SDW"))
                {
                    theNote = $"Attachez le renfort d'appui BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (ou mieux), sur {(Hor_Block.NumberBlock == 1 ? "une face" : "les deux faces")} de la l’âme avec {(row)} {(row > 1 ? "rangée décalées" : "rangée")} de vis {fasDescription} de Simpson@ {Hor_Block.MinSpacing}{iom.Text} c.c. Installez les vis selon les spécifications de Simpson. Installez un min. de ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) vis{(Hor_Block.NumberBlock == 2 ? " par bloc." : ".")}";
                }
                else if (fastenerType.Contains("SDS"))
                {
                    theNote = $"Attachez le renfort d'appui BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (ou mieux), sur {(Hor_Block.NumberBlock == 1 ? "une face" : "les deux faces")} de la l’âme avec ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) vis {fasDescription} de Simpson{(Hor_Block.NumberBlock == 2 ? " par bloc." : ".")} Consultez le rapport d'évaluation ESR-2236 pour les espacements min. d'extrémité et de rive pour les vis SDS.";
                }
                else { theNote = ""; }
            }
            return theNote;
        }
    }
    public class Bearing_Enhancer_HorBlock : Bearing_Enhancer
    {
        public string Chosen_Solution { get; set; }

        public Bearing_Enhancer_HorBlock(string chosensolution)
        {
            Chosen_Solution = chosensolution;
        }
        public override string Generate_Enhancer_Note(string chosensolution, string language, string unit)
        {
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit,language);
            string[] arrKey = chosensolution.Split('-');
            int numberPly = int.Parse(Ply);
            int blockLength = int.Parse(arrKey[0].Replace(iom.Text, "").Trim());
            bool vertical = false;
            int numberBlock = int.Parse(arrKey[3]);
            int numberFastener = int.Parse(arrKey[5]);
            string fastenerType = arrKey[6];

            Block_Info Hor_Block = new Block_Info(numberPly, vertical, numberBlock, LumSize, blockLength, fastenerType);
            double row_Calculate = Math.Ceiling(numberFastener / ((blockLength - 2 * Hor_Block.EndDistance) / Hor_Block.MinSpacing + 1)/numberBlock);
            double row = row_Calculate <= Hor_Block.MinRow ? Hor_Block.MinRow : row_Calculate;
            string fasDescription = Enum.GetValues(typeof(eFastenerName)).Cast<eFastenerName>().FirstOrDefault(e => e.ToString() == fastenerType).GetDescription();

            List<string[]> LumberSizeUnit = new List<string[]>()
            {
                new string[]  { "2x4", "38x89" },
                new string[]  { "2x6", "38x140" },
                new string[]  { "2x8", "38x184" },
                new string[]  { "2x10", "38x235" },
                new string[]  { "2x12", "38x286" },
            };
            string lumSize = iom.Unit == "Imperial" ? LumSize : LumberSizeUnit.FirstOrDefault(e => e[0] == LumSize)[1];

            string theNote = "";
            if (language == "English")
            {
                if (fastenerType.Contains("Nail"))
                {
                    theNote = $"Attach bearing block BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (or better), to {(Hor_Block.NumberBlock == 1 ? "one face" : "both faces")} of the bottom chord w/ {(row)} {(row>1 ? "staggered rows" : "row")} of {fasDescription} @ {Math.Round(Hor_Block.MinSpacing*iom.miliFactor)}{iom.Text} o.c. {(row>1? "Stagger rows by 1/2 the nails spacing." : ".")} Install a minimum of ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) nails{(Hor_Block.NumberBlock == 2 ? " per block." : ".")}";
                }
                else if (fastenerType.Contains("SDW"))
                {
                    theNote = $"Attach bearing block BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (or better), to {(Hor_Block.NumberBlock == 1 ? "one face" : "both faces")} of the bottom chord w/ {(row)} {(row > 1 ? "staggered rows" : "row")} of Simpson {fasDescription} @ {Math.Round(Hor_Block.MinSpacing * iom.miliFactor)}{iom.Text} o.c. Install the screws per Simpson specifications. Install a minimum of ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) screws{(Hor_Block.NumberBlock == 2 ? " per block." : ".")}";
                }
                else if (fastenerType.Contains("SDS"))
                {
                    theNote = $"Attach bearing block BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (or better), to {(Hor_Block.NumberBlock == 1 ? "one face" : "both faces")} of the bottom chord w/ ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) Simpson {fasDescription}{(Hor_Block.NumberBlock == 2 ? " per block." : ".")} See ICC-ES Report ESR-2236 for minimum spacing, edge distance, and end distance requirements for SDS screws.";
                }
                else { theNote = ""; }
            }
            else
            {
                if (fastenerType.Contains("Nail"))
                {
                    theNote = $"Attachez le renfort d'appui BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (ou mieux), sur {(Hor_Block.NumberBlock == 1 ? "une face" : "les deux faces")} de la MI avec {(row)} {(row>1 ? "rangée décalées" : "rangée")} de {fasDescription} @ {Math.Round(Hor_Block.MinSpacing * iom.miliFactor)}{iom.Text} c.c. {(row>1 ? "Le décalage des rangées doit être de 1/2 l'espacement." : ".")} Installez un min de ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) clous{(Hor_Block.NumberBlock == 2 ? " par bloc." : ".")}";
                }
                else if (fastenerType.Contains("SDW"))
                {
                    theNote = $"Attachez le renfort d'appui BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (ou mieux), sur {(Hor_Block.NumberBlock == 1 ? "une face" : "les deux faces")} de la MI avec {(row)} {(row > 1 ? "rangée décalées" : "rangée")} de vis {fasDescription} de Simpson@ {Math.Round(Hor_Block.MinSpacing * iom.miliFactor)}{iom.Text} c.c. Installez les vis selon les spécifications de Simpson. Installez un min. de ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) vis{(Hor_Block.NumberBlock == 2 ? " par bloc." : ".")}";
                }
                else if (fastenerType.Contains("SDS"))
                {
                    theNote = $"Attachez le renfort d'appui BB1, {lumSize}x{Hor_Block.BlockLength}{iom.Text} {LumSpecie} #2 (ou mieux), sur {(Hor_Block.NumberBlock == 1 ? "une face" : "les deux faces")} de la MI avec ({(Hor_Block.NumberBlock == 2 ? (Math.Ceiling(numberFastener * 1.0 / 2)) : numberFastener)}) vis {fasDescription} de Simpson{(Hor_Block.NumberBlock == 2 ? " par bloc." : ".")} Consultez le rapport d'évaluation ESR-2236 pour les espacements min. d'extrémité et de rive pour les vis SDS.";
                }
                else { theNote = ""; }
            }
            return theNote;
        }
    }
    public class Bearing_Enhancer_5Percent : Bearing_Enhancer
    {
        public string Chosen_Solution { get; set; }
        public Bearing_Enhancer_5Percent(string chosensolution)
        {
            Chosen_Solution = chosensolution;
        }
        public override string Generate_Enhancer_Note(string chosensolution, string language, string unit = "Imperial")
        {
            string theNote = "";
            if (language == "English")
            {
                theNote = $"Bearing factored reaction within 5% over Bearing Capacity. Building Designer may provide adequate bearing size or enhancement.";
            }
            else
            {
                theNote = $"Les réactions d’appuis pondérées sont moins de 5% au-dessus de la capacité des appuis. L'ingénieur concepteur peut fournir un détail de renfort d'appui approprié.";
            }
            return theNote;
        }
    }
    public class Bearing_Enhancer_BuildingDesigner : Bearing_Enhancer
    {
        public string Chosen_Solution { get; set; }
        public Bearing_Enhancer_BuildingDesigner(string chosensolution)
        {
            Chosen_Solution = chosensolution;
        }
        public override string Generate_Enhancer_Note(string chosensolution, string language, string unit = "Imperial")
        {
            string theNote = "";
            if (language == "English")
            {
                theNote = $"Building Designer to provide adequate bearing size or enhancement.";
            }
            else
            {
                theNote = $"L’ingénieur concepteur doit fournir un appui adéquat ou un détail de renfort.";
            }
            return theNote;
        }
    }
}
