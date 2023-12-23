using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRising.GameData;

namespace BloodyEncounters.DB.Models
{
    internal class ItemEncounterModel
    {
        public string name { get; set; }
        public int ItemID { get; set; }
        public int Stack { get; set; }
        public int Chance { get; set; } = 1;
        public string Color { get; set; } = "#daa520";
    }
}
