using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.BaBEvents
{
    public class BaBPopupEvent : BaBImmediateEvent
    {
        public string ImagePath { get; }
        public TextObject TitleText { get; }
        public List<TextObject> DescriptionTexts { get; }
        private Func<bool> _condition;

        public BaBPopupEvent(
            string stringId,
            BaBEventTypes eventType,
            float chance,
            string imagePath,
            TextObject titleText,
            List<TextObject> descriptionTexts,
            Action onFire,
            Func<bool>? condition = null)
            : base(stringId, eventType, chance, onFire)
        {
            ImagePath = imagePath;
            TitleText = titleText;
            DescriptionTexts = descriptionTexts;
            _condition = condition ?? (() => true);
        }

        public override bool CheckCondition() => _condition();

        protected override void FireInternal(MapState mapState)
        {
            
        }
    }
}
