using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodAndBittersteel.Features.LanceSystem.Deserialization
{
    public enum LanceTemplateSettlementType
    {
        Town,
        Settlement,
        Castle,
        All
    }
    public class LanceTemplates
    {
        public List<Lance> Lances { get; set; } = new();
    }

    public class Lance
    {
        public string CultureId { get; set; } = string.Empty;
        public LanceTemplateSettlementType SettlementType { get; set; }
        public Troops Troops { get; set; } = new();
    }

    public class Troops
    {
        public TroopData MeleeTroop { get; set; } = new();
        public TroopData RangedTroop { get; set; } = new();
        public TroopData CavalryTroop { get; set; } = new();
        public TroopData HorseArcherTroop { get; set; } = new();
    }

    public class TroopData
    {
        public double Likelihood { get; set; }
        public string BasicTroopId { get; set; } = string.Empty;
    }

}
