using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents.Events
{
    public class JoustingEvent
    {
        public const string StringId = "jousting_tournament";
        [BaBEvent]
        private static BaBPopupEvent CreateEvent()
        {
            return new BaBPopupEvent(
                StringId,
                BaBEventTypes.OnWeeklyTick,
                0f,
                "test",
                new("{bab_jousting_tournament}Jousting Tournament!"),
                new("{JOUSTING_DESCRIPTION}"),
                CampaignTime.Days(24),
                () => { return false; },
                () => { });
        }

    }
}
