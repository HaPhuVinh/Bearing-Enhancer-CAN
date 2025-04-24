using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearing_Enhancer_CAN
{
    public class English_Or_French
    {
        public string Language = "English";
        public string Truss => Language=="English"? "Truss:": "Ferme:";
        public string Qty => Language == "English" ? "Qty:" : "Qté:";
        public string BuildingCode => Language == "English" ? "Building Code:" : "Code du Bâtiment";
        public string Kd => Language == "English" ? "Kd" : "Kd";
        public string Snow => Language == "English" ? "(Snow)" : "(neige)";
        public string Live => Language == "English" ? "(Live)" : "(Vive)";
        public string Wind => Language == "English" ? "(Wind)" : "(Vent)";
        public string Jnt => Language == "English" ? "Jnt" : "Jnt";
        public string XLoc => Language == "English" ? "X-Loc" : "Empl X";
        public string React => Language == "English" ? "React" : "Réact";
        public string Up => Language == "English" ? "Up" : "Soulv.";
        public string Width => Language == "English" ? "Width" : "Larg";
        public string Reqd => Language == "English" ? "Reqd" : "Exig.";
        public string Mat => Language == "English" ? "Mat" : "Mat";
        public string UnfactoredReactionSummary => Language == "English" ? "Unfactored Reaction Summary" : "Résumé des réactions non-pondérées";


        public English_Or_French(string language)
        {
            Language = language;
        }
    }
}
