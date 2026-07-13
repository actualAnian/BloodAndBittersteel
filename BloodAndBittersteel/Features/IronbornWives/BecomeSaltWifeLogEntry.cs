using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.IronbornWives
{
    public class BecomeSaltWifeLogEntry : LogEntry, IEncyclopediaLog, IChatNotification
    {
        private readonly Hero _happenedTo;
        private readonly Hero _forcedBy;

        public BecomeSaltWifeLogEntry(Hero happenedTo, Hero forcedBy)
        {
            _happenedTo = happenedTo;
            _forcedBy = forcedBy;
        }

        public bool IsVisibleNotification => true;

        public TextObject GetEncyclopediaText()
        {
            return GetNotificationText();
        }

        public const string NotificationText = "{=bab_salt_wife_log}{HAPPENED_TO} became the salt wife of {FORCED_BY}.";
        public TextObject GetNotificationText()
        {
            var text = new TextObject(NotificationText);
            GameTexts.SetVariable("HAPPENED_TO", _happenedTo.CharacterObject.Name);
            GameTexts.SetVariable("FORCED_BY", _forcedBy.CharacterObject.Name);

            return text;
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == _happenedTo || obj == _forcedBy;
        }
    }

}
