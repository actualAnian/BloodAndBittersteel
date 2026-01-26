using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace LanceSystem.Dialogues
{
    public class MercenaryLanceDialogs
    {
        static readonly List<string> MercGreeting_T0_Dismissive = new()
        {
            "{=lance_merc_t0_d_01}You’re blocking the light.",
            "{=lance_merc_t0_d_02}This seat’s taken.",
            "{=lance_merc_t0_d_03}I’m not in the mood for strangers.",
            "{=lance_merc_t0_d_04}Move along.",
            "{=lance_merc_t0_d_05}Find another table.",
            "{=lance_merc_t0_d_05}This isn’t a social table.",
        };
        static readonly List<string> MercGreeting_T0_Male = new()
        {
            "{=lance_merc_t0_m_01}If you’re here for drink, order it.",
            "{=lance_merc_t0_m_02}I don’t know you.",
            "{=lance_merc_t0_m_03}I’m resting. Don’t interrupt.",
        };
        static readonly List<string> MercGreeting_T0_Female = new()
        {
            "{=lance_merc_t0_f_01}I didn’t order another serving girl.",
            "{=lance_merc_t0_f_02}If you’re here to pour ale, you’re late.",
            "{=lance_merc_t0_f_03}This table’s for soldiers, not tavern girls.",
            "{=lance_merc_t0_f_04}Run along. I’m waiting for someone with coin.",
            "{=lance_merc_t0_f_05}Unless you’re bringing drink, you’re in the wrong place."
        };
        static readonly List<string> MercGreeting_T1_Male = new()
        {
            "{=lance_merc_t1_m_01}You looking for swords, or just talk?",
            "{=lance_merc_t1_m_02}I don’t recognize you, but I’ll remember the face.",
            "{=lance_merc_t1_m_03}You carry yourself like someone with plans.",
            "{=lance_merc_t1_m_04}What brings you to my table?",
            "{=lance_merc_t1_m_05}If there’s work, I’m listening."
        };
        static readonly List<string> MercGreeting_T1_Female = new()
        {
            "{=lance_merc_t1_f_01}Didn’t expect you to be the one asking.",
            "{=lance_merc_t1_f_02}You don’t look like the usual sort.",
            "{=lance_merc_t1_f_03}That’s a steady gaze for a noisy hall.",
            "{=lance_merc_t1_f_04}People don’t usually come here without a reason.",
            "{=lance_merc_t1_f_05}Well… speak, then."
        };
        static readonly List<string> MercGreeting_T2Plus = new()
        {
            "{=lance_merc_t2_01}Are you looking for mercenaries, my {?PLAYER.GENDER}lady{?}lord{\\?}?",
            "{=lance_merc_t2_02}If you’re hiring, you’ve found willing steel.",
            "{=lance_merc_t2_03}My company’s between contracts — for now.",
            "{=lance_merc_t2_04}I’ve heard you know how to keep men paid.",
            "{=lance_merc_t2_05}I’m looking for work worthy of my blades.",
            "{=lance_merc_t2_06}You have my ear, my lord. Speak your business."
        };
        static readonly List<string> MercGreeting_T2Plus_Owner = new()
        {
            "{=lance_merc_t2o_01}My {?PLAYER.GENDER}lady{?}lord{\\?} — your hall keeps a fine tavern.",
            "{=lance_merc_t2o_02}An honor to meet the ruler of this place.",
            "{=lance_merc_t2o_03}Your coin spends true in this town, they say.",
            "{=lance_merc_t2o_04}I’d serve gladly, if you have need.",
            "{=lance_merc_t2o_05}A ruler who drinks among their people earns notice.",
            "{=lance_merc_t2o_06}Your presence carries weight here.",
        };
        const string MercFinishedBusiness_T0 = "{=lance_merc_finished_t0}Be gone. You’re not worth the breath.";
        const string MercFinishedBusiness_female_T0 = "{=lance_merc_finished_t0_f}Off with you. Find a broom or a bed.";
        const string MercFinishedBusiness_T0_Lingering = "{=lance_merc_finished_t0_lingering}Another word, and you’ll be tasting the floorboards.";
        const string MercFinishedBusiness_female_T0_Lingering = "{=lance_merc_finished_t0_f_lingering}Move, girl, before I have you dragged out by the hair.";
        const string MercFinishedBusiness_T1 = "{=lance_merc_finished_t1}We’ll see how this works out.";
        const string MercFinishedBusiness_T2 = "{=lance_merc_finished_t2}I look forward to our next venture.";
        public static string GetFinishedBusinessDialog(int amountAsked)
        {
            switch (Hero.MainHero.Clan.Tier)
            {
                case 0:
                    if (LanceSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale) 
                        return amountAsked >=5? MercFinishedBusiness_female_T0_Lingering : MercFinishedBusiness_female_T0;
                    else return amountAsked >= 5 ? MercFinishedBusiness_T0_Lingering : MercFinishedBusiness_T0;
                case 1:
                    return MercFinishedBusiness_T1;
                default:
                    return MercFinishedBusiness_T2;
            }
        }
        public static string GetGreetingDialogs(Settlement settlement)
        {
            List<string> pool = new();
            bool isFemale = Hero.MainHero.IsFemale;
            switch (Hero.MainHero.Clan.Tier)
            {
                case 0:
                    pool.AddRange(MercGreeting_T0_Dismissive);
                    pool.AddRange(MercGreeting_T0_Male);
                    if (LanceSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(MercGreeting_T0_Female);
                    break;
                case 1:
                    pool.AddRange(MercGreeting_T1_Male);
                    if (LanceSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(MercGreeting_T1_Female);
                    break;
                case 2:
                    pool.AddRange(MercGreeting_T2Plus);
                    pool.AddRange(MercGreeting_T2Plus_Owner);
                    break;
                default:
                    if (settlement.OwnerClan == Clan.PlayerClan)
                        pool.AddRange(MercGreeting_T2Plus_Owner);
                    pool.AddRange(MercGreeting_T2Plus);
                    break;
            }
            return pool.GetRandomElement();
        }

        // hiring
        static readonly List<string> MercHire_T0_Male = new()
        {
            "{=lance_merc_hire_t0_m_01}You’re in no position to hire men like us.",
            "{=lance_merc_hire_t0_m_02}Come back when your name carries weight.",
            "{=lance_merc_hire_t0_m_03}I don’t take contracts from nobodies.",
            "{=lance_merc_hire_t0_m_04}Try your luck elsewhere.",
            "{=lance_merc_hire_t0_m_05}We don’t follow dreams, only coin and reputation."
        };
        static readonly List<string> MercHire_T0_Female = new()
        {
            "{=lance_merc_hire_t0_f_01}That’s a good joke. Now go pour ale.",
            "{=lance_merc_hire_t0_f_02}Hiring mercenaries? Do your job as a wench.",
            "{=lance_merc_hire_t0_f_03}You think men like us answer to that?",
            "{=lance_merc_hire_t0_f_04}Run along, this isn’t playacting.",
            "{=lance_merc_hire_t0_f_05}Hah, I’ve heard better offers shouted across a bar."
        };
        static readonly List<string> MercHire_T1_Male = new()
        {
            "{=lance_merc_hire_t1_m_01}If the coin’s real, I’ll listen {TOTAL_AMOUNT}{GOLD_ICON} in advance.",
            "{=lance_merc_hire_t1_m_02}Gold spends the same everywhere. {TOTAL_AMOUNT}{GOLD_ICON}, paid up front.",
            "{=lance_merc_hire_t1_m_03}I don’t know how long you’ll last, but the price is {TOTAL_AMOUNT}{GOLD_ICON}.",
            "{=lance_merc_hire_t1_m_04}Pay {TOTAL_AMOUNT}{GOLD_ICON}, and my men will march.",
            "{=lance_merc_hire_t1_m_05}I’ll take the contract for {TOTAL_AMOUNT}{GOLD_ICON}, no delays."
        };
        static readonly List<string> MercHire_T1_Female = new()
        {
            "{=lance_merc_hire_t1_f_01}If you truly have {TOTAL_AMOUNT}{GOLD_ICON}, we’ll see.",
            "{=lance_merc_hire_t1_f_02}I’ve doubts, but gold is gold. {TOTAL_AMOUNT}{GOLD_ICON}, in advance.",
            "{=lance_merc_hire_t1_f_03}Men will follow coin, not appearances. {TOTAL_AMOUNT}{GOLD_ICON}.",
            "{=lance_merc_hire_t1_f_04}I hope you understand what you’re buying. {TOTAL_AMOUNT}{GOLD_ICON}."
        };
        static readonly List<string> MercHire_T2Plus = new()
        {
            "{=lance_merc_hire_t2_01}Agreed. {TOTAL_AMOUNT}{GOLD_ICON} in advance, and my band is yours.",
            "{=lance_merc_hire_t2_02}Those are fair terms. Pay {TOTAL_AMOUNT}{GOLD_ICON}, and we ride.",
            "{=lance_merc_hire_t2_03}The contract is simple, {TOTAL_AMOUNT}{GOLD_ICON}, paid now.",
            "{=lance_merc_hire_t2_04}We’ll serve under your banner for {TOTAL_AMOUNT}{GOLD_ICON}.",
            "{=lance_merc_hire_t2_05}Gold spends the same in every kingdom. {TOTAL_AMOUNT}{GOLD_ICON}, and we’re sworn."
        };
        static readonly List<string> MercHire_T3Plus_Respectful = new()
        {
            "{=lance_merc_hire_t2r_01}Your reputation precedes you. {TOTAL_AMOUNT}{GOLD_ICON}, and we are at your command.",
            "{=lance_merc_hire_t2r_02}An honor to take the contract. {TOTAL_AMOUNT}{GOLD_ICON}, as agreed.",
            "{=lance_merc_hire_t2r_03}My men will follow you. {TOTAL_AMOUNT}{GOLD_ICON}, paid in advance.",
            "{=lance_merc_hire_t2r_04}We accept. {TOTAL_AMOUNT}{GOLD_ICON}, and our blades are yours.",
            "{=lance_merc_hire_t2r_05}Terms accepted. {TOTAL_AMOUNT}{GOLD_ICON}, and the contract is sealed."
        };
        public static string GetHireDismissalDialog()
        {
            List<string> pool = new();
            bool isFemale = Hero.MainHero.IsFemale;
            pool.AddRange(MercHire_T0_Male);
            if (LanceSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(MercHire_T0_Female);
            return pool.GetRandomElement();
        }
        public static string GetHireAgreeDialog()
        {
            List<string> pool = new();
            bool isFemale = Hero.MainHero.IsFemale;
            switch (Hero.MainHero.Clan.Tier)
            {
                case 1:
                    pool.AddRange(MercHire_T1_Male);
                    if (LanceSettings.Instance.FemalePrejudice && isFemale) pool.AddRange(MercHire_T1_Female);
                    break;
                case 2:
                    pool.AddRange(MercHire_T2Plus);
                    break;
                default:
                    pool.AddRange(MercHire_T2Plus);
                    pool.AddRange(MercHire_T3Plus_Respectful);
                    break;
            }
            return pool.GetRandomElement();
        }
        static readonly List<string> MercRenew_T1 = new()
        {
            "{=lance_merc_renew_t1_m_01}Our time’s up. If you want us to stay, it’ll cost {TOTAL_AMOUNT}{GOLD_ICON}, paid now.",
            "{=lance_merc_renew_t1_m_02}The contract’s ended. Renew it for {TOTAL_AMOUNT}{GOLD_ICON}, or we part ways.",
            "{=lance_merc_renew_t1_m_03}My men won’t ride on old terms. {TOTAL_AMOUNT}{GOLD_ICON} decides it."
        };
        static readonly List<string> MercRenew_T2 = new()
        {
            "{=lance_merc_renew_t2_m_01}The contract has ended. Renewal stands at {TOTAL_AMOUNT}{GOLD_ICON}.",
            "{=lance_merc_renew_t2_m_02}My men have served well. For {TOTAL_AMOUNT}{GOLD_ICON}, they will continue gladly.",
            "{=lance_merc_renew_t2_m_03}My company remains available for {TOTAL_AMOUNT}{GOLD_ICON}."
        };
        static readonly List<string> MercRenew_Unpaid_T1 = new()
        {
            "{=lance_merc_renew_u_t1_m_01}You paid late, or not at all. Double the price, {TOTAL_AMOUNT}{GOLD_ICON} or we’ll take our due ",
            "{=lance_merc_renew_u_t1_m_02}My men remember empty purses. Pay double, {TOTAL_AMOUNT}{GOLD_ICON}, up front.",
            "{=lance_merc_renew_u_t1_m_03}We don’t forget debt. Pay double, {TOTAL_AMOUNT}{GOLD_ICON} or things will get unpleasant.",
        };
        static readonly List<string> MercRenew_Unpaid_T2 = new()
        {
            "{=lance_merc_renew_u_t2_m_01}Your delays were noted. Renewal now requires {TOTAL_AMOUNT}{GOLD_ICON} — doubled.",
            "{=lance_merc_renew_u_t2_m_02}Trust must be bought again. Double pay, {TOTAL_AMOUNT}{GOLD_ICON}, up front.",
            "{=lance_merc_renew_u_t2_f_01}My {?PLAYER.GENDER}lady{?}lord{\\?}, we missed our wages. We demand double pay, {TOTAL_AMOUNT}{GOLD_ICON} for the contract renewal.",
        };
        public static string GetHireAgreeDialog(bool hasMissedWages)
        {
            List<string> pool = new();
            switch (Hero.MainHero.Clan.Tier)
            {
                case 1:
                    if (hasMissedWages) pool.AddRange(MercRenew_Unpaid_T1);
                    else pool.AddRange(MercRenew_T1);
                    break;
                default:
                    if (hasMissedWages) pool.AddRange(MercRenew_Unpaid_T2);
                    else pool.AddRange(MercRenew_T2);
                    break;
            }
            return pool.GetRandomElement();
        }

        static readonly List<string> MercRebel_GoToThreat = new()
        {
            "{=lancel_merc_rebell_explain_01}You let our pay lapse, yet now you speak of ending the contract. Don’t mistake yourself for untouchable, {?PLAYER.GENDER}lady{?}lord{\\?}. We’ll strip you and your followers of everything worth coin, and sell you off like spoiled plunder.",
            "{=lancel_merc_rebell_explain_02}My men marched without wages, and you would dismiss us as if nothing were owed. No  pay means no loyalty. We take your horses, your arms, and your banners — then we sell you like any other broken commander.",
            "{=lancel_merc_rebell_explain_03}You broke the terms first, not us. You think rank protects you? It doesn’t. We’ll empty your packs, divide your gear, and put you on the block before nightfall",
        };
        public static string GetThreatExplanationDialog()
        {
            List<string> pool = new();
            pool.AddRange(MercRebel_GoToThreat);
            return pool.GetRandomElement();
        }

        static readonly List<string> MercRebel_T1 = new()
        {
            "{=lance_merc_rebel_t1_01}You forget who’s armed here — and who isn’t.",
            "{=lance_merc_rebel_t1_02}My men didn’t march for promises. We’ll take our due one way or another.",
            "{=lance_merc_rebel_t1_03}Refuse us, and you won’t be the one choosing how this ends."
        };
        static readonly List<string> MercRebel_T1_Female = new()
        {
            "{=lance_merc_rebel_t1_f}Refuse us, and you’ll regret being so alone among soldiers."
        };
        static readonly List<string> MercRebel_T2 = new()
        {
            "{=lance_merc_rebel_t2_01}This need not become ugly, if wisdom prevails.",
            "{=lance_merc_rebel_t2_02}This path ends poorly for everyone. Choose carefully.",
        };
        public static string GetMutinyDialog()
        {
            List<string> pool = new();
            switch (Hero.MainHero.Clan.Tier)
            {
                case 1:
                    pool.AddRange(MercRebel_T1);
                    if (LanceSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale)
                        pool.AddRange(MercRebel_T1_Female);
                    break;
                default:
                    pool.AddRange(MercRebel_T2);
                    break;
            }
            return pool.GetRandomElement();
        }
    }
}