using BloodAndBittersteel.Features.BaBIncidents;
using BloodAndBittersteel.Features.BlackfyreRebellion;
using BloodAndBittersteel.Features.ModifiableValues;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Incidents;
using TaleWorlds.SaveSystem;

namespace BloodAndBittersteel
{
    internal class BaBSaveableTypeDefiner : SaveableTypeDefiner
    {
        public BaBSaveableTypeDefiner() : base(5_18379_918)
        {
        }
        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(TimedModifier), 1);
            AddClassDefinition(typeof(TimedModifierNumber), 2);
            AddClassDefinition(typeof(BlackfyreRebellionData), 3);
            AddClassDefinition(typeof(BaBIncident), 4);
        }
        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<string>));
            ConstructContainerDefinition(typeof(List<TimedModifier>));
            ConstructContainerDefinition(typeof(Dictionary<string, CampaignTime>));
        }
    }
}