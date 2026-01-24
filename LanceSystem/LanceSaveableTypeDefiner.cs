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
            AddClassDefinition(typeof(DisbandedLancePartyComponent), 3);
        }
        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<LanceData>));
            ConstructContainerDefinition(typeof(Dictionary<string, List<LanceData>>));
            ConstructContainerDefinition(typeof(Dictionary<string, NotableLanceData>));
            ConstructContainerDefinition(typeof(Dictionary<string, CampaignTime>));
        }
    }
}