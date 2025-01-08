﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bearing_Enhancer_CAN
{
    public class LumberInventory
    {
        public string Lumber_Key { get; set; }
        public string Lumber_Name { get; set; }
        public string Lumber_Size { get; set; }
        public string Lumber_Thickness { get; set; }
        public string Lumber_Width { get; set; }
        public string Lumber_Grade { get; set; }
        public string Lumber_SpeciesName { get; set; }
        public int Lumber_Sequence { get; set; }


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
