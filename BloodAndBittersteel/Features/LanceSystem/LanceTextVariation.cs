using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.LanceSystem
{
    public class LanceTextVariation
    {
        public static bool ChooseTextVariantWhenNotTakingLance()
        {
            string refusalText;
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0)
            {
                if (Hero.MainHero.IsFemale)
                    refusalText = "{=bab_lance_not_taken_cruel_female}They may linger in their unremarkable lives. I have no concern for their fate at present.";
                else refusalText = "{=bab_lance_not_taken__cruel_male}They are of no concern now. Stand idle, and tend to whatever small comforts you cling to.";
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 0 &&
                Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
            {
                refusalText = "{=bab_lance_refusal_valor_mercy}They are not required at present. Though I would see them prove their courage, I will not risk them unnecessarily.";
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
            {
                if (Hero.MainHero.IsFemale)
                    refusalText = "{=bab_lance_not_taken_mercy_female}They are not needed at present, and I am glad for it. Let them rest and tend their lives.";
                else refusalText = "{=bab_lance_not_taken_mercy_male}Their service is not required at this time. Let them remain ready for when duty calls.";
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 0)
            {
                refusalText = "{=bab_lance_not_taken_valor}Their service is not required now, though I would see them tested. Better they remain ready, for the field may call soon.";
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            refusalText = "{=bab_lance_not_taken_base}Their service is not required at present.";
            GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
            return true;
        }
        public static bool ChooseTextVariationWhenEnlistingLance()
        {
            string takeText;
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0 &&
                Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = "{=bab_lance_take_cruel_dishonor_female}I will take them. Let them be seen, feared, and remembered — villages burned and blood spilled will teach obedience where words will not.";
                takeText = "{=bab_lance_take_cruel_dishonor_male}I will take them. There will be plunder to claim and captives to take.";
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) < 0)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = "{=bab_lance_take_cautious_female}I will take their service, but they will not be spent lightly. I will answer for them myself.";
                else takeText = "{=bab_lance_take_cautious_male}I will take them, but they will not be thrown away without cause.";
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = "{=bab_lance_take_cruel_female}They will ride as I command. I care not for their comfort, only for their obedience.";
                else takeText = "{=bab_lance_take_cruel_male}“They will ride, whether they wish it or not.";
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
            {
                takeText = "{=bab_lance_take_devious}I claim their service. Ask no more of it.";
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 1 &&
                Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) > 1)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = "{=bab_lance_take_valor_honor_female}I will accept their service openly and in good faith. Let them ride where courage and duty call, under my banner.";
                else takeText = "{=bab_lance_take_valor_honor_male}I will accept their service in full. They shall ride where duty calls.";
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0 &&
                Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) > 0)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = "{=bab_lance_take_mercy_honor_female}I must take their service, though I would rather spare them. I will see that they are not wasted, nor led into death without cause, for their lives weigh heavily upon me.";
                else takeText = "{=bab_lance_take_mercy_honor_male}I will take their service and see that it is done with honor and care.";
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 0)
            {
                takeText = "{=bab_lance_take_valor}I will take their lances. They will ride where I ride.";
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = "{=bab_lance_take_mercy_female}I will take their service, but I will see that none are overworked or put in needless danger. Their well-being is my concern.";
                else takeText = "{=bab_lance_take_mercy_male}I will take their service, and ensure they are not sent to needless peril.";
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) > 0)
            {
                takeText = "{=bab_lance_take_honor}“I will take them into service, as duty requires.";
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            takeText = "{=bab_lance_take_base}I will accept their service. See that the men are made ready.";
            GameTexts.SetVariable("TAKE_TEXT", takeText);
            return true;
        }
        public static bool ChooseTextVariationWhenNotableRefusesToEnlistLance()
        {
            string refusalText = "";
            Hero notable = CharacterObject.OneToOneConversationCharacter.HeroObject;
            Clan ownerClan = Hero.MainHero.CurrentSettlement.OwnerClan;
            if (ownerClan == Clan.PlayerClan)
            {
                if (Hero.MainHero.CurrentSettlement.Town?.Loyalty < 20)
                {
                    refusalText = "{=bab_lance_refusal_loyalty_low}The town is restless, my {?PLAYER.GENDER}lady{?}lord{\\?}. Men whisper and watch one another. I will not arm them for war until order is restored.";
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (Hero.MainHero.CurrentSettlement.Town?.Security < 20)
                {
                    refusalText = "{=bab_lance_refusal_security_low}The roads are unsafe and the walls thinly watched. I dare not send fighting men away while danger still walks among us.";
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (Hero.MainHero.CurrentSettlement.Town?.FoodStocks < 10)
                {
                    refusalText = "{=bab_lance_refusal_food_low}There is scarcely bread enough for the town, my {?PLAYER.GENDER}lady{?}lord{\\?}. I will not take men from their ploughs while hunger stalks these streets.";
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (BaBSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale && notable.GetRelationWithPlayer() < 5
                    && Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 1)
                {
                    refusalText = "{=bab_lance_refusal_female_relation_valor}Your courage is spoken of, my lady, yet oaths are not sworn on courage alone. Stand with us longer, and this may change.";
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (BaBSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale && notable.GetRelationWithPlayer() < 15
                    && Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 1)
                {
                    refusalText = "{=bab_lance_refusal_female_relation_mercy}Your rule is a kind one, my lady. For that reason, I would not risk souring it by forcing men into service before their hearts are ready.";
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (BaBSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale && notable.GetRelationWithPlayer() < 20)
                {
                    refusalText = "{=bab_lance_refusal_female_relation}Men will not ride at my word for a woman they scarcely know. If you wish obedience, you must first command loyalty.";
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                var lance = Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>().GetNotableData(notable.StringId);
                if (lance.IsTaken)
                {
                    if (PartyBase.MainParty.Lances().Any(l => l.NotableId == lance.NotableId))
                        refusalText = "{=bab_lance_refusal_taken_self}My {?PLAYER.GENDER}lady{?}lord{\\?}, my banner already flies beneath yours. I cannot swear myself twice.";
                    else
                        refusalText = "{=bab_lance_refusal_taken_clan}My {?PLAYER.GENDER}lady{?}lord{\\?}, I am already bound by oath to your clan. My spear is spoken for.";
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (notable.GetRelationWithPlayer() < -5)
                {
                    refusalText = "{=bab_lance_refusal_relation}I do not yet trust you enough to place my life and men in your hands, my {?PLAYER.GENDER}lady{?}lord{\\?}.";
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (PartyBase.MainParty.Lances().Count > Campaign.Current.Models.LanceModel().MaxLancesForParty(PartyBase.MainParty).RoundedResultNumber)
                {
                    refusalText = "{=bab_lance_refusal_too_many_lances}Your warband can not take any more lances, my {?PLAYER.GENDER}lady{?}lord{\\?}.";
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                return false;
            }
            if (FactionManager.IsAtWarAgainstFaction(Clan.PlayerClan, ownerClan))
            {
                refusalText = "{=bab_lance_refusal_war}My lord, our lands stand opposed in blood and fire. I will not swear myself to one who is an enemy of my liege.";
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Clan.PlayerClan.Tier == 0)
            {
                if (BaBSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale) refusalText = "{=bab_lance_refusal_female_clantier0}A woman without land, speaking of banners and men? You shame yourself. Begone, before you are laughed from the gate.";
                else refusalText = "{=bab_lance_refusal_clantier0}Begone, whelp. Earn a name for yourself before you dare speak of banners and oaths.";
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Clan.PlayerClan.Tier == 1)
            {
                if (BaBSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale) refusalText = "{=bab_lance_refusal_female_clantier1}I have heard some talk of you, woman, but this is another man’s land. Take such speech of banners and men elsewhere.";
                else refusalText = "{=bab_lance_refusal_clantier1}You show spirit, but you are yet unproven. I must remain sworn to my rightful lord.";
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Clan.PlayerClan.Tier == 2)
            {
                if (BaBSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale) refusalText = "{=bab_lance_refusal_female_clantier2}I know who you are, yet you stand on another’s land and ask for men as though it were your own — liberties rarely granted, and less so to a woman.";
                else refusalText = "{=bab_lance_refusal_clantier2}Your name is known to me, my {?PLAYER.GENDER}lady{?}lord{\\?}, yet my oath binds me elsewhere. I cannot raise my banner for you.";
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Clan.PlayerClan.Tier >= 3)
            {
                refusalText = "{=bab_lance_refusal_clantier3}Were you lord of this land, my {?PLAYER.GENDER}lady{?}lord{\\?}, I would answer gladly. As it stands, my duty lies elsewhere.";
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            refusalText = "{=bab_lance_refusal_generic}I am not prepared to swear such an oath, my {?PLAYER.GENDER}lady{?}lord{\\?}. Perhaps another time.";
            GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
            return true;
        }
    }
}
