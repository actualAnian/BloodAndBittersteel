using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace BaBUnitTests.TestUtilities
{
    internal class TestCampaignTimeModel : CampaignTimeModel
    {
        public override CampaignTime CampaignStartTime => new();

        public override int SunRise => 6;

        public override int SunSet => 20;

        public override long TimeTicksPerMillisecond => 1;

        public override int MillisecondInSecond => 1000;

        public override int SecondsInMinute => 60;

        public override int MinutesInHour => 60;

        public override int HoursInDay => 24;

        public override int DaysInWeek => 7;

        public override int WeeksInSeason => 4;

        public override int SeasonsInYear => 4;
    }
}
