using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Collections;

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
        public virtual void Get_Enhancer_Note()
        {

        } 
        public List<Bearing_Enhancer> Get_Bearing_Info(string txtpath, string language, string unit)
        {
            //Get Data from tdlTruss file
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
                            string keyLumber = Get_Lumber(TP.Value.XLocation,TP.Value.YLocation,rootNode);
                            LumberInventory lumI = new LumberInventory();
                            List<LumberInventory> list_lumI = lumI.Get_Lumber_Inv(projectID);

                            if (keyLumber == "")
                            {
                                //Consider if necessary
                            }
                            else
                            {
                                foreach (LumberInventory LI in list_lumI)
                                {
                                    if (keyLumber == LI.Lumber_Key)
                                    {
                                        bE.LumSpecie = LI.Lumber_SpeciesName;
                                        bE.LumSize = LI.Lumber_Size;
                                        bE.LumWidth = LI.Lumber_Width;
                                        bE.LumThick = LI.Lumber_Thickness;
                                    }
                                }
                            }

                            //Calculate Load Transfer load
                            double react = bE.TopPlateInfo.Reaction;
                            double bear_W = Convert_To_Inch(bE.TopPlateInfo.BearingWidth);
                            double bear_Wrq = Convert_To_Inch(bE.TopPlateInfo.RequireWidth);
                            bE.TopPlateInfo.LoadTransfer = Math.Round((react - react * bear_W / bear_Wrq),0);

                            //Get Bearing Solution
                            List<string> bear_Solution = bE.Check_Bearing_Solution(bE.Ply, bE.TopPlateInfo, unit);
                            bE.BearingSolution = bear_Solution;
                            bearingEnhancerItems.Add(bE);
                        }
                    }
                }
            }
            
            return bearingEnhancerItems;
        }

        public List<string> Check_Bearing_Solution (string ply, Top_Plate_Info topPlate, string unit)
        {
            List<string> list_BearingSolution = new List<string>();
            const double alternateFactor = 0.6;
            int plies = int.Parse(ply);
            int No_Block = 0;
            double brgWidth = Convert_To_Inch(topPlate.BearingWidth);
            double rqdWidth = Convert_To_Inch(topPlate.RequireWidth);
            double rqdArea = 1.5 * plies * rqdWidth;
            double brgArea = 1.5 * plies * brgWidth;

            //Check Number of Block
            if (topPlate.LoadTransfer < 0)
            {
                list_BearingSolution.Add("Enhancer not required");
                return list_BearingSolution;
            }
            else if ((topPlate.LoadTransfer / topPlate.Reaction) <= 0.05)
            {
                list_BearingSolution.Add("Within 5%");
                return list_BearingSolution;
            }
            else if (rqdArea <= brgArea + 1.5 * 1 * brgWidth)
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
            else if (rqdArea <= brgArea + 1.5 * 2 * brgWidth)
            {
                No_Block = 2;
            }
            else
            {
                No_Block = 3;
            }
            list_BearingSolution.Add("Number of Block: "+No_Block);
            //Check TBE
            TBE_Info tbe_Data = new TBE_Info(unit);
            if(brgWidth >= 3.5)
            {
                if(topPlate.Material == "SPF")
                {
                    double allowableValue = tbe_Data.TBE4_SPF[plies - 1, 1] * (brgWidth>3.5 ? alternateFactor : 1.0);
                    if (topPlate.LoadTransfer <= allowableValue)
                    {
                        list_BearingSolution.Add("TBE4");
                    }
                }
                if(topPlate.Material == "DFL")
                {
                    double allowableValue = tbe_Data.TBE4_DFL[plies - 1, 1] * (brgWidth > 3.5 ? alternateFactor : 1.0);
                    if (topPlate.LoadTransfer <= allowableValue)
                    {
                        list_BearingSolution.Add("TBE4");
                    }
                }
            }
            if (brgWidth >= 5.5)
            {
                if (topPlate.Material == "SPF")
                {
                    double allowableValue = tbe_Data.TBE6_SPF[plies - 1, 1] * (brgWidth > 5.5 ? alternateFactor : 1.0);
                    if (topPlate.LoadTransfer <= allowableValue)
                    {
                        list_BearingSolution.Add("TBE6");
                    }
                }
                if (topPlate.Material == "DFL")
                {
                    double allowableValue = tbe_Data.TBE6_DFL[plies - 1, 1] * (brgWidth > 5.5 ? alternateFactor : 1.0);
                    if (topPlate.LoadTransfer <= allowableValue)
                    {
                        list_BearingSolution.Add("TBE6");
                    }
                }
            }

            return list_BearingSolution; 
        }
        #endregion

        #region Support Methods:
        string Get_Lumber(string x, string y, XmlNode rootNode)//Get keyLumber grade and lumber size at Xlocation of the bearing
        {
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
                            string ss = N.Attributes["L"].Value;
                            ss = ss.Trim();
                            string[] sss = ss.Split();
                            arrList.Add(sss[0]);
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
                            string ss = N.Attributes["L"].Value;
                            ss = ss.Trim();
                            string[] sss = ss.Split();
                            arrList.Add(sss[0]);
                        }
                    }
                    listArrList.Add(arrList);//{id, YLocation, Lumber ID, XLocation at the right end}
                }
                
            }
            double xloc = Convert_To_Inch(x);
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
            return keyLumber;
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
        public TBE_Info TBE_Info { get; set; }

        public override void Get_Enhancer_Note()
        {
            base.Get_Enhancer_Note();
        }
    }
}
