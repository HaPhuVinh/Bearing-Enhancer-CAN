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

namespace Bearing_Enhancer_CAN
{
    public class Bearing_Enhancer
    {
        //public string Language { get; set; } = "";
        //public string Unit { get; set; } = "";
        public string TrussName { get; set; }
        public string Ply { get; set; }
        public string LumSpecie { get; set; }
        public string LumSize { get; set; }
        public string LumWidth { get; set; }
        public string LumThick { get; set; }
        public Top_Plate_Info TopPlateInfo { get; set; }
        public List<string> BearingSolution { get; set; }

        //public Bearing_Enhancer(string language, string unit)
        //{
        //    Language = language;
        //    Unit = unit;
        //}

        public Bearing_Enhancer()
        {
        }

        #region Service Method
        public virtual string Get_Enhancer_Note(string bearingsolution, string language)
        {
            return "";
        } 
        public List<Bearing_Enhancer> Get_Bearing_Info(string txtpath, string language, string unit)
        {
            //Get Data from tdlTruss file
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit);
            string txtPath = txtpath;
            string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(txtPath));
            string trussesPath = $"{projectPath}\\Trusses";
            string[] arrPath = trussesPath.Split('\\');
            string projectID = arrPath[arrPath.Length-2];
            string fileName = @"";
            string extName = @"";
            int j = 0;
            List<string> subListTrussName = new List<string>();
            List<string> mainListTrussName = new List<string>();
            List<Bearing_Enhancer> bearingEnhancerItems = new List<Bearing_Enhancer>();
            Dictionary<int,Top_Plate_Info> dictTopPlate = new Dictionary<int, Top_Plate_Info>();
            Top_Plate_Info tpi = new Top_Plate_Info();
            mainListTrussName = Directory.GetFiles(trussesPath).ToList();
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
                    dictTopPlate = tpi.Get_TopPlate_Info(txtPath, fileName,language,unit);
                    if (dictTopPlate != null)
                    {
                        foreach(KeyValuePair<int,Top_Plate_Info> TP in dictTopPlate)
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
                                        bE.TopPlateInfo.YLocation = S.SingleOrDefault(n => n == "BotChd" || n == "TopChd" || n == "Web");
                                    }
                                    j = j + 1;
                                    
                                }
                            }
                            //Get Data in <LumberResults> Node
                            var keyLumber = Get_Lumber(TP.Value.XLocation,TP.Value.YLocation,rootNode, unit);
                            LumberInventory lumI = new LumberInventory();
                            List<LumberInventory> list_lumI = lumI.Get_Lumber_Inv(projectID);

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
                            double xloc =double.TryParse(TP.Value.XLocation, out double result)?result/iom.miliFactor: Convert_To_Inch(TP.Value.XLocation);
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
                            double bear_W = double.TryParse(bE.TopPlateInfo.BearingWidth,out double resultW)?resultW:Convert_To_Inch(bE.TopPlateInfo.BearingWidth);
                            double bear_Wrq = double.TryParse(bE.TopPlateInfo.RequireWidth,out double resultR)?resultR:Convert_To_Inch(bE.TopPlateInfo.RequireWidth);
                            bE.TopPlateInfo.LoadTransfer = Math.Round((react - react * bear_W / bear_Wrq),0);

                            //Get Bearing Solution
                            List<string> bear_Solution = bE.Check_Bearing_Solution(bE.Ply, bE.LumSize, bE.LumSpecie, bE.TopPlateInfo, unit);
                            bE.BearingSolution = bear_Solution;
                            bearingEnhancerItems.Add(bE);
                        }
                    }
                }
            }
            
            return bearingEnhancerItems;
        }

        public List<string> Check_Bearing_Solution (string ply, string lumSize, string lumSpecie, Top_Plate_Info topPlate, string unit, bool bVertical = false, string contactlength = "0-00")
        {
            List<string> list_Bearing_Solution = new List<string>();

            //Check Horizontal Block
            if (bVertical == false)
            {
                List<string> list_Horizontal_Block = Check_Horizontal_Block(ply, lumSize, lumSpecie, topPlate, unit);
                list_Bearing_Solution.AddRange(list_Horizontal_Block);

                //Check TBE
                bool bCheckTBE = list_Horizontal_Block.Count != 0 ? Enum.GetValues(typeof(No_Solution_Enum)).Cast<No_Solution_Enum>().Any(s => s.GetDescription() == list_Horizontal_Block[0]) : false;
                if (bCheckTBE == false)
                {
                    list_Bearing_Solution.AddRange(Check_TBE(ply, topPlate, unit));
                }
            }
            //Check Vertical Block
            if (bVertical == true)
            {
                List<string> list_Vertical_Block = Check_Vertical_Block(ply, lumSize, lumSpecie, topPlate, unit, contactlength);
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
        List<string> Check_Horizontal_Block(string ply, string lumSize, string lumSpecie, Top_Plate_Info topPlate, string unit)
        {
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit);
            List<string> list_Horizontal_Block = new List<string>();
            int plies = int.Parse(ply);
            int No_Block = 0;
            double brgWidth = double.TryParse(topPlate.BearingWidth, out double resultB) ? resultB : Convert_To_Inch(topPlate.BearingWidth);
            double rqdWidth = double.TryParse(topPlate.RequireWidth, out double resultR) ? resultR : Convert_To_Inch(topPlate.RequireWidth);
            double rqdArea = 1.5*iom.miliFactor * plies * rqdWidth;
            double brgArea = 1.5*iom.miliFactor * plies * brgWidth;
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
            else if (rqdArea <= brgArea + 1.5*iom.miliFactor * 1 * brgWidth)
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
            else if (rqdArea <= brgArea + 1.5*iom.miliFactor * 2 * brgWidth)
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
                                    (16,"CommonWire10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed)
                                {
                                    Block_Info HBB = new Block_Info(plies,false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{item.length*iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
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
                                                string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
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
                                    (16,"CommonWire10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"CommonWire10dNail"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"CommonWire10dNail"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed)
                                {
                                    Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col])*iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
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
                                                string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
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
                                    (16,"CommonWire10dNail"),
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
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col])*iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                        list_Horizontal_Block.Add(suggestSolution);
                                    }
                                }
                            }
                            else//Interior Bearing
                            {
                                List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                {
                                    (16,"CommonWire10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"CommonWire10dNail"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"CommonWire10dNail"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed)
                                {
                                    Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
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
                                    List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                    {
                                        (16,"CommonWire10dNail"),
                                        (16,"SDW22458"),
                                        (16,"SDS25412"),
                                        (18,"SDW22458"),
                                        (18,"SDS25412"),
                                        (24,"SDW22458"),
                                        (24,"SDS25412")
                                    };
                                    foreach ((int length, string fastener) item in listFasUsed)
                                    {
                                        Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                        int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                        int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                        double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                        double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                        if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                        {
                                            string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                            list_Horizontal_Block.Add(suggestSolution);
                                        }
                                    }
                                }
                                else//Interior Bearing
                                {
                                    List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                    {
                                        (16,"CommonWire10dNail"),
                                        (16,"SDW22458"),
                                        (16,"SDS25412"),
                                        (18,"CommonWire10dNail"),
                                        (18,"SDW22458"),
                                        (18,"SDS25412"),
                                        (24,"CommonWire10dNail"),
                                        (24,"SDW22458"),
                                        (24,"SDS25412")
                                    };
                                    foreach ((int length, string fastener) item in listFasUsed)
                                    {
                                        Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                        int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                        int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                        double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                        double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                        if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                        {
                                            string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                            list_Horizontal_Block.Add(suggestSolution);
                                        }
                                    }
                                }
                            }
                            else//Load Transfer > 1000 Lbs
                            {
                                if (topPlate.Location_Type == "Exterior")
                                {
                                    List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                    {
                                        (16,"CommonWire10dNail"),
                                        (16,"SDW22458"),
                                        (16,"SDS25412"),
                                        (18,"SDW22458"),
                                        (18,"SDS25412"),
                                        (24,"SDW22458"),
                                        (24,"SDS25412")
                                    };
                                    foreach ((int length, string fastener) item in listFasUsed)
                                    {
                                        Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                        int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                        int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                        double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                        double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                        if (!item.fastener.Contains("Nail"))
                                        {
                                            if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                                list_Horizontal_Block.Add(suggestSolution);
                                            }
                                        }
                                        else//Check Hor_Block on each face with nail
                                        {
                                            int No_Block2 = 2;
                                            Block_Info HBB2 = new Block_Info(plies, false, No_Block2, lumSize, item.length, item.fastener);
                                            if (topPlate.LoadTransfer / latDeignValue <= HBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                                list_Horizontal_Block.Add(suggestSolution);
                                            }
                                        }
                                    }
                                }
                                else//Interior Bearing
                                {
                                    List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                    {
                                        (16,"CommonWire10dNail"),
                                        (16,"SDW22300"),
                                        (16,"SDS25300"),
                                        (18,"CommonWire10dNail"),
                                        (18,"SDW22300"),
                                        (18,"SDS25300"),
                                        (24,"CommonWire10dNail"),
                                        (24,"SDW22300"),
                                        (24,"SDS25300")
                                    };
                                    foreach ((int length, string fastener) item in listFasUsed)
                                    {
                                        Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                        int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                        int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                        double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                        double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                        if (!item.fastener.Contains("Nail"))
                                        {
                                            if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                                list_Horizontal_Block.Add(suggestSolution);
                                            }
                                        }
                                        else//Check Hor_Block on each face with nail
                                        {
                                            int No_Block2 = 2;
                                            Block_Info HBB2 = new Block_Info(plies, false, No_Block2, lumSize, item.length, item.fastener);
                                            if (topPlate.LoadTransfer / latDeignValue <= HBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
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
                                List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                {
                                    (16,"CommonWire10dNail"),
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
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                        list_Horizontal_Block.Add(suggestSolution);
                                    }
                                }
                            }
                            else//Interior Bearing
                            {
                                List<(int length, string fastener)> listFasUsed = new List<(int, string)>()
                                {
                                    (16,"CommonWire10dNail"),
                                    (16,"SDW22300"),
                                    (16,"SDS25300"),
                                    (18,"CommonWire10dNail"),
                                    (18,"SDW22300"),
                                    (18,"SDS25300"),
                                    (24,"CommonWire10dNail"),
                                    (24,"SDW22300"),
                                    (24,"SDS25300")
                                };
                                foreach ((int length, string fastener) item in listFasUsed)
                                {
                                    Block_Info HBB = new Block_Info(plies, false, No_Block, lumSize, item.length, item.fastener);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item.fastener));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
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
                            double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                            double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                            if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                            {
                                string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
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
                            double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                            double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                            if (topPlate.LoadTransfer / latDeignValue <= HBB.MaxNumberFastener)
                            {
                                string suggestSolution = $"{item.length * iom.miliFactor}{iom.Text}-{(HBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item.fastener}";
                                list_Horizontal_Block.Add(suggestSolution);
                            }
                        }
                        break;
                }
            }
            return list_Horizontal_Block;
        }

        List<string> Check_Vertical_Block(string ply, string lumSize, string lumSpecie, Top_Plate_Info topPlate, string unit, string contactLength)
        {
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit);
            List<string> list_Vertical_Block = new List<string>();
            int plies = int.Parse(ply);
            int No_Block = 0;
            double brgWidth = double.TryParse(topPlate.BearingWidth, out double resultB) ? resultB : Convert_To_Inch(topPlate.BearingWidth);
            double rqdWidth = double.TryParse(topPlate.RequireWidth, out double resultR) ? resultR : Convert_To_Inch(topPlate.RequireWidth);
            double contactL = double.TryParse(contactLength, out double resultC) ? resultC : Convert_To_Inch(contactLength);
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
                                "CommonWire10dNail",
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
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{l1 * iom.miliFactor}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                        list_Vertical_Block.Add(suggestSolution);
                                        exit1 = true;
                                    }
                                    if(l1>24)//Check Ver_Block on each face with Nail
                                    {
                                        if (item.Contains("Nail"))
                                        {
                                            No_Block = 2;
                                            Block_Info VBB2 = new Block_Info(plies, false, No_Block, lumSize, l1, item);
                                            if (topPlate.LoadTransfer / latDeignValue <= VBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{l1 * iom.miliFactor}{iom.Text}-{(VBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                                list_Vertical_Block.Add(suggestSolution);
                                            }
                                        }
                                    }
                                    l1 += 6;
                                } while (exit1==false);
                            }
                        }
                        else//No_Block = 2
                        {
                            int l1 = 12;
                            bool exit1 = false;
                            List<string> listFasUsed1 = new List<string>()
                            {
                                ("CommonWire10dNail"),
                                ("SDW22300"),
                                ("SDS25300"),
                            };
                            foreach (string item in listFasUsed1)
                            {
                                l1 = 12;
                                exit1 = false;
                                do
                                {
                                    Block_Info VBB = new Block_Info(plies, true, No_Block, lumSize, l1, item);
                                    int row = Fastener_Design_Value.Lateral_Design_Value.FindIndex(n => n.Contains(item));
                                    int col = Array.IndexOf(Fastener_Design_Value.Lateral_Design_Value[0], lumSpecie);
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{l1 * iom.miliFactor}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                        list_Vertical_Block.Add(suggestSolution);
                                        exit1 = true;
                                    }
                                    
                                    l1 += 6;
                                } while (exit1==false);
                            }
                        }

                        break;
                    case 2://ply = 2
                        if (No_Block == 1)
                        {
                            int l2 = 12;
                            bool exit2 = false;
                            List<string> listFasUsed2 = new List<string>()
                            {
                                ("CommonWire10dNail"),
                                ("SDW22458"),
                                ("SDS25412"),
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
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{l2 * iom.miliFactor}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                        list_Vertical_Block.Add(suggestSolution);
                                        exit2 = true;
                                    }
                                    if (l2 > 24)//Check Hor_Block on each face with nail if one face is not enough
                                    {
                                        if (item.Contains("Nail"))
                                        {
                                            No_Block = 2;
                                            Block_Info VBB2 = new Block_Info(plies, false, No_Block, lumSize, l2, item);
                                            if (topPlate.LoadTransfer / latDeignValue <= VBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{l2 * iom.miliFactor}{iom.Text}-{(VBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                                list_Vertical_Block.Add(suggestSolution);
                                            }
                                        }
                                        if (item == "SDW22458")
                                        {
                                            No_Block = 2;
                                            Block_Info VBB2 = new Block_Info(plies, false, No_Block, lumSize, l2, "SDW22300");
                                            if (topPlate.LoadTransfer / latDeignValue <= VBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{l2 * iom.miliFactor}{iom.Text}-{(VBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                                list_Vertical_Block.Add(suggestSolution);
                                            }
                                        }
                                        if (item == "SDS25412")
                                        {
                                            No_Block = 2;
                                            Block_Info VBB2 = new Block_Info(plies, false, No_Block, lumSize, l2, "SDS25300");
                                            if (topPlate.LoadTransfer / latDeignValue <= VBB2.MaxNumberFastener)
                                            {
                                                string suggestSolution = $"{l2 * iom.miliFactor} {iom.Text}-{(VBB2.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
                                                list_Vertical_Block.Add(suggestSolution);
                                            }
                                        }
                                    }
                                    l2 += 6;
                                } while (exit2==false);
                            }
                        }
                        else//No_Block = 2
                        {
                            int l2 = 12;
                            bool exit2 = false;
                            List<string> listFasUsed2 = new List<string>()
                            {
                                ("CommonWire10dNail"),
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
                                    double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                    double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                    if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                    {
                                        string suggestSolution = $"{l2 * iom.miliFactor}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
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
                                        (No_Block==1?"SDW22638":"SDW22458"),
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
                                double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                {
                                    string suggestSolution = $"{l3 * iom.miliFactor}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
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
                                        ("SDW22638"),
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
                                double latDeignValue = double.Parse(Fastener_Design_Value.Lateral_Design_Value[row][col]) * iom.kNFactor;
                                double numberFastener = Math.Ceiling(topPlate.LoadTransfer / latDeignValue);
                                if (topPlate.LoadTransfer / latDeignValue <= VBB.MaxNumberFastener)
                                {
                                    string suggestSolution = $"{l4 * iom.miliFactor}{iom.Text}-{(VBB.Vertical == false ? "Hor-Block" : "Ver-Block")}-{No_Block}-Face-{numberFastener}-{item}";
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

        List<string> Check_TBE(string ply, Top_Plate_Info topPlate, string unit)//Need to consider Top Plate Species and Lumber Species
        {
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit);
            const double alternateFactor = 0.6;
            int plies = int.Parse(ply);
            double brgWidth = double.TryParse(topPlate.BearingWidth, out double resultB)? resultB:Convert_To_Inch(topPlate.BearingWidth);
            double rqdWidth = double.TryParse(topPlate.RequireWidth, out double resultR)?resultR:Convert_To_Inch(topPlate.RequireWidth);
            TBE_Info tbe_Data = new TBE_Info(unit);
            List<string> list_TBE = new List<string>();
            //Check lumber is null
            //if (lumSize is null || lumSpecie is null)
            //{
            //    list_TBE.Add(No_Solution_Enum.Please_check_and_input_relevant_data.ToString());
            //    return list_TBE;
            //}
            if (brgWidth >= 3.5*iom.miliFactor)
            {
                if (topPlate.Material == "SPF")
                {
                    double allowableValue = tbe_Data.TBE4_SPF[plies - 1, 1] * (brgWidth > 3.5* iom.miliFactor ? alternateFactor : 1.0);
                    if (topPlate.LoadTransfer <= allowableValue)
                    {
                        list_TBE.Add("TBE4");
                    }
                }
                if (topPlate.Material == "DFL")
                {
                    double allowableValue = tbe_Data.TBE4_DFL[plies - 1, 1] * (brgWidth > 3.5 * iom.miliFactor ? alternateFactor : 1.0);
                    if (topPlate.LoadTransfer <= allowableValue)
                    {
                        list_TBE.Add("TBE4");
                    }
                }
            }
            if (brgWidth >= 5.5* iom.miliFactor)
            {
                if (topPlate.Material == "SPF")
                {
                    double allowableValue = tbe_Data.TBE6_SPF[plies - 1, 1] * (brgWidth > 5.5 * iom.miliFactor ? alternateFactor : 1.0);
                    if (topPlate.LoadTransfer <= allowableValue)
                    {
                        list_TBE.Add("TBE6");
                    }
                }
                if (topPlate.Material == "DFL")
                {
                    double allowableValue = tbe_Data.TBE6_DFL[plies - 1, 1] * (brgWidth > 5.5 * iom.miliFactor ? alternateFactor : 1.0);
                    if (topPlate.LoadTransfer <= allowableValue)
                    {
                        list_TBE.Add("TBE6");
                    }
                }
            }
            
            return list_TBE;
        }

        (string key, string x_leftend, string x_rightend) Get_Lumber(string x, string y, XmlNode rootNode, string unit)//Get keyLumber grade and lumber size at Xlocation of the bearing
        {
            Imperial_Or_Metric iom = new Imperial_Or_Metric(unit);
            int i = 0;
            string[] S, A;
            string s;
            string keyLumber="";
            List<ArrayList> listArrList=new List<ArrayList>();
            XmlNode elementNode = rootNode.SelectSingleNode("//LumberResults");
            A = elementNode.InnerText.Split('\n');
            foreach (string I in A)
            {
                i ++;
                if (y == "BotChd")
                {
                    if (!I.Contains("BC"))
                    {
                        continue;
                    }
                    ArrayList arrList = new ArrayList();
                    s =I.Trim('\r');
                    if (s.Contains("BC"))
                    {
                        S = s.Split(' ');
                        
                        arrList.Add(i);
                        arrList.Add(S[0]);
                        arrList.Add(S[1]);
                    }
                    XmlNodeList nodeList = rootNode.SelectSingleNode("//Members").ChildNodes;
                    int j = 0;
                    foreach(XmlNode N in nodeList)
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
                            arrList.Add(ssr[ssr.Length-3]);
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
            double xloc = double.TryParse(x,out double result)?result/iom.miliFactor:Convert_To_Inch(x);

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
            return listArrList.Count>0?(keyLumber, listArrList[0][3].ToString(), listArrList[listArrList.Count-1][4].ToString()):("","0","0");
        }

        double Convert_To_Inch(string xxx)
        {
            string s = xxx.Trim();
            string[] ss = s.Split('-');
            double q;
            if (ss.Length>2)
            {
                q = double.Parse(ss[0]) * 12 + double.Parse(ss[1]) + double.Parse(ss[2]) / 16;
            }
            else if(ss.Length>1)
            {
                q = double.Parse(ss[0]) + double.Parse(ss[1]) / 16;
            }
            else
            {
                q=double.Parse(ss[0]) / 16;
            }
            
            return q;
        }
        #endregion
    }

    public class Bearing_Enhancer_TBE : Bearing_Enhancer
    {
        public string Chosen_Solution { get; set; }

        public override string Get_Enhancer_Note(string bearingsolution, string language)
        {
            return "";
        }

    }

    public class Bearing_Enhancer_VerBlock : Bearing_Enhancer
    {
        public string Chosen_Solution { get; set; }
        public Block_Info Ver_Block { get; set; }
        public override string Get_Enhancer_Note(string bearingsolution, string language)
        {
            return "";
        }
    }
    public class Bearing_Enhancer_HorBlock : Bearing_Enhancer
    {
        public string Chosen_Solution { get; set; }

        public Bearing_Enhancer_HorBlock(string chosensolution)
        {
            Chosen_Solution = chosensolution;
        }
        public override string Get_Enhancer_Note(string chosensolution, string language)
        {
            string[] arrKey = chosensolution.Split();
            int numberPly = int.Parse(Ply);
            int blockLength = int.Parse(arrKey[0].Replace("in.", "").Trim());
            bool vertical = false;
            int numberBlock = int.Parse(arrKey[3]);
            int numberFastener = int.Parse(arrKey[5]);
            string fastenerType = arrKey[6];
            
            Block_Info Hor_Block = new Block_Info(numberPly,vertical,numberBlock,LumSize,blockLength,fastenerType);
            double row = Math.Ceiling(numberFastener/((blockLength-2*Hor_Block.EndDistance)/Hor_Block.MinSpacing+1));
            string fasDescription = Enum.GetValues(typeof(eFastenerName)).Cast<eFastenerName>().FirstOrDefault(e=>e.ToString()==fastenerType).GetDescription();
            string theNote = "";
            if(language == "English")
            {
                if (fastenerType.Contains("Nail"))
                {
                    theNote = $"Attach bearing block BB1, {LumSize}x{Hor_Block.BlockLength}\" {LumSpecie} #2 (or better), to {(Hor_Block.NumberBlock == 1 ? "one" : "both")} face(s) of the bottom chord w/ {(row <= Hor_Block.MinRow ? Hor_Block.MinRow : row)} (staggered) row(s) of {fasDescription} @ {Hor_Block.MinSpacing}\" o.c. (Stagger rows by 1/2 the nails spacing). Install a minimum of {numberFastener} nails.";
                }
                else if (fastenerType.Contains("SDW"))
                {

                }
                else if (fastenerType.Contains("SDS"))
                {

                }
                else { theNote = ""; }
            }
            else
            {
                
            }

            return theNote;
        }
    }
}
