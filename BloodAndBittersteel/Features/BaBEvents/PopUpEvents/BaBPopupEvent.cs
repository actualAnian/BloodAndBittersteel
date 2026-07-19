using SandBox.View.Map;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents
{
    public class BaBPopupEvent : BaBImmediateEvent
    {
        public string ImageStringId { get; }
        public TextObject TitleText { get; }
        public TextObject Description { get; }
        public BaBPopupEvent(
            string stringId,
            BaBEventTypes eventType,
            float chance,
            string imageStringId,
            TextObject titleText,
            TextObject description,
            CampaignTime cooldown,
            Func<bool> condition,
            Action consequence)
            : base(stringId, eventType, chance, condition, consequence, cooldown)
        {
            ImageStringId = imageStringId;
            TitleText = titleText;
            Description = description;
        }

        protected override void FireInternal()
        {
            if (!BaBEventMapScreenExtensions.CanShowBaBEventPopup()) return;

            var mapScreen = MapScreen.Instance;
            if (mapScreen == null) return;
            mapScreen.AddMapView<BaBEventPopupView>(
                TitleText.ToString(),
                Description.ToString(),
                ImageStringId);
        }
    }
}
