using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace BloodAndBittersteel.Features.ModifiableValues
{
    public class TimedModifier
    {
        [SaveableProperty(1)]
        public string Id { get; set; }
        [SaveableProperty(2)]
        public float Factor { get; set; }
        [SaveableProperty(3)]
        public CampaignTime EndTime { get; set; }
        [SaveableProperty(4)]
        public string Description { get; set; }


        public TimedModifier(string id, float factor, CampaignTime endTime, string description = "")
        {
            Id = id;
            Factor = factor;
            EndTime = endTime;
            Description = description;
        }
        public bool IsExpired => EndTime.IsPast;
    }

}
