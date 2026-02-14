using LanceSystem.LanceDataClasses;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace LanceSystem
{
    public class LanceSaveableTypeDefiner : SaveableTypeDefiner
    {
        public LanceSaveableTypeDefiner() : base(1_87240_004) { }
        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(LanceData), 1);
            AddClassDefinition(typeof(NotableLanceData), 2);
            AddClassDefinition(typeof(MercenaryLanceData), 3);
            AddClassDefinition(typeof(SettlementNotableLanceInfo), 4);
            AddClassDefinition(typeof(DisbandedLancePartyComponent), 5);
        }
        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<LanceData>));
            ConstructContainerDefinition(typeof(List<MercenaryLanceData>));
            ConstructContainerDefinition(typeof(Dictionary<string, List<LanceData>>));
            ConstructContainerDefinition(typeof(Dictionary<string, SettlementNotableLanceInfo>));     
            ConstructContainerDefinition(typeof(Dictionary<string, CampaignTime>));
        }
    }
}