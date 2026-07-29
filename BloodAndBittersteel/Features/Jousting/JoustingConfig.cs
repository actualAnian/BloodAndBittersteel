using System.Collections.Generic;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.Jousting
{
    public static class JoustingConfig
    {
        public readonly static TextObject BaseReason = new("{CLAN_OWNER} decided to host a jousting tournament in {TOWN_NAME}");
        public readonly static TextObject WeddingReason = new("To celebrate a wedding between {GROOM} and {BRIDE}, a jousting tournament is held by clan {CLAN_NAME} in {TOWN_NAME}");
        public const float GroomBoostDurationDays = 7f;
        public const float WeddingBoostChance = 0.15f;
        public const float HostingMalusPerActiveTournament = 0.2f;
        public const int MinimumTreasuryForHosting = 50000;
        public const int AmountOfGoldForMaxBonus = 100000;
        public const float MaxScoreFromTreasury = 0.20f;
        public const float DefaultJoustBaseChance = 0.03f;
        public const float NearByLordRange = 100f;
        public const int MaxActiveJoustingTournamentsForPlayerToHostHisOwn = 2;
        public const int PlayerHostedJoustCost = 50000;
        public static readonly List<string> CulturesThatCantHost = new()
        {
            Globals.WildlingsCultureId,
            Globals.IronbornCultureId
        };

        // ai decision making to move to the town with tournament score
        public static float MaxDistanceScore = 8f;
        public static float MaxDistanceForAttraction = 150f; // 200
        public static float MaxRelationScore = 6f;
        public static float MaxMilitaryPenalty = 10f;
        public static float ArmyPenaltyPerArmy = 2f;
        public static float OwnerBonusScore = 15f;
        public static float GroomBonusScore = 30f;
        //
    }
}