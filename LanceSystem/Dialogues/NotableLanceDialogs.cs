using LanceSystem.CampaignBehaviors;
using LanceSystem.LanceDataClasses;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace LanceSystem.Dialogues
{
    public static class NotableLanceDialogs
    {
        // Not taking lance texts
        private const string NotTakenCruelFemale = "{=lance_not_taken_cruel_female}They may linger in their unremarkable lives. I have no concern for their fate at present.";
        private const string NotTakenCruelMale = "{=lance_not_taken__cruel_male}They are of no concern now. Stand idle, and tend to whatever small comforts you cling to.";
        private const string RefusalValorMercy = "{=lance_refusal_valor_mercy}They are not required at present. Though I would see them prove their courage, I will not risk them unnecessarily.";
        private const string NotTakenMercyFemale = "{=lance_not_taken_mercy_female}They are not needed at present, and I am glad for it. Let them rest and tend their lives.";
        private const string NotTakenMercyMale = "{=lance_not_taken_mercy_male}Their service is not required at this time. Let them remain ready for when duty calls.";
        private const string NotTakenValor = "{=lance_not_taken_valor}Their service is not required now, though I would see them tested. Better they remain ready, for the field may call soon.";
        private const string NotTakenBase = "{=lance_not_taken_base}Their service is not required at present.";

        // Enlisting lance texts
        private const string TakeCruelDishonorFemale = "{=lance_take_cruel_dishonor_female}I will take them. Let them be seen, feared, and remembered — villages burned and blood spilled will teach obedience where words will not.";
        private const string TakeCruelDishonorMale = "{=lance_take_cruel_dishonor_male}I will take them. There will be plunder to claim and captives to take.";
        private const string TakeCautiousFemale = "{=lance_take_cautious_female}I will take their service, but they will not be spent lightly. I will answer for them myself.";
        private const string TakeCautiousMale = "{=lance_take_cautious_male}I will take them, but they will not be thrown away without cause.";
        private const string TakeCruelFemale = "{=lance_take_cruel_female}They will ride as I command. I care not for their comfort, only for their obedience.";
        private const string TakeCruelMale = "{=lance_take_cruel_male}“They will ride, whether they wish it or not.";
        private const string TakeDevious = "{=lance_take_devious}I claim their service. Ask no more of it.";
        private const string TakeValorHonorFemale = "{=lance_take_valor_honor_female}I will accept their service openly and in good faith. Let them ride where courage and duty call, under my banner.";
        private const string TakeValorHonorMale = "{=lance_take_valor_honor_male}I will accept their service in full. They shall ride where duty calls.";
        private const string TakeMercyHonorFemale = "{=lance_take_mercy_honor_female}I must take their service, though I would rather spare them. I will see that they are not wasted, nor led into death without cause, for their lives weigh heavily upon me.";
        private const string TakeMercyHonorMale = "{=lance_take_mercy_honor_male}I will take their service and see that it is done with honor and care.";
        private const string TakeValor = "{=lance_take_valor}I will take their lances. They will ride where I ride.";
        private const string TakeMercyFemale = "{=lance_take_mercy_female}I will take their service, but I will see that none are overworked or put in needless danger. Their well-being is my concern.";
        private const string TakeMercyMale = "{=lance_take_mercy_male}I will take their service, and ensure they are not sent to needless peril.";
        private const string TakeHonor = "{=lance_take_honor}“I will take them into service, as duty requires.";
        private const string TakeBase = "{=lance_take_base}I will accept their service. See that the men are made ready.";

        // Notable refusal texts
        private const string RefusalLoyaltyLow = "{=lance_refusal_loyalty_low}The town is restless, my {?PLAYER.GENDER}lady{?}lord{\\?}. Men whisper and watch one another. I will not arm them for war until order is restored.";
        private const string RefusalSecurityLow = "{=lance_refusal_security_low}The roads are unsafe and the walls thinly watched. I dare not send fighting men away while danger still walks among us.";
        private const string RefusalFoodLow = "{=lance_refusal_food_low}There is scarcely bread enough for the town, my {?PLAYER.GENDER}lady{?}lord{\\?}. I will not take men from their ploughs while hunger stalks these streets.";
        private const string RefusalFemaleRelationValor = "{=lance_refusal_female_relation_valor}Your courage is spoken of, my lady, yet oaths are not sworn on courage alone. Stand with us longer, and this may change.";
        private const string RefusalFemaleRelationMercy = "{=lance_refusal_female_relation_mercy}Your rule is a kind one, my lady. For that reason, I would not risk souring it by forcing men into service before their hearts are ready.";
        private const string RefusalFemaleRelation = "{=lance_refusal_female_relation}Men will not ride at my word for a woman they scarcely know. If you wish obedience, you must first command loyalty.";
        private const string RefusalTakenSelf = "{=lance_refusal_taken_self}My {?PLAYER.GENDER}lady{?}lord{\\?}, my banner already flies beneath yours. I cannot swear myself twice.";
        private const string RefusalTakenClan = "{=lance_refusal_taken_clan}My {?PLAYER.GENDER}lady{?}lord{\\?}, I am already bound by oath to your clan. My spear is spoken for.";
        private const string RefusalRelation = "{=lance_refusal_relation}I do not yet trust you enough to place my life and men in your hands, my {?PLAYER.GENDER}lady{?}lord{\\?}.";
        private const string RefusalTooManyLances = "{=lance_refusal_too_many_lances}Your warband can not take any more lances, my {?PLAYER.GENDER}lady{?}lord{\\?}.";
        private const string RefusalWar = "{=lance_refusal_war}My lord, our lands stand opposed in blood and fire. I will not swear myself to one who is an enemy of my liege.";
        private const string RefusalFemaleClanTier0 = "{=lance_refusal_female_clantier0}A woman without land, speaking of banners and men? You shame yourself. Begone, before you are laughed from the gate.";
        private const string RefusalClanTier0 = "{=lance_refusal_clantier0}Begone, whelp. Earn a name for yourself before you dare speak of banners and oaths.";
        private const string RefusalFemaleClanTier1 = "{=lance_refusal_female_clantier1}I have heard some talk of you, woman, but this is another man’s land. Take such speech of banners and men elsewhere.";
        private const string RefusalClanTier1 = "{=lance_refusal_clantier1}You show spirit, but you are yet unproven. I must remain sworn to my rightful lord.";
        private const string RefusalFemaleClanTier2 = "{=lance_refusal_female_clantier2}I know who you are, yet you stand on another’s land and ask for men as though it were your own — liberties rarely granted, and less so to a woman.";
        private const string RefusalClanTier2 = "{=lance_refusal_clantier2}Your name is known to me, my {?PLAYER.GENDER}lady{?}lord{\\?}, yet my oath binds me elsewhere. I cannot raise my banner for you.";
        private const string RefusalClanTier3 = "{=lance_refusal_clantier3}Were you lord of this land, my {?PLAYER.GENDER}lady{?}lord{\\?}, I would answer gladly. As it stands, my duty lies elsewhere.";
        private const string RefusalGeneric = "{=lance_refusal_generic}I am not prepared to swear such an oath, my {?PLAYER.GENDER}lady{?}lord{\\?}. Perhaps another time.";

        public static bool ChooseTextVariantWhenNotTakingLance()
        {
            string refusalText;
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0)
            {
                if (Hero.MainHero.IsFemale)
                    refusalText = NotTakenCruelFemale;
                else refusalText = NotTakenCruelMale;
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 0 &&
                Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
            {
                refusalText = RefusalValorMercy;
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
            {
                if (Hero.MainHero.IsFemale)
                    refusalText = NotTakenMercyFemale;
                else refusalText = NotTakenMercyMale;
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 0)
            {
                refusalText = NotTakenValor;
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            refusalText = NotTakenBase;
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
                    takeText = TakeCruelDishonorFemale;
                else takeText = TakeCruelDishonorMale;
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) < 0)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = TakeCautiousFemale;
                else takeText = TakeCautiousMale;
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = TakeCruelFemale;
                else takeText = TakeCruelMale;
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) < 0)
            {
                takeText = TakeDevious;
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 1 &&
                Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) > 1)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = TakeValorHonorFemale;
                else takeText = TakeValorHonorMale;
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0 &&
                Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) > 0)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = TakeMercyHonorFemale;
                else takeText = TakeMercyHonorMale;
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 0)
            {
                takeText = TakeValor;
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 0)
            {
                if (Hero.MainHero.IsFemale)
                    takeText = TakeMercyFemale;
                else takeText = TakeMercyMale;
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            if (Hero.MainHero.GetTraitLevel(DefaultTraits.Honor) > 0)
            {
                takeText = TakeHonor;
                GameTexts.SetVariable("TAKE_TEXT", takeText);
                return true;
            }
            takeText = TakeBase;
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
                    refusalText = RefusalLoyaltyLow;
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (Hero.MainHero.CurrentSettlement.Town?.Security < 20)
                {
                    refusalText = RefusalSecurityLow;
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (Hero.MainHero.CurrentSettlement.Town?.FoodStocks < 10)
                {
                    refusalText = RefusalFoodLow;
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (LanceSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale && notable.GetRelationWithPlayer() < 5
                    && Hero.MainHero.GetTraitLevel(DefaultTraits.Valor) > 1
                    && !notable.CurrentSettlement.IsCastle)
                {
                    refusalText = RefusalFemaleRelationValor;
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (LanceSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale && notable.GetRelationWithPlayer() < 15
                    && Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) > 1
                    && !notable.CurrentSettlement.IsCastle)
                {
                    refusalText = RefusalFemaleRelationMercy;
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (LanceSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale && notable.GetRelationWithPlayer() < 20
                    && !notable.CurrentSettlement.IsCastle)
                {
                    refusalText = RefusalFemaleRelation;
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                var lance = Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>().GetNotableData(notable.StringId);
                if (lance.IsTaken)
                {
                    if (PartyBase.MainParty.Lances().Any(l => l is NotableLanceData nl &&  nl.NotableId == lance.NotableId))
                        refusalText = RefusalTakenSelf;
                    else
                        refusalText = RefusalTakenClan;
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (notable.GetRelationWithPlayer() < -5)
                {
                    refusalText = RefusalRelation;
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                if (PartyBase.MainParty.Lances().Count >= Campaign.Current.Models.LanceModel().MaxLancesForParty(PartyBase.MainParty).RoundedResultNumber)
                {
                    refusalText = RefusalTooManyLances;
                    GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                    return true;
                }
                return false;
            }
            if (FactionManager.IsAtWarAgainstFaction(Clan.PlayerClan, ownerClan))
            {
                refusalText = RefusalWar;
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Clan.PlayerClan.Tier == 0)
            {
                if (LanceSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale) refusalText = RefusalFemaleClanTier0;
                else refusalText = RefusalClanTier0;
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Clan.PlayerClan.Tier == 1)
            {
                if (LanceSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale) refusalText = RefusalFemaleClanTier1;
                else refusalText = RefusalClanTier1;
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Clan.PlayerClan.Tier == 2)
            {
                if (LanceSettings.Instance.FemalePrejudice && Hero.MainHero.IsFemale) refusalText = RefusalFemaleClanTier2;
                else refusalText = RefusalClanTier2;
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            if (Clan.PlayerClan.Tier >= 3)
            {
                refusalText = RefusalClanTier3;
                GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
                return true;
            }
            refusalText = RefusalGeneric;
            GameTexts.SetVariable("REFUSAL_TEXT", refusalText);
            return true;
        }
    }
}
