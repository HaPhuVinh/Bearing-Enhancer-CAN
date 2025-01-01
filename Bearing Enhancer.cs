using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Bearing_Enhancer_CAN
{
    public class Bearing_Enhancer
    {
        public string TrussName { get; set; }
        public int Ply { get; set; }
        public string BCSpecie { get; set; }
        public string TOPSpecie { get; set; }
        public string WebSpecie { get; set; }
        public double DOL { get; set; }
        public string BCSize { get; set; }
        public double Reaction { get; set; }
        public string BearingWidth { get; set; }
        public string RequireWidth { get; set; }
        public bool Horizontal { get; set; }
        public string LoadTransfer { get; set; }
        public string SgtOption1 { get; set; }
        public string SgtOption2 { get; set; }
        public string SgtOption3 { get; set; }
        public string BlockOption { get; set; }
        public string BlockSize { get; set; }
        public int BlockLength { get; set; }
        public double NumberFast { get; set; }
        public string FastType { get; set; }
        public int NumberRow { get; set; }

        public Bearing_Enhancer()
        {
            TrussName = "";
            Ply = 0;
            BCSpecie = "";
            DOL = 1;
            BCSize = "";
            Reaction = 0;
            BearingWidth = "";
            RequireWidth = "";
            Horizontal = true;
            LoadTransfer = "";
            SgtOption1 = "";
            SgtOption2 = "";
            SgtOption3 = "";
            BlockOption = "";
            BlockSize = "";
            BlockLength = 0;
            NumberFast = 0;
            FastType = "";
            NumberRow = 0;
        }

        public Bearing_Enhancer(string trussName, int ply, string bcSpecie, string topSpecie, string webSpecie, double dol, string bCSize, double reaction, string bearingWidth, string require,
            bool horizontal, string loadTransfer, string sgtOption1, string sgtOption2, string sgtOption3, string blockOption, string blockSize,
            int blockLength, double numberFast, string fastType, int numberRow)
        {
            TrussName = trussName;
            Ply = ply;
            BCSpecie = bcSpecie;
            TOPSpecie = topSpecie;
            WebSpecie = webSpecie;
            DOL = dol;
            BCSize = bCSize;
            Reaction = reaction;
            BearingWidth = bearingWidth;
            RequireWidth = require;
            Horizontal = horizontal;
            LoadTransfer = loadTransfer;
            SgtOption1 = sgtOption1;
            SgtOption2 = sgtOption2;
            SgtOption3 = sgtOption3;
            BlockOption = blockOption;
            BlockSize = blockSize;
            BlockLength = blockLength;
            NumberFast = numberFast;
            FastType = fastType;
            NumberRow = numberRow;
        }

        public Bearing_Enhancer(string trussName, int ply, string bcSpecie, string topSpecie, string webSpecie, double dol, string bCSize, double reaction, string bearingWidth, string require,
            bool horizontal, string loadTransfer, string sgtOption1, string sgtOption2, string sgtOption3, string blockOption)
        {
            TrussName = trussName;
            Ply = ply;
            BCSpecie = bcSpecie;
            TOPSpecie = topSpecie;
            WebSpecie = webSpecie;
            DOL = dol;
            BCSize = bCSize;
            Reaction = reaction;
            BearingWidth = bearingWidth;
            RequireWidth = require;
            Horizontal = horizontal;
            LoadTransfer = loadTransfer;
            SgtOption1 = sgtOption1;
            SgtOption2 = sgtOption2;
            SgtOption3 = sgtOption3;
            BlockOption = blockOption;
        }

        public void Get_TrussInfo()
        {

        }

        public List<LumberInventory> Get_Lumber_Inv(string projectnumber)
        {
            List<LumberInventory> lumber_inv = new List<LumberInventory>();

            string projectNumber = projectnumber;
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
