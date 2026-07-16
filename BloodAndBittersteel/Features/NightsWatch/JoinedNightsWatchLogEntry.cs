using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.NightsWatch
{
    public class JoinedNightsWatchLogEntry : LogEntry, IEncyclopediaLog, IChatNotification
    {
        private readonly Hero _happenedTo;
        private readonly Hero _forcedBy;

        public JoinedNightsWatchLogEntry(Hero happenedTo, Hero forcedBy)
        {
            _happenedTo = happenedTo;
            _forcedBy = forcedBy;
        }

        public bool IsVisibleNotification => true;

        public TextObject GetEncyclopediaText()
        {
            return GetNotificationText();
        }

        public const string NotificationText = "{=bab_nights_watch_log}{HAPPENED_TO} was forced to take the black at the behest of {FORCED_BY}.";
        public TextObject GetNotificationText()
        {
            var text = new TextObject(NotificationText);
            GameTexts.SetVariable("HAPPENED_TO", _happenedTo.CharacterObject.Name);
            GameTexts.SetVariable("FORCED_BY", _forcedBy.CharacterObject.Name);
            return text;
        }

        public bool IsVisibleInEncyclopediaPageOf(MBObjectBase obj)
        {
            return obj == _happenedTo;
        }
    }
}
