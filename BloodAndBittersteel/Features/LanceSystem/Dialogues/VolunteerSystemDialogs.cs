using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.LanceSystem.Dialogues
{
    public class VolunteerSystemDialogs
    {
        static readonly List<string> Neutral = new()
        {
            "{=bab_nv_neutral_01}There are no men here seeking service at present.",
            "{=bab_nv_neutral_02}None here are free to take up arms under another banner.",
            "{=bab_nv_neutral_03}All able hands are bound already.",
            "{=bab_nv_neutral_04}No one here is without prior obligation.",
            "{=bab_nv_neutral_05}Those who might fight are sworn elsewhere.",
            "{=bab_nv_neutral_06}There are no volunteers to be found.",
            "{=bab_nv_neutral_07}No men here stand idle enough to follow you.",
            "{=bab_nv_neutral_08}The village has no spare men to offer.",
            "{=bab_nv_neutral_09}All who could bear arms are spoken for.",
            "{=bab_nv_neutral_10}None here are free of duty.",
            "{=bab_nv_neutral_11}No one here seeks a new master.",
            "{=bab_nv_neutral_12}Those able to fight are already accounted for.",
            "{=bab_nv_neutral_13}There are no willing hands at this time.",
            "{=bab_nv_neutral_14}No one here is prepared to leave their work.",
            "{=bab_nv_neutral_15}None are available to follow you.",
            "{=bab_nv_neutral_16}No one here is looking to leave their current obligations.",
            "{=bab_nv_neutral_17}There is no lack of men — only of freedom.",
            "{=bab_nv_neutral_18}No volunteers present themselves today.",
            "{=bab_nv_neutral_19}There are no willing hands at this time."
        };
        static readonly List<string> Courteous = new()
        {
            "{=bab_nv_courteous_01}I regret it, my {?PLAYER.GENDER}lady{?}lord{\\?}, but none here seek new service.",
            "{=bab_nv_courteous_02}Were there willing men, I would gladly direct them to you.",
            "{=bab_nv_courteous_03}You honor us by asking, yet there are none to offer.",
            "{=bab_nv_courteous_04}All able men are bound, my {?PLAYER.GENDER}lady{?}lord{\\?}.",
            "{=bab_nv_courteous_05}I would not deny you if I could, but none are free.",
            "{=bab_nv_courteous_06}Your banner is known, yet no man here may answer it.",
            "{=bab_nv_courteous_07}It pains me to say it, but no man here may follow you.",
            "{=bab_nv_courteous_08}None are available at present, my {?PLAYER.GENDER}lady{?}lord{\\?}.",
            "{=bab_nv_courteous_09}All who might serve are already sworn elsewhere.",
            "{=bab_nv_courteous_10}I regret we have no men to spare for you.",
            "{=bab_nv_courteous_11}Your request is received with respect, though unmet.",
            "{=bab_nv_courteous_12}No volunteers stand ready at this time.",
            "{=bab_nv_courteous_13}I wish I could be of greater help, but there are none.",
            "{=bab_nv_courteous_14}None here are free to follow another banner.",
            "{=bab_nv_courteous_15}We have no men who may answer your call.",
            "{=bab_nv_c_06}None are free — though your request is well received.",
            "{=bab_nv_c_09}No volunteers today, my {?PLAYER.GENDER}lady{?}lord{\\?}."
        };
        static readonly List<string> SlightlyCourteous = new()
        {
            "{=bab_nv_slight_01}You have some name, at least — but no men to offer.",
            "{=bab_nv_slight_02}Your request is heard, though none may answer it.",
            "{=bab_nv_slight_03}I see your intent, but there are no volunteers here.",
            "{=bab_nv_slight_04}You ask plainly enough, yet none are free.",
            "{=bab_nv_slight_05}Your banner is noted, but it finds no takers here.",
            "{=bab_nv_slight_06}None here are prepared to follow you just yet.",
            "{=bab_nv_slight_07}There are no men willing to leave their duties.",
            "{=bab_nv_slight_08}I cannot offer you men at this time.",
            "{=bab_nv_slight_09}You may ask, but there are none to answer.",
            "{=bab_nv_slight_10}No volunteers present themselves today."
        };
        static readonly List<string> Suspicious_T0 = new()
        {
            "{=bab_nv_s_t0_01}Men do not gamble their lives on nameless banners.",
            "{=bab_nv_s_t0_02}You ask much for one so little known.",
            "{=bab_nv_s_t0_03}No one here is eager to follow an upstart.",
            "{=bab_nv_s_t0_04}Words are cheap; loyalty is not.",
            "{=bab_nv_s_t0_05}Men here wait to see if names endure.",
            "{=bab_nv_s_t0_06}Few are keen to die for untested promises.",
            "{=bab_nv_s_t0_07}Strength is proven in deeds, not requests.",
            "{=bab_nv_s_t0_08}No one rushes to follow a stranger’s cause.",
            "{=bab_nv_s_t0_09}Men value caution over empty ambition.",
            "{=bab_nv_s_t0_10}You will find no blind faith here.",
            "{=bab_nv_s_05}No one is eager to gamble their neck on new promises.",
            "{=bab_nv_s_06}Men follow strength proven, not claimed."
       };

        static readonly List<string> Suspicious_T1 = new()
        {
            "{=bab_nv_s_t1_01}Your name is heard, but trust comes slowly.",
            "{=bab_nv_s_t1_02}Men here watch before they commit.",
            "{=bab_nv_s_t1_03}Reputation takes time to settle.",
            "{=bab_nv_s_t1_04}Not all banners are worth the risk.",
            "{=bab_nv_s_t1_05}Men do not follow lightly, even known names.",
            "{=bab_nv_s_t1_06}Some here wait for firmer footing.",
            "{=bab_nv_s_t1_07}Folk here measure twice before they act.",
            "{=bab_nv_s_t1_08}Caution still rules these lands.",
            "{=bab_nv_s_t1_09}Men listen, but do not yet move.",
            "{=bab_nv_s_t1_10}Your cause is heard, not yet trusted.",
            "{=bab_nv_s_t1_11}Your banner has yet to weigh heavy here.",
            "{=bab_nv_s_t1_12}Folk here are cautious with their lives."
        };
        static readonly List<string> Suspicious_T2 = new()
        {
            "{=bab_nv_s_t2_01}Men choose their oaths carefully.",
            "{=bab_nv_s_t2_02}Even good names invite caution.",
            "{=bab_nv_s_t2_03}Loyalty is not given without need.",
            "{=bab_nv_s_t2_04}Men weigh risk, even under rightful banners.",
            "{=bab_nv_s_t2_05}Service is offered when necessity calls.",
            "{=bab_nv_s_t2_06}Few leave steady lives without cause.",
            "{=bab_nv_s_t2_07}Men stand ready, but not restless.",
            "{=bab_nv_s_t2_08}Prudence guides men more than zeal.",
            "{=bab_nv_s_t2_09}Not all calls to arms are answered at once.",
            "{=bab_nv_s_t2_10}Men wait for the right moment to serve.",
            "{=bab_nv_s_t2_11}Trust is slow to earn in these lands."
        };
        static readonly List<string> Brazen_T0 = new()
        {
            "{=bab_nv_b_t0_01}This is not a place for reckless dreams.",
            "{=bab_nv_b_t0_02}Men here value their lives more than your cause.",
            "{=bab_nv_b_t0_03}You will find no eager swords here.",
            "{=bab_nv_b_t0_04}If you seek fools, look elsewhere.",
            "{=bab_nv_b_t0_05}We are not so desperate as to follow you.",
            "{=bab_nv_b_t0_06}Your words stir little here.",
            "{=bab_nv_b_t0_07}Men do not queue for uncertain banners.",
            "{=bab_nv_b_t0_08}This is not a market for empty ambition.",
            "{=bab_nv_b_t0_09}You overestimate your pull.",
            "{=bab_nv_b_t0_10}No one here is eager to throw his life away."
        };
        static readonly List<string> Brazen_T1 = new()
        {
            "{=bab_nv_b_t1_01}You speak boldly, but that does not summon men.",
            "{=bab_nv_b_t1_02}Ambition alone does not earn followers.",
            "{=bab_nv_b_t1_03}Men here choose carefully whom they serve.",
            "{=bab_nv_b_t1_04}This is not a call that stirs many hearts.",
            "{=bab_nv_b_t1_05}You may ask — that is all.",
            "{=bab_nv_b_t1_06}Not every banner finds takers.",
            "{=bab_nv_b_t1_07}Men are not so easily impressed.",
            "{=bab_nv_b_t1_08}You expect more than is owed.",
            "{=bab_nv_b_t1_09}Such requests carry weight only in time.",
            "{=bab_nv_b_t1_10}You ask much of men who owe you little."
        };
        static readonly List<string> Brazen_T2 = new()
        {
            "{=bab_nv_b_t2_01}Even established banners go unanswered at times.",
            "{=bab_nv_b_t2_02}Men are cautious with their service.",
            "{=bab_nv_b_t2_03}Not all calls find willing ears.",
            "{=bab_nv_b_t2_04}Men here are not quick to uproot themselves.",
            "{=bab_nv_b_t2_05}Service is not given without need.",
            "{=bab_nv_b_t2_06}Men follow necessity, not impulse.",
            "{=bab_nv_b_t2_07}There is little haste in taking new oaths.",
            "{=bab_nv_b_t2_08}Men here prefer certainty.",
            "{=bab_nv_b_t2_09}Even known lords must wait.",
            "{=bab_nv_b_t2_10}The call to arms is not always answered at once."
        };
        static readonly List<string> Female_T0 = new()
        {
            "{=bab_nv_gh_t0_01}A woman calling for men? I thought I had heard everything.",
            "{=bab_nv_gh_t0_02}This is soldier’s work, not something for skirts and words.",
            "{=bab_nv_gh_t0_03}Men do not leave their homes for a woman’s ambition.",
            "{=bab_nv_gh_t0_04}You would have men die, yet you’ve never stood where they stand.",
            "{=bab_nv_gh_t0_05}War does not answer to soft hands, no matter how bold the tongue.",
            "{=bab_nv_gh_t0_06}I will not mock my people by sending them to a woman’s banner.",
            "{=bab_nv_gh_t0_07}Men follow strength — not novelty.",
            "{=bab_nv_gh_t0_08}This land has seen many fools, but few so misplaced.",
            "{=bab_nv_gh_t0_09}You should seek a hearth, not soldiers.",
            "{=bab_nv_gh_t0_10}No man here would shame himself by following you."
        }; 
        static readonly List<string> FemaleTier1 = new()
        {
            "{=bab_nv_gh_t1_01}Your name is spoken, but doubts remain — especially given who you are.",
            "{=bab_nv_gh_t1_02}Some find it… unusual, seeing a woman ask for men.",
            "{=bab_nv_gh_t1_03}Men hesitate when custom is bent too far.",
            "{=bab_nv_gh_t1_04}You are bold, I grant that, though many still question it.",
            "{=bab_nv_gh_t1_05}Not all are ready to serve beneath a woman’s command.",
            "{=bab_nv_gh_t1_06}This is no tale to stir men into following you.",
            "{=bab_nv_gh_t1_07}Such things invite talk — and caution.",
            "{=bab_nv_gh_t1_08}Men here weigh tradition heavily.",
            "{=bab_nv_gh_t1_09}Some find your presence… unsettling.",
        };
        static readonly List<string> FemaleTier2 = new()
        {
            "{=bab_nv_gh_t2_01}Not all customs bend easily, even for respected figures.",
            "{=bab_nv_gh_t2_02}You command respect but old habits die slowly.",
        };
        static readonly List<string> GangLeaderLines = new()
        {
            "{=bab_nv_gang_01}My boys don’t die for strangers.",
            "{=bab_nv_gang_02}Men here don’t march unless there’s blood or coin.",
            "{=bab_nv_gang_03}No one’s eager to trade turf for a battlefield.",
            "{=bab_nv_gang_04}This isn’t a charity for doomed causes.",
            "{=bab_nv_gang_05}Your banner doesn’t scare anyone yet.",
            "{=bab_nv_gang_06}We survive by staying put.",
            "{=bab_nv_gang_07}My men answer to me, not passing ambitions.",
            "{=bab_nv_gang_08}They know better than to follow promises.",
            "{=bab_nv_gang_09}War chews men up — smart ones stay here.",
            "{=bab_nv_gang_10}No volunteers. No fools."
        };
        static readonly List<string> MerchantLines= new()
        {
            "{=bab_nv_merch_01}Trade keeps men fed better than war.",
            "{=bab_nv_merch_02}Those with sense keep to their ledgers.",
            "{=bab_nv_merch_03}Risk is best avoided when profit is steady.",
            "{=bab_nv_merch_04}No one leaves honest work lightly.",
            "{=bab_nv_merch_05}Men here value coin over glory.",
            "{=bab_nv_merch_06}Uncertainty frightens investors — and families.",
            "{=bab_nv_merch_07}War disrupts markets, not inspires them.",
            "{=bab_nv_merch_08}There is little appetite for danger today.",
            "{=bab_nv_merch_09}Stability keeps men where they are.",
            "{=bab_nv_merch_10}No volunteers at present."
        };
        static readonly List<string> HeadmanLines = new()
        {
            "{=bab_nv_head_01}Our people cannot spare strong hands.",
            "{=bab_nv_head_02}Families come before banners.",
            "{=bab_nv_head_03}The fields need tending more than swords.",
            "{=bab_nv_head_04}I will not send sons away lightly.",
            "{=bab_nv_head_05}We have suffered enough already.",
            "{=bab_nv_head_06}War takes more than it gives.",
            "{=bab_nv_head_07}My duty is to those who live here.",
            "{=bab_nv_head_08}There are no men free to leave.",
            "{=bab_nv_head_09}Our village must endure.",
            "{=bab_nv_head_10}None will volunteer."
        };
        static readonly List<string> ArtisanLines = new()
        {
            "{=bab_nv_art_01}Skilled hands are worth more at the bench than the grave.",
            "{=bab_nv_art_02}Craft feeds families longer than war.",
            "{=bab_nv_art_03}Men here value skill over steel.",
            "{=bab_nv_art_04}No one abandons their trade lightly.",
            "{=bab_nv_art_05}War dulls talents better used elsewhere.",
            "{=bab_nv_art_06}Our work is our survival.",
            "{=bab_nv_art_07}Men here build — not destroy.",
            "{=bab_nv_art_08}There are no idle hands.",
            "{=bab_nv_art_09}Craft keeps men grounded.",
            "{=bab_nv_art_10}None wish to leave their work."
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
                    if (BaBSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(Female_T0);
                    break;

                case 1:
                    pool.AddRange(Suspicious_T1);
                    pool.AddRange(Brazen_T1);
                    pool.AddRange(SlightlyCourteous);
                    if (notable.IsGangLeader) pool.AddRange(GangLeaderLines);
                    if (notable.IsHeadman) pool.AddRange(HeadmanLines);
                    if (notable.IsMerchant) pool.AddRange(MerchantLines);
                    if (notable.IsArtisan) pool.AddRange(ArtisanLines);
                    if (BaBSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(FemaleTier1);
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
                    if (BaBSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(FemaleTier2);
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
