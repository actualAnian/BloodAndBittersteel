using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace BloodAndBittersteel.Models
{
    public class BnBCampaignTimeModel : CampaignTimeModel
    {
        public override CampaignTime CampaignStartTime
        {
            get
            {
                return CampaignTime.Years(195f) + CampaignTime.Hours(9f);
            }
        }
        public override int SunRise => 2;

        public override int SunSet => 22;

        public override long TimeTicksPerMillisecond => 10L;

        public override int MillisecondInSecond => 1000;

        public override int SecondsInMinute => 60;

        public override int MinutesInHour => 60;

        public override int HoursInDay => 24;

        public override int DaysInWeek => 7;

        public override int WeeksInSeason => 4;

        public override int SeasonsInYear => 12;
    }
}
