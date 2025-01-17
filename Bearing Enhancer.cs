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
        public string TrussName { get; set; }
        public int Ply { get; set; }
        public string LumSpecie { get; set; }
        public string LumSize { get; set; }
        Top_Plate_Info TopPlateInfo { get; set; }


        public Bearing_Enhancer()
        {
            
        }

        public List<Bearing_Enhancer> Get_Bearing_Info(string txtpath)
        {
            //Get Data from tdlTruss file
            string txtPath = txtpath;
            string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(txtPath));
            string trussesPath = $"{projectPath}\\Trusses";
            string[] arrPath = trussesPath.Split('\\');
            string projectID = arrPath[arrPath.Length-1];
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
                    dictTopPlate = tpi.Get_TopPlate_Info(txtPath, fileName);
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
                                    bE.Ply = int.Parse(S[1]);
                                    //break;
                                }
                                if (I.Contains("brg:"))
                                {
                                    if (j == TP.Key)
                                    {
                                        S = I.Split(' ');
                                        bE.TopPlateInfo.YLocation = S[3];
                                    }
                                    j = j + 1;
                                    //break;
                                }
                            }
                            //Get Data in <LumberResults> Node
                            Get_Lumber(TP.Value.XLocation,TP.Value.YLocation,rootNode);

                            bearingEnhancerItems.Add(bE);
                        }
                    }
                }
            }
            
            return bearingEnhancerItems;
        }
        void Get_Lumber(string x, string y, XmlNode rootNode)
        {
            int i = 0;
            string[] S, A;
            string s;
            List<ArrayList> listArrList=new List<ArrayList>();
            XmlNode elementNode = rootNode.SelectSingleNode("//LumberResults");
            A = elementNode.InnerText.Split('\n');
            foreach (string I in A)
            {
                
                if (y== "BotChd")
                {
                    ArrayList arrList = new ArrayList();
                    s =I.Trim('\r');
                    if (s.Contains("TC"))
                    {
                        S = s.Split(' ');
                        
                        arrList.Add(i);
                        arrList.Add(S[0]);
                        arrList.Add(S[1]);
                    }
                    listArrList.Add(arrList);
                }
                //if (y == "BotChd")
                //{
                //    s = I.Trim('\r');
                //    S = s.Split(' ');


                //}
                
                i += 1;
            }
        }

    }

}
