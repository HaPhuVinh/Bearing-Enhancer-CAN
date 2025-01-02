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
        public string TOPSpecie { get; set; }
        public double DOL { get; set; }
        public string LumSpecie { get; set; }
        public string LumSize { get; set; }

        public Bearing_Enhancer()
        {
            
        }

        public List<string> Get_BE_Info(string PJN)
        {
            string projectNumber = PJN;
            string path = @"";
            string fileName = @"";
            string extName = @"";
            List<string> subListName = new List<string>();
            List<string> mainListName = new List<string>();
            List<Bearing_Enhancer> bearingEnhancers = new List<Bearing_Enhancer>();
            path = "C:\\SST-EA\\Client\\Projects\\" + projectNumber+"\\Trusses";
            mainListName = Directory.GetFiles(path).ToList();
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode, elementNode;
            foreach (string Item in mainListName)
            { 
                Bearing_Enhancer bE = new Bearing_Enhancer();
                extName = Path.GetExtension(Item);
                if (extName == ".tdlTruss")
                {
                    fileName = Path.GetFileNameWithoutExtension(Item);
                    subListName.Add(fileName);

                    //Get Data in tdlTruss file
                    xmlDoc.Load(Item);
                    rootNode = xmlDoc.DocumentElement;
                    elementNode = rootNode.SelectSingleNode("//Script");
                    string scpt = elementNode.InnerText.Trim();
                    string[] arr_scpt = scpt.Split('\n');

                    bE.TrussName = fileName;
                    foreach(string I in arr_scpt)
                    {
                        string[] S;
                        if (I.Contains("plys"))
                        {
                            S = I.Split(':');
                            bE.Ply = int.Parse(S[1]);
                        }
                        break;
                    }
                }
            }
            
            return subListName;
        }

        public List<LumberInventory> Get_Lumber_Inv(string PJN)
        {
            List<LumberInventory> lumber_inv = new List<LumberInventory>();

            string projectNumber = PJN;
            string path = "C:\\SST-EA\\Client\\Projects\\" + projectNumber + "\\Presets\\TrussStudio\\LumberInventory.xml";
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode, elementNode;

            xmlDoc.Load(path);
            rootNode = xmlDoc.DocumentElement;
            elementNode = rootNode.SelectSingleNode("//LumberMaterialList");
            XmlNodeList searchNodes = elementNode.ChildNodes;

            foreach (XmlNode searchNode in searchNodes)
            {
                LumberInventory lumber = new LumberInventory();
                lumber.Lumber_Key = searchNode.Attributes["Key"].Value;
                lumber.Lumber_Name = searchNode.Attributes["Name"].Value;
                lumber.Lumber_Size = searchNode.Attributes["Size"].Value;
                lumber.Lumber_Thickness = searchNode.Attributes["Thickness"].Value;
                lumber.Lumber_Width = searchNode.Attributes["Width"].Value;
                if (searchNode.Attributes["Grade"] != null)
                {
                    lumber.Lumber_Grade = searchNode.Attributes["Grade"].Value;
                }
                else
                {
                    lumber.Lumber_Grade = "-";
                }
                lumber.Lumber_SpeciesName = searchNode.Attributes["SpeciesName"].Value;
                lumber.Lumber_Sequence = int.Parse(searchNode.Attributes["Sequence"].Value);
                lumber_inv.Add(lumber);
            }
            return lumber_inv;
        }
    }

}
