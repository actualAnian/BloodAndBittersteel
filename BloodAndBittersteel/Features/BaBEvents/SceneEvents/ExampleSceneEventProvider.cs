using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;

namespace BloodAndBittersteel.Features.BaBEvents.SceneEvents
{
    //public class ExampleSceneNotificationData : SceneNotificationData
    //{
    //    public override string SceneID => "example_scene";
    //}

    public class ExampleSceneEventProvider : IEventProvider
    {
        public IEnumerable<IBaBEvent> InitializeEvents()
        {
            yield return new BaBSceneEvent(
                "scene_example",
                BaBEventTypes.OnWeeklyTick,
                new BecomeKingSceneNotificationItem(Hero.MainHero),
                () => { },
                () => { return true; }
            );
        }
    }
}
