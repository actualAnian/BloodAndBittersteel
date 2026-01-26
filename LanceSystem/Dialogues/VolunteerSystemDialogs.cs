using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace LanceSystem.Dialogues
{
    public class VolunteerSystemDialogs
    {
        static readonly List<string> Neutral = new()
        {
            "{=lance_volunteer_response_neutral_01}There are no men here seeking service at present.",
            "{=lance_volunteer_response_neutral_02}None here are free to take up arms under another banner.",
            "{=lance_volunteer_response_neutral_03}All able hands are bound already.",
            "{=lance_volunteer_response_neutral_04}No one here is without prior obligation.",
            "{=lance_volunteer_response_neutral_05}Those who might fight are sworn elsewhere.",
            "{=lance_volunteer_response_neutral_06}There are no volunteers to be found.",
            "{=lance_volunteer_response_neutral_07}No men here stand idle enough to follow you.",
            "{=lance_volunteer_response_neutral_08}The village has no spare men to offer.",
            "{=lance_volunteer_response_neutral_09}All who could bear arms are spoken for.",
            "{=lance_volunteer_response_neutral_10}None here are free of duty.",
            "{=lance_volunteer_response_neutral_11}No one here seeks a new master.",
            "{=lance_volunteer_response_neutral_12}Those able to fight are already accounted for.",
            "{=lance_volunteer_response_neutral_13}There are no willing hands at this time.",
            "{=lance_volunteer_response_neutral_14}No one here is prepared to leave their work.",
            "{=lance_volunteer_response_neutral_15}None are available to follow you.",
            "{=lance_volunteer_response_neutral_16}No one here is looking to leave their current obligations.",
            "{=lance_volunteer_response_neutral_17}There is no lack of men, only of freedom.",
            "{=lance_volunteer_response_neutral_18}No volunteers present themselves today.",
            "{=lance_volunteer_response_neutral_19}There are no willing hands at this time."
        };
        static readonly List<string> Courteous = new()
        {
            "{=lance_volunteer_response_courteous_01}I regret it, my {?PLAYER.GENDER}lady{?}lord{\\?}, but none here seek new service.",
            "{=lance_volunteer_response_courteous_02}Were there willing men, I would gladly direct them to you.",
            "{=lance_volunteer_response_courteous_03}You honor us by asking, yet there are none to offer.",
            "{=lance_volunteer_response_courteous_04}All able men are bound, my {?PLAYER.GENDER}lady{?}lord{\\?}.",
            "{=lance_volunteer_response_courteous_05}I would not deny you if I could, but none are free.",
            "{=lance_volunteer_response_courteous_06}Your banner is known, yet no man here may answer it.",
            "{=lance_volunteer_response_courteous_07}It pains me to say it, but no man here may follow you.",
            "{=lance_volunteer_response_courteous_08}None are available at present, my {?PLAYER.GENDER}lady{?}lord{\\?}.",
            "{=lance_volunteer_response_courteous_09}All who might serve are already sworn elsewhere.",
            "{=lance_volunteer_response_courteous_10}I regret we have no men to spare for you.",
            "{=lance_volunteer_response_courteous_11}Your request is received with respect, though unmet.",
            "{=lance_volunteer_response_courteous_12}No volunteers stand ready at this time.",
            "{=lance_volunteer_response_courteous_13}I wish I could be of greater help, but there are none.",
            "{=lance_volunteer_response_courteous_14}None here are free to follow another banner.",
            "{=lance_volunteer_response_courteous_15}We have no men who may answer your call.",
            "{=lance_volunteer_response_courteous_16}None are free, though your request is well received.",
            "{=lance_volunteer_response_courteous_17}No volunteers today, my {?PLAYER.GENDER}lady{?}lord{\\?}."
        };
        static readonly List<string> SlightlyCourteous = new()
        {
            "{=lance_volunteer_response_slight_01}You have some name, at least, but I have no men to offer.",
            "{=lance_volunteer_response_slight_02}Your request is heard, though none may answer it.",
            "{=lance_volunteer_response_slight_03}I see your intent, but there are no volunteers here.",
            "{=lance_volunteer_response_slight_04}You ask plainly enough, yet none are free.",
            "{=lance_volunteer_response_slight_05}Your banner is noted, but it finds no takers here.",
            "{=lance_volunteer_response_slight_06}None here are prepared to follow you just yet.",
            "{=lance_volunteer_response_slight_07}There are no men willing to leave their duties.",
            "{=lance_volunteer_response_slight_08}I cannot offer you men at this time.",
            "{=lance_volunteer_response_slight_09}You may ask, but there are none to answer.",
            "{=lance_volunteer_response_slight_10}No volunteers present themselves today."
        };
        static readonly List<string> Suspicious_T0 = new()
        {
            "{=lance_volunteer_response_suspicious_t0_01}Men do not gamble their lives on nameless banners.",
            "{=lance_volunteer_response_suspicious_t0_02}You ask much for one so little known.",
            "{=lance_volunteer_response_suspicious_t0_03}No one here is eager to follow an upstart.",
            "{=lance_volunteer_response_suspicious_t0_04}Words are cheap; loyalty is not.",
            "{=lance_volunteer_response_suspicious_t0_05}Men here wait to see if names endure.",
            "{=lance_volunteer_response_suspicious_t0_06}Few are keen to die for untested promises.",
            "{=lance_volunteer_response_suspicious_t0_07}Strength is proven in deeds, not requests.",
            "{=lance_volunteer_response_suspicious_t0_08}No one rushes to follow a stranger’s cause.",
            "{=lance_volunteer_response_suspicious_t0_09}Men value caution over empty ambition.",
            "{=lance_volunteer_response_suspicious_t0_10}You will find no blind faith here.",
            "{=lance_volunteer_response_suspicious_t0_11}No one is eager to gamble their neck on new promises.",
            "{=lance_volunteer_response_suspicious_t0_12}Men follow strength proven, not claimed."
       };

        static readonly List<string> Suspicious_T1 = new()
        {
            "{=lance_volunteer_response_suspicious_t1_01}Your name is heard, but trust comes slowly.",
            "{=lance_volunteer_response_suspicious_t1_02}Men here watch before they commit.",
            "{=lance_volunteer_response_suspicious_t1_03}Reputation takes time to settle.",
            "{=lance_volunteer_response_suspicious_t1_04}Not all banners are worth the risk.",
            "{=lance_volunteer_response_suspicious_t1_05}Men do not follow lightly, even known names.",
            "{=lance_volunteer_response_suspicious_t1_06}Some here wait for firmer footing.",
            "{=lance_volunteer_response_suspicious_t1_07}Folk here measure twice before they act.",
            "{=lance_volunteer_response_suspicious_t1_08}Caution still rules these lands.",
            "{=lance_volunteer_response_suspicious_t1_09}Men listen, but do not yet move.",
            "{=lance_volunteer_response_suspicious_t1_10}Your cause is heard, not yet trusted.",
            "{=lance_volunteer_response_suspicious_t1_11}Your banner has yet to weigh heavy here.",
            "{=lance_volunteer_response_suspicious_t1_12}Folk here are cautious with their lives."
        };
        static readonly List<string> Suspicious_T2 = new()
        {
            "{=lance_volunteer_response_suspicious_t2_01}Men choose their oaths carefully.",
            "{=lance_volunteer_response_suspicious_t2_02}Even good names invite caution.",
            "{=lance_volunteer_response_suspicious_t2_03}Loyalty is not given without need.",
            "{=lance_volunteer_response_suspicious_t2_04}Men weigh risk, even under rightful banners.",
            "{=lance_volunteer_response_suspicious_t2_05}Service is offered when necessity calls.",
            "{=lance_volunteer_response_suspicious_t2_06}Few leave steady lives without cause.",
            "{=lance_volunteer_response_suspicious_t2_07}Men stand ready, but not restless.",
            "{=lance_volunteer_response_suspicious_t2_08}Prudence guides men more than zeal.",
            "{=lance_volunteer_response_suspicious_t2_09}Not all calls to arms are answered at once.",
            "{=lance_volunteer_response_suspicious_t2_10}Men wait for the right moment to serve.",
            "{=lance_volunteer_response_suspicious_t2_11}Trust is slow to earn in these lands."
        };
        static readonly List<string> Brazen_T0 = new()
        {
            "{=lance_volunteer_response_brazen_t0_01}This is not a place for reckless dreams.",
            "{=lance_volunteer_response_brazen_t0_02}Men here value their lives more than your cause.",
            "{=lance_volunteer_response_brazen_t0_03}You will find no eager swords here.",
            "{=lance_volunteer_response_brazen_t0_04}If you seek fools, look elsewhere.",
            "{=lance_volunteer_response_brazen_t0_05}We are not so desperate as to follow you.",
            "{=lance_volunteer_response_brazen_t0_06}Your words stir little here.",
            "{=lance_volunteer_response_brazen_t0_07}Men do not queue for uncertain banners.",
            "{=lance_volunteer_response_brazen_t0_08}This is not a market for empty ambition.",
            "{=lance_volunteer_response_brazen_t0_09}You overestimate your pull.",
            "{=lance_volunteer_response_brazen_t0_10}No one here is eager to throw his life away."
        };
        static readonly List<string> Brazen_T1 = new()
        {
            "{=lance_volunteer_response_brazen_t1_01}You speak boldly, but that does not summon men.",
            "{=lance_volunteer_response_brazen_t1_02}Ambition alone does not earn followers.",
            "{=lance_volunteer_response_brazen_t1_03}Men here choose carefully whom they serve.",
            "{=lance_volunteer_response_brazen_t1_04}This is not a call that stirs many hearts.",
            "{=lance_volunteer_response_brazen_t1_05}You may ask. That is all.",
            "{=lance_volunteer_response_brazen_t1_06}Not every banner finds takers.",
            "{=lance_volunteer_response_brazen_t1_07}Men are not so easily impressed.",
            "{=lance_volunteer_response_brazen_t1_08}You expect more than is owed.",
            "{=lance_volunteer_response_brazen_t1_09}Such requests carry weight only in time.",
            "{=lance_volunteer_response_brazen_t1_10}You ask much of men who owe you little."
        };
        static readonly List<string> Brazen_T2 = new()
        {
            "{=lance_volunteer_response_brazen_t2_01}Even established banners go unanswered at times.",
            "{=lance_volunteer_response_brazen_t2_02}Men are cautious with their service.",
            "{=lance_volunteer_response_brazen_t2_03}Not all calls find willing ears.",
            "{=lance_volunteer_response_brazen_t2_04}Men here are not quick to uproot themselves.",
            "{=lance_volunteer_response_brazen_t2_05}Service is not given without need.",
            "{=lance_volunteer_response_brazen_t2_06}Men follow necessity, not impulse.",
            "{=lance_volunteer_response_brazen_t2_07}There is little haste in taking new oaths.",
            "{=lance_volunteer_response_brazen_t2_08}Men here prefer certainty.",
            "{=lance_volunteer_response_brazen_t2_09}Even known lords must wait.",
            "{=lance_volunteer_response_brazen_t2_10}The call to arms is not always answered at once."
        };
        static readonly List<string> Female_T0 = new()
        {
            "{=lance_volunteer_response_female_t0_01}A woman calling for men? I thought I had heard everything.",
            "{=lance_volunteer_response_female_t0_02}This is soldier’s work, not something for skirts and words.",
            "{=lance_volunteer_response_female_t0_03}Men do not leave their homes for a woman’s ambition.",
            "{=lance_volunteer_response_female_t0_04}You would have men die, yet you’ve never stood where they stand.",
            "{=lance_volunteer_response_female_t0_05}War does not answer to soft hands, no matter how bold the tongue.",
            "{=lance_volunteer_response_female_t0_06}I will not mock my people by sending them to a woman’s banner.",
            "{=lance_volunteer_response_female_t0_07}Men follow strength, not novelty.",
            "{=lance_volunteer_response_female_t0_08}This land has seen many fools, but few so misplaced.",
            "{=lance_volunteer_response_female_t0_09}You should seek a hearth, not soldiers.",
            "{=lance_volunteer_response_female_t0_10}No man here would shame himself by following you."
        }; 
        static readonly List<string> FemaleTier1 = new()
        {
            "{=lance_volunteer_response_female_t1_01}Your name is spoken, but doubts remain, especially given who you are.",
            "{=lance_volunteer_response_female_t1_02}Some find it… unusual, seeing a woman ask for men.",
            "{=lance_volunteer_response_female_t1_03}Men hesitate when custom is bent too far.",
            "{=lance_volunteer_response_female_t1_04}You are bold, I grant that, though many still question it.",
            "{=lance_volunteer_response_female_t1_05}Not all are ready to serve beneath a woman’s command.",
            "{=lance_volunteer_response_female_t1_06}This is no tale to stir men into following you.",
            "{=lance_volunteer_response_female_t1_07}Such things invite talk, and caution.",
            "{=lance_volunteer_response_female_t1_08}Men here weigh tradition heavily.",
            "{=lance_volunteer_response_female_t1_09}Some find your presence… unsettling.",
        };
        static readonly List<string> FemaleTier2 = new()
        {
            "{=lance_volunteer_response_female_t2_01}Not all customs bend easily, even for respected figures.",
            "{=lance_volunteer_response_female_t2_02}You command respect but old habits die slowly.",
        };
        static readonly List<string> GangLeaderLines = new()
        {
            "{=lance_volunteer_response_gang_01}My boys don’t die for strangers.",
            "{=lance_volunteer_response_gang_02}Men here don’t march unless there’s blood or coin.",
            "{=lance_volunteer_response_gang_03}No one’s eager to trade turf for a battlefield.",
            "{=lance_volunteer_response_gang_04}This isn’t a charity for doomed causes.",
            "{=lance_volunteer_response_gang_05}Your banner doesn’t scare anyone yet.",
            "{=lance_volunteer_response_gang_06}We survive by staying put.",
            "{=lance_volunteer_response_gang_07}My men answer to me, not passing ambitions.",
            "{=lance_volunteer_response_gang_08}They know better than to follow promises.",
            "{=lance_volunteer_response_gang_09}War chews men up. Smart ones stay here.",
            "{=lance_volunteer_response_gang_10}No volunteers. No fools."
        };
        static readonly List<string> MerchantLines = new()
        {
            "{=lance_volunteer_response_merch_01}Trade keeps men fed better than war.",
            "{=lance_volunteer_response_merch_02}Those with sense keep to their ledgers.",
            "{=lance_volunteer_response_merch_03}Risk is best avoided when profit is steady.",
            "{=lance_volunteer_response_merch_04}No one leaves honest work lightly.",
            "{=lance_volunteer_response_merch_05}Men here value coin over glory.",
            "{=lance_volunteer_response_merch_06}Uncertainty frightens investors and families.",
            "{=lance_volunteer_response_merch_07}War disrupts markets, not inspires them.",
            "{=lance_volunteer_response_merch_08}There is little appetite for danger today.",
            "{=lance_volunteer_response_merch_09}Stability keeps men where they are.",
            "{=lance_volunteer_response_merch_10}No volunteers at present."
        };
        static readonly List<string> HeadmanLines = new()
        {
            "{=lance_volunteer_response_head_01}Our people cannot spare strong hands.",
            "{=lance_volunteer_response_head_02}Families come before banners.",
            "{=lance_volunteer_response_head_03}The fields need tending more than swords.",
            "{=lance_volunteer_response_head_04}I will not send sons away lightly.",
            "{=lance_volunteer_response_head_05}We have suffered enough already.",
            "{=lance_volunteer_response_head_06}War takes more than it gives.",
            "{=lance_volunteer_response_head_07}My duty is to those who live here.",
            "{=lance_volunteer_response_head_08}There are no men free to leave.",
            "{=lance_volunteer_response_head_09}Our village must endure.",
            "{=lance_volunteer_response_head_10}None will volunteer."
        };
        static readonly List<string> ArtisanLines = new()
        {
            "{=lance_volunteer_response_artisan_01}Skilled hands are worth more at the bench than the grave.",
            "{=lance_volunteer_response_artisan_02}Craft feeds families longer than war.",
            "{=lance_volunteer_response_artisan_03}Men here value skill over steel.",
            "{=lance_volunteer_response_artisan_04}No one abandons their trade lightly.",
            "{=lance_volunteer_response_artisan_05}War dulls talents better used elsewhere.",
            "{=lance_volunteer_response_artisan_06}Our work is our survival.",
            "{=lance_volunteer_response_artisan_07}Men here build, not destroy.",
            "{=lance_volunteer_response_artisan_08}There are no idle hands.",
            "{=lance_volunteer_response_artisan_09}Craft keeps men grounded.",
            "{=lance_volunteer_response_artisan_10}None wish to leave their work."
        };
        public static string GetNoVolunteersDialogue(Hero notable)
        {
            List<string> pool = new();
            bool isFemale = Hero.MainHero.IsFemale;
            switch (Hero.MainHero.Clan.Tier)
            {
                case 0:
                    pool.AddRange(Suspicious_T0);
                    pool.AddRange(Brazen_T0);
                    if (notable.IsGangLeader) pool.AddRange(GangLeaderLines);
                    if (notable.IsHeadman) pool.AddRange(HeadmanLines);
                    if (notable.IsMerchant) pool.AddRange(MerchantLines);
                    if (notable.IsArtisan) pool.AddRange(ArtisanLines);
                    if (LanceSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(Female_T0);
                    break;

                case 1:
                    pool.AddRange(Suspicious_T1);
                    pool.AddRange(Brazen_T1);
                    pool.AddRange(SlightlyCourteous);
                    if (notable.IsGangLeader) pool.AddRange(GangLeaderLines);
                    if (notable.IsHeadman) pool.AddRange(HeadmanLines);
                    if (notable.IsMerchant) pool.AddRange(MerchantLines);
                    if (notable.IsArtisan) pool.AddRange(ArtisanLines);
                    if (LanceSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(FemaleTier1);
                    break;
                case 2:
                    pool.AddRange(Suspicious_T2);
                    pool.AddRange(Brazen_T2);
                    pool.AddRange(Courteous);
                    pool.AddRange(Neutral);
                    if (notable.IsGangLeader) pool.AddRange(GangLeaderLines);
                    if (notable.IsHeadman) pool.AddRange(HeadmanLines);
                    if (notable.IsMerchant) pool.AddRange(MerchantLines);
                    if (notable.IsArtisan) pool.AddRange(ArtisanLines);
                    if (LanceSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(FemaleTier2);
                    break;
                default:
                    pool.AddRange(Neutral);
                    pool.AddRange(Courteous);
                    break;
            }
            return pool.GetRandomElement();
        }
    }
}
