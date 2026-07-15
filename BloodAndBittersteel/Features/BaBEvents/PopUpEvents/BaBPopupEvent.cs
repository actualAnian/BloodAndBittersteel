using BloodAndBittersteel.Features.BaBEvents.PopUpVM;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents
{
    public class BaBPopupEvent : BaBImmediateEvent
    {
        public string ImageStringId { get; }
        public TextObject TitleText { get; }
        public TextObject Description { get; }
        private readonly Func<bool> _condition;

        public BaBPopupEvent(
            string stringId,
            BaBEventTypes eventType,
            float chance,
            string imageStringId,
            TextObject titleText,
            TextObject description,
            CampaignTime cooldown,
            Action onFire,
            Func<bool>? condition = null)
            : base(stringId, eventType, chance, onFire, cooldown)
        {
            ImageStringId = imageStringId;
            TitleText = titleText;
            Description = description;
            _condition = condition ?? (() => true);
        }

        public override bool CheckCondition() => _condition();

        protected override void FireInternal(MapState mapState)
        {
            GameStateManager.Current.PushState(GameStateManager.Current.CreateState<BaBEventState>(TitleText, Description, "test"));

        }
    }
}
