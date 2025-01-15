using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Security.Cryptography.X509Certificates;

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
            List<string> subListTrussName = new List<string>();
            List<string> mainListTrussName = new List<string>();
            List<Bearing_Enhancer> bearingEnhancerItems = new List<Bearing_Enhancer>();
            List<Top_Plate_Info> listTopPlate = new List<Top_Plate_Info>();
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
                    listTopPlate = tpi.Get_TopPlate_Info(txtPath, fileName);
                    if (listTopPlate != null)
                    {
                        foreach(Top_Plate_Info TP in listTopPlate)
                        {
                            Bearing_Enhancer bE = new Bearing_Enhancer();
                            bE.TopPlateInfo = TP;
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
                                    break;
                                }
                                //if (I.Contains("brg:"))
                                //{
                                //    S = I.Split(':',' ') ;

                                //    //break;
                                //}
                            }
                            //Get Data in <LoadTemplate> Node
                            //elementNode = rootNode.SelectSingleNode("//LoadTemplate");
                            //string desc = elementNode.Attributes["Description"].Value;
                            //string loadtem = elementNode.InnerText;
                            //Arr = loadtem.Split('\n');
                            //foreach (string I in Arr)
                            //{

                            //    if (I.Contains("std") && !desc.Contains("No Wind"))
                            //    {
                            //        S = I.Split(' ');
                            //        bE.DOL = int.Parse(S[1]);
                            //        break;
                            //    }
                            //}

                            bearingEnhancerItems.Add(bE);
                        }
                    }
                }
            }
            
            return bearingEnhancerItems;
        }

        
    }

}
