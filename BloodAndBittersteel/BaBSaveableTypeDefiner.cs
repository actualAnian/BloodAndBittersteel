using BloodAndBittersteel.Features.BaBIncidents;
using BloodAndBittersteel.Features.BlackfyreRebellion;
using BloodAndBittersteel.Features.IronbornWives;
using BloodAndBittersteel.Features.ModifiableValues;
using BloodAndBittersteel.Features.NightsWatch;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
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
            AddClassDefinition(typeof(BecomeSaltWifeLogEntry), 5);
            AddClassDefinition(typeof(JoinedNightsWatchLogEntry), 6);
        }
        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<string>));
            ConstructContainerDefinition(typeof(List<TimedModifier>));
            ConstructContainerDefinition(typeof(Dictionary<string, CampaignTime>));
        }
    }
}