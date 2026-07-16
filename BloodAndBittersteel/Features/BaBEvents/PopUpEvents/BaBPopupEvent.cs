using SandBox.View.Map;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
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

        protected override void FireInternal()
        {
            if (!BaBEventMapScreenExtensions.CanShowBaBEventPopup()) return;

            var mapScreen = MapScreen.Instance;
            if (mapScreen == null) return;
            mapScreen.AddMapView<BaBEventPopupView>(
                TitleText.ToString(),
                Description.ToString(),
                "test");
        }
    }
}
