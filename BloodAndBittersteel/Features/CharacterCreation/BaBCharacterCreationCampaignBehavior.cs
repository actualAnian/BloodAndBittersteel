using System.Collections.Generic;
using System.Linq;
using BloodAndBittersteel.Features.CharacterCreation.Cultures;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.CharacterCreation;

public class BaBCharacterCreationCampaignBehavior : CampaignBehaviorBase, ICharacterCreationContentHandler
{
    private readonly List<ICharacterCreationCulture> _cultures = new();
    private readonly HashSet<string> _registeredOptionIds = new();

    private readonly IReadOnlyDictionary<string, string> _occupationToEquipmentMapping = new Dictionary<string, string>
    {
        { CharacterOccupations.Retainer, "retainer" },
        { CharacterOccupations.Bard, "bard" },
        { CharacterOccupations.Hunter, "hunter" },
        { CharacterOccupations.Farmer, "farmer" },
        { CharacterOccupations.Herder, "herder" },
        { CharacterOccupations.Healer, "healer" },
        { CharacterOccupations.Mercenary, "mercenary" },
        { CharacterOccupations.Infantry, "infantry" },
        { CharacterOccupations.Skirmisher, "skirmisher" },
        { CharacterOccupations.Kern, "kern" },
        { CharacterOccupations.Guard, "guard" },
        { CharacterOccupations.RetainerUrban, "retainer" },
        { CharacterOccupations.MercenaryUrban, "mercenary" },
        { CharacterOccupations.MerchantUrban, "merchant" },
        { CharacterOccupations.VagabondUrban, "vagabond" },
        { CharacterOccupations.ArtisanUrban, "artisan" },
        { CharacterOccupations.PhysicianUrban, "physician" },
        { CharacterOccupations.HealerUrban, "healer" },
        { CharacterOccupations.BardUrban, "bard" },
        { "noble", "noble" },
        { "cavalry", "cavalry" },
        { "bandit", "bandit" }
    };

    public override void RegisterEvents()
    {
        CampaignEvents.OnCharacterCreationInitializedEvent.AddNonSerializedListener(this, OnCharacterCreationInitialized);
    }

    public override void SyncData(IDataStore dataStore) { }

    private void OnCharacterCreationInitialized(CharacterCreationManager characterCreationManager)
    {
        characterCreationManager.CharacterCreationContent.DefaultSelectedTitleType = "guard";
        characterCreationManager.RegisterCharacterCreationContentHandler(this, 800);
    }

    void ICharacterCreationContentHandler.InitializeContent(CharacterCreationManager characterCreationManager)
    {
        _registeredOptionIds.Clear();
        characterCreationManager.CharacterCreationContent.AddEquipmentToUseGetter(delegate(string occupationId, out string equipmentId)
        {
            return _occupationToEquipmentMapping.TryGetValue(occupationId, out equipmentId);
        });
        RegisterCultures();
        InitializeCharacterCreationStages(characterCreationManager);
        InitializeCharacterCreationCultures(characterCreationManager);
        var upgrades = new NarrativeSkillUpgrades(characterCreationManager);
        characterCreationManager.CharacterCreationContent.ChangeReviewPageDescription(new TextObject("{=W6pKpEoT}You prepare to set off for a grand adventure in Calradia! Here is your character. Continue if you are ready, or go back to make changes."));
        AddParentsMenu(characterCreationManager, upgrades);
        AddChildhoodMenu(characterCreationManager, upgrades);
        AddEducationMenu(characterCreationManager, upgrades);
        AddYouthMenu(characterCreationManager, upgrades);
        AddAdulthoodMenu(characterCreationManager, upgrades);
        AddAgeSelectionMenu(characterCreationManager, upgrades);
    }

    void ICharacterCreationContentHandler.AfterInitializeContent(CharacterCreationManager characterCreationManager) { }

    void ICharacterCreationContentHandler.OnStageCompleted(CharacterCreationStageBase stage)
    {
        if (stage is CharacterCreationFaceGeneratorStage)
            FaceGenUpdated();
    }

    void ICharacterCreationContentHandler.OnCharacterCreationFinalize(CharacterCreationManager characterCreationManager) { }

    private void RegisterCultures()
    {
        _cultures.Clear();
        _cultures.Add(new VlandianCulture());
        _cultures.Add(new CrownlanderCulture());
        _cultures.Add(new ValemanCulture());
        _cultures.Add(new StormlanderCulture());
        _cultures.Add(new DornishCulture());
        _cultures.Add(new ReachmanCulture());
        _cultures.Add(new WesterlanderCulture());
        _cultures.Add(new RiverlanderCulture());
        _cultures.Add(new IronIslanderCulture());
        _cultures.Add(new NorthmanCulture());
    }

    private void InitializeCharacterCreationStages(CharacterCreationManager characterCreationManager)
    {
        characterCreationManager.AddStage(new CharacterCreationCultureStage());
        characterCreationManager.AddStage(new CharacterCreationFaceGeneratorStage());
        characterCreationManager.AddStage(new CharacterCreationNarrativeStage());
        characterCreationManager.AddStage(new CharacterCreationBannerEditorStage());
        characterCreationManager.AddStage(new CharacterCreationClanNamingStage());
        characterCreationManager.AddStage(new CharacterCreationReviewStage());
        characterCreationManager.AddStage(new CharacterCreationOptionsStage());
    }

    private void InitializeCharacterCreationCultures(CharacterCreationManager characterCreationManager)
    {
        foreach (var objectType in _cultures)
        {
            var culture = MBObjectManager.Instance.GetObject<CultureObject>(objectType.CultureId);
            if (culture == null) 
            {
                InformationManager.DisplayMessage(new($"ERROR, no culture with id {objectType.CultureId}", new Color(1, 0, 0)));
                continue;
            }
            characterCreationManager.CharacterCreationContent.AddCharacterCreationCulture(culture, 1, 10);
        }
    }

    public void FaceGenUpdated()
    {
        CharacterCreationManager characterCreationManager = (GameStateManager.Current.ActiveState as CharacterCreationState).CharacterCreationManager;
        BodyProperties motherBodyProperties;
        BodyProperties fatherBodyProperties;
        FaceGen.GenerateParentKey(fatherBodyProperties = (motherBodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment)), CharacterObject.PlayerCharacter.Race, ref motherBodyProperties, ref fatherBodyProperties);
        motherBodyProperties = new BodyProperties(new DynamicBodyProperties(33f, 0.3f, 0.2f), motherBodyProperties.StaticProperties);
        fatherBodyProperties = new BodyProperties(new DynamicBodyProperties(33f, 0.5f, 0.5f), fatherBodyProperties.StaticProperties);
        foreach (NarrativeMenu narrativeMenu in characterCreationManager.NarrativeMenus)
        {
            foreach (NarrativeMenuCharacter character in narrativeMenu.Characters)
            {
                if (character.StringId.Equals("mother_character"))
                    character.UpdateBodyProperties(motherBodyProperties, CharacterObject.PlayerCharacter.Race, isFemale: true);
                if (character.StringId.Equals("father_character"))
                    character.UpdateBodyProperties(fatherBodyProperties, CharacterObject.PlayerCharacter.Race, isFemale: false);
                if (character.StringId.Equals("player_childhood_character") || character.StringId.Equals("player_education_character") || character.StringId.Equals("player_youth_character") || character.StringId.Equals("player_adulthood_character") || character.StringId.Equals("player_age_selection_character"))
                    character.UpdateBodyProperties(CharacterObject.PlayerCharacter.GetBodyProperties(null), CharacterObject.PlayerCharacter.Race, isFemale: false);
            }
        }
    }

    private void AddParentsMenu(CharacterCreationManager characterCreationManager, NarrativeSkillUpgrades upgrades)
    {
        List<NarrativeMenuCharacter> list = new List<NarrativeMenuCharacter>();
        BodyProperties motherBodyProperties;
        BodyProperties fatherBodyProperties;
        FaceGen.GenerateParentKey(fatherBodyProperties = (motherBodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment)), CharacterObject.PlayerCharacter.Race, ref motherBodyProperties, ref fatherBodyProperties);
        motherBodyProperties = new BodyProperties(new DynamicBodyProperties(33f, 0.3f, 0.2f), motherBodyProperties.StaticProperties);
        fatherBodyProperties = new BodyProperties(new DynamicBodyProperties(33f, 0.5f, 0.5f), fatherBodyProperties.StaticProperties);
        var motherCharacter = new NarrativeMenuCharacter("mother_character", motherBodyProperties, CharacterObject.PlayerCharacter.Race, isFemale: true);
        list.Add(motherCharacter);
        var fatherCharacter = new NarrativeMenuCharacter("father_character", fatherBodyProperties, CharacterObject.PlayerCharacter.Race, isFemale: false);
        list.Add(fatherCharacter);
        var narrativeMenu = new NarrativeMenu("narrative_parent_menu", "start", "narrative_childhood_menu", new TextObject("{=b4lDDcli}Family"), new TextObject("{=XgFU1pCx}You were born into a family of..."), list, GetParentMenuNarrativeMenuCharacterArgs);
        foreach (ICharacterCreationCulture culture in _cultures)
        {
            foreach (NarrativeOption option in culture.GetParentOptions())
            {
                if (_registeredOptionIds.Add(option.OptionId))
                    narrativeMenu.AddNarrativeMenuOption(CreateVanillaOption(option, upgrades));
            }
        }
        characterCreationManager.AddNewMenu(narrativeMenu);
    }

    private List<NarrativeMenuCharacterArgs> GetParentMenuNarrativeMenuCharacterArgs(CultureObject culture, string occupationType, CharacterCreationManager characterCreationManager)
    {
        return new List<NarrativeMenuCharacterArgs>
        {
            new("mother_character", 33, "mother_char_creation_none_" + characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, "act_character_creation_female_default_standing", "spawnpoint_player_1", "", "", null, isHuman: true, isFemale: true),
            new("father_character", 33, "father_char_creation_none_" + characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, "act_character_creation_male_default_standing", "spawnpoint_player_1")
        };
    }

    private void AddChildhoodMenu(CharacterCreationManager characterCreationManager, NarrativeSkillUpgrades upgrades)
    {
        var list = new List<NarrativeMenuCharacter>();
        BodyProperties originalBodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment);
        originalBodyProperties = FaceGen.GetBodyPropertiesWithAge(ref originalBodyProperties, 7f);
        var playerChildhoodCharacter = new NarrativeMenuCharacter("player_childhood_character", originalBodyProperties, CharacterObject.PlayerCharacter.Race, CharacterObject.PlayerCharacter.IsFemale);
        list.Add(playerChildhoodCharacter);
        var narrativeMenu = new NarrativeMenu("narrative_childhood_menu", "narrative_parent_menu", "narrative_education_menu", new TextObject("{=8Yiwt1z6}Early Childhood"), new TextObject("{=character_creation_content_16}As a child you were noted for..."), list, GetChildhoodMenuNarrativeMenuCharacterArgs);
        foreach (ICharacterCreationCulture culture in _cultures)
        {
            foreach (NarrativeOption option in culture.GetChildhoodOptions())
            {
                if (_registeredOptionIds.Add(option.OptionId))
                    narrativeMenu.AddNarrativeMenuOption(CreateVanillaOption(option, upgrades));
            }
        }
        characterCreationManager.AddNewMenu(narrativeMenu);
    }

    private List<NarrativeMenuCharacterArgs> GetChildhoodMenuNarrativeMenuCharacterArgs(CultureObject culture, string occupationType, CharacterCreationManager characterCreationManager)
    {
        string playerChildhoodAgeEquipmentId = NarrativeEquipmentHelper.GetPlayerChildhoodAgeEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, CharacterObject.PlayerCharacter.IsFemale);
        return new List<NarrativeMenuCharacterArgs>
        {
            new("player_childhood_character", 7, playerChildhoodAgeEquipmentId, "act_childhood_schooled", "spawnpoint_player_1", "", "", null, isHuman: true, CharacterObject.PlayerCharacter.IsFemale)
        };
    }

    private void AddEducationMenu(CharacterCreationManager characterCreationManager, NarrativeSkillUpgrades upgrades)
    {
        BodyProperties originalBodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment);
        originalBodyProperties = FaceGen.GetBodyPropertiesWithAge(ref originalBodyProperties, 12f);
        List<NarrativeMenuCharacter> list = new List<NarrativeMenuCharacter>();
        NarrativeMenuCharacter educationCharacter = new NarrativeMenuCharacter("player_education_character", originalBodyProperties, CharacterObject.PlayerCharacter.Race, CharacterObject.PlayerCharacter.IsFemale);
        list.Add(educationCharacter);
        NarrativeMenu narrativeMenu = new NarrativeMenu("narrative_education_menu", "narrative_childhood_menu", "narrative_youth_menu", new TextObject("{=rcoueCmk}Adolescence"), new TextObject("{=WYvnWcXQ}Like all village children you helped out in the fields. You also..."), list, GetEducationMenuNarrativeMenuCharacterArgs);
        foreach (ICharacterCreationCulture culture in _cultures)
        {
            foreach (NarrativeOption option in culture.GetEducationOptions())
            {
                if (_registeredOptionIds.Add(option.OptionId))
                    narrativeMenu.AddNarrativeMenuOption(CreateVanillaOption(option, upgrades));
            }
        }
        characterCreationManager.AddNewMenu(narrativeMenu);
    }

    private List<NarrativeMenuCharacterArgs> GetEducationMenuNarrativeMenuCharacterArgs(CultureObject culture, string occupationType, CharacterCreationManager characterCreationManager)
    {
        string playerEducationAgeEquipmentId = NarrativeEquipmentHelper.GetPlayerEducationAgeEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, CharacterObject.PlayerCharacter.IsFemale);
        return new List<NarrativeMenuCharacterArgs>
        {
            new NarrativeMenuCharacterArgs("player_education_character", 12, playerEducationAgeEquipmentId, "act_childhood_schooled", "spawnpoint_player_1", "", "", null, isHuman: true, CharacterObject.PlayerCharacter.IsFemale)
        };
    }

    private void AddYouthMenu(CharacterCreationManager characterCreationManager, NarrativeSkillUpgrades upgrades)
    {
        TextObject description = (CharacterObject.PlayerCharacter.IsFemale ? new TextObject("{=5kbeAC7k}In wartorn Calradia, especially in frontier or tribal areas, some women as well as men learn to fight from an early age. You...") : new TextObject("{=F7OO5SAa}As a youngster growing up in Calradia, war was never too far away. You..."));
        BodyProperties originalBodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment);
        originalBodyProperties = FaceGen.GetBodyPropertiesWithAge(ref originalBodyProperties, 17f);
        NarrativeMenuCharacter playerYouthCharacter = new NarrativeMenuCharacter("player_youth_character", originalBodyProperties, CharacterObject.PlayerCharacter.Race, CharacterObject.PlayerCharacter.IsFemale);
        NarrativeMenuCharacter horseCharacter = new NarrativeMenuCharacter("narrative_character_horse");
        List<NarrativeMenuCharacter> list = new List<NarrativeMenuCharacter>();
        list.Add(playerYouthCharacter);
        list.Add(horseCharacter);
        NarrativeMenu narrativeMenu = new NarrativeMenu("narrative_youth_menu", "narrative_education_menu", "narrative_adulthood_menu", new TextObject("{=ok8lSW6M}Youth"), description, list, GetYouthMenuNarrativeMenuCharacterArgs);
        foreach (ICharacterCreationCulture culture in _cultures)
        {
            foreach (NarrativeOption option in culture.GetYouthOptions())
            {
                if (_registeredOptionIds.Add(option.OptionId))
                    narrativeMenu.AddNarrativeMenuOption(CreateVanillaOption(option, upgrades));
            }
        }
        characterCreationManager.AddNewMenu(narrativeMenu);
    }

    private List<NarrativeMenuCharacterArgs> GetYouthMenuNarrativeMenuCharacterArgs(CultureObject culture, string occupationType, CharacterCreationManager characterCreationManager)
    {
        if (string.IsNullOrEmpty(characterCreationManager.CharacterCreationContent.SelectedTitleType))
        {
            characterCreationManager.CharacterCreationContent.SelectedTitleType = "guard";
        }
        string playerEquipmentId = NarrativeEquipmentHelper.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, CharacterObject.PlayerCharacter.IsFemale);
        MBEquipmentRoster playerEquipment = NarrativeEquipmentHelper.LoadWithFallback(playerEquipmentId);
        return new List<NarrativeMenuCharacterArgs>
        {
            new NarrativeMenuCharacterArgs("player_youth_character", 17, playerEquipmentId, "act_childhood_schooled", "spawnpoint_player_1", "", "", null, isHuman: true, CharacterObject.PlayerCharacter.IsFemale),
            new NarrativeMenuCharacterArgs(mountCreationKey: MountCreationKey.GetRandomMountKey(playerEquipment.DefaultEquipment[EquipmentIndex.ArmorItemEndSlot].Item, CharacterObject.PlayerCharacter.GetMountKeySeed()), characterId: "narrative_character_horse", age: -1, equipmentId: "", animationId: "act_inventory_idle_start", spawnPointEntityId: "spawnpoint_mount_1", leftHandItemId: playerEquipment.DefaultEquipment[EquipmentIndex.ArmorItemEndSlot].Item.StringId, rightHandItemId: playerEquipment.DefaultEquipment[EquipmentIndex.HorseHarness].Item.StringId, isHuman: false)
        };
    }

    private void AddAdulthoodMenu(CharacterCreationManager characterCreationManager, NarrativeSkillUpgrades upgrades)
    {
        BodyProperties originalBodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment);
        originalBodyProperties = FaceGen.GetBodyPropertiesWithAge(ref originalBodyProperties, 20f);
        NarrativeMenuCharacter playerAdulthoodCharacter = new NarrativeMenuCharacter("player_adulthood_character", originalBodyProperties, CharacterObject.PlayerCharacter.Race, CharacterObject.PlayerCharacter.IsFemale);
        NarrativeMenuCharacter horseCharacter = new NarrativeMenuCharacter("narrative_character_horse");
        List<NarrativeMenuCharacter> list = new List<NarrativeMenuCharacter>();
        list.Add(playerAdulthoodCharacter);
        list.Add(horseCharacter);
        MBTextManager.SetTextVariable("EXP_VALUE", upgrades.SkillLevelToAdd);
        NarrativeMenu narrativeMenu = new NarrativeMenu("narrative_adulthood_menu", "narrative_youth_menu", "narrative_age_selection_menu", new TextObject("{=MafIe9yI}Young Adulthood"), new TextObject("{=4WYY0X59}Before you set out for a life of adventure, your biggest achievement was..."), list, GetAdultMenuNarrativeMenuCharacterArgs);
        foreach (ICharacterCreationCulture culture in _cultures)
        {
            foreach (NarrativeOption option in culture.GetAdulthoodOptions())
            {
                if (_registeredOptionIds.Add(option.OptionId))
                    narrativeMenu.AddNarrativeMenuOption(CreateVanillaOption(option, upgrades));
            }
        }
        characterCreationManager.AddNewMenu(narrativeMenu);
    }

    private List<NarrativeMenuCharacterArgs> GetAdultMenuNarrativeMenuCharacterArgs(CultureObject culture, string occupationType, CharacterCreationManager characterCreationManager)
    {
        string playerEquipmentId = NarrativeEquipmentHelper.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, CharacterObject.PlayerCharacter.IsFemale);
        MBEquipmentRoster playerEquipment = NarrativeEquipmentHelper.LoadWithFallback(playerEquipmentId);
        return new List<NarrativeMenuCharacterArgs>
        {
            new NarrativeMenuCharacterArgs("player_adulthood_character", 20, playerEquipmentId, "act_childhood_schooled", "spawnpoint_player_1", "", "", null, isHuman: true, CharacterObject.PlayerCharacter.IsFemale),
            new NarrativeMenuCharacterArgs(mountCreationKey: MountCreationKey.GetRandomMountKey(playerEquipment.DefaultEquipment[EquipmentIndex.ArmorItemEndSlot].Item, CharacterObject.PlayerCharacter.GetMountKeySeed()), characterId: "narrative_character_horse", age: -1, equipmentId: "", animationId: "act_horse_stand_1", spawnPointEntityId: "spawnpoint_mount_1", leftHandItemId: playerEquipment.DefaultEquipment[EquipmentIndex.ArmorItemEndSlot].Item.StringId, rightHandItemId: playerEquipment.DefaultEquipment[EquipmentIndex.HorseHarness].Item.StringId, isHuman: false)
        };
    }


    private void AddAgeSelectionMenu(CharacterCreationManager characterCreationManager, NarrativeSkillUpgrades upgrades)
    {
        MBTextManager.SetTextVariable("EXP_VALUE", upgrades.SkillLevelToAdd);
        BodyProperties originalBodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment);
        originalBodyProperties = FaceGen.GetBodyPropertiesWithAge(ref originalBodyProperties, characterCreationManager.CharacterCreationContent.StartingAge);
        NarrativeMenuCharacter ageSelectionCharacter = new NarrativeMenuCharacter("player_age_selection_character", originalBodyProperties, CharacterObject.PlayerCharacter.Race, CharacterObject.PlayerCharacter.IsFemale);
        NarrativeMenuCharacter horseCharacter = new NarrativeMenuCharacter("narrative_character_horse");
        List<NarrativeMenuCharacter> list = new List<NarrativeMenuCharacter>();
        list.Add(ageSelectionCharacter);
        list.Add(horseCharacter);
        NarrativeMenu narrativeMenu = new NarrativeMenu("narrative_age_selection_menu", "narrative_adulthood_menu", "", new TextObject("{=HDFEAYDk}Starting Age"), new TextObject("{=VlOGrGSn}Your character started off on the adventuring path at the age of..."), list, GetAgeSelectionMenuNarrativeMenuCharacterArgs);
        narrativeMenu.AddNarrativeMenuOption(CreateAgeOption("age_selection_young_adult_option", "{=!}20", "{=2k7adlh7}While lacking experience a bit, you are full with youthful energy, you are fully eager, for the long years of adventuring ahead.", 20, 2, 1, "act_childhood_focus", characterCreationManager));
        narrativeMenu.AddNarrativeMenuOption(CreateAgeOption("age_selection_adult_option", "{=!}30", "{=NUlVFRtK}You are at your prime, You still have some youthful energy but also have a substantial amount of experience under your belt. ", 30, 4, 2, "act_childhood_athlete", characterCreationManager));
        narrativeMenu.AddNarrativeMenuOption(CreateAgeOption("age_selection_middle_age_option", "{=!}40", "{=5MxTYApM}This is the right age for starting off, you have years of experience, and you are old enough for people to respect you and gather under your banner.", 40, 6, 3, "act_childhood_sharp", characterCreationManager));
        narrativeMenu.AddNarrativeMenuOption(CreateAgeOption("age_selection_elder_option", "{=!}50", "{=ePD5Afvy}While you are past your prime, there is still enough time to go on that last big adventure for you. And you have all the experience you need to overcome anything!", 50, 8, 4, "act_childhood_tough", characterCreationManager));
        characterCreationManager.AddNewMenu(narrativeMenu);
    }

    private List<NarrativeMenuCharacterArgs> GetAgeSelectionMenuNarrativeMenuCharacterArgs(CultureObject culture, string occupationType, CharacterCreationManager characterCreationManager)
    {
        string playerEquipmentId = NarrativeEquipmentHelper.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, CharacterObject.PlayerCharacter.IsFemale);
        MBEquipmentRoster playerEquipment = NarrativeEquipmentHelper.LoadWithFallback(playerEquipmentId);
        return new List<NarrativeMenuCharacterArgs>
        {
            new NarrativeMenuCharacterArgs("player_age_selection_character", characterCreationManager.CharacterCreationContent.StartingAge, playerEquipmentId, "act_childhood_schooled", "spawnpoint_player_1", "", "", null, isHuman: true, CharacterObject.PlayerCharacter.IsFemale),
            new NarrativeMenuCharacterArgs(mountCreationKey: MountCreationKey.GetRandomMountKey(playerEquipment.DefaultEquipment[EquipmentIndex.ArmorItemEndSlot].Item, CharacterObject.PlayerCharacter.GetMountKeySeed()), characterId: "narrative_character_horse", age: -1, equipmentId: "", animationId: "act_horse_stand_1", spawnPointEntityId: "spawnpoint_mount_1", leftHandItemId: playerEquipment.DefaultEquipment[EquipmentIndex.ArmorItemEndSlot].Item.StringId, rightHandItemId: playerEquipment.DefaultEquipment[EquipmentIndex.HorseHarness].Item.StringId, isHuman: false)
        };
    }

    private NarrativeMenuOption CreateAgeOption(string id, string titleKey, string descKey, int age, int focus, int attribute, string animation, CharacterCreationManager ccm)
    {
        return new NarrativeMenuOption(id, new TextObject(titleKey), new TextObject(descKey),
            args =>
            {
                args.SetUnspentFocusToAdd(focus);
                args.SetUnspentAttributeToAdd(attribute);
            },
            _ => true,
            characterCreationManager =>
            {
                string playerEquipmentId = NarrativeEquipmentHelper.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, CharacterObject.PlayerCharacter.IsFemale);
                foreach (NarrativeMenuCharacter character in characterCreationManager.CurrentMenu.Characters)
                {
                    if (character.StringId == "player_age_selection_character")
                    {
                        character.SetAnimationId(animation);
                        character.ChangeAge(age);
                        MBEquipmentRoster equipment = NarrativeEquipmentHelper.LoadWithFallback(playerEquipmentId);
                        character.SetEquipment(equipment);
                        break;
                    }
                }
                characterCreationManager.CharacterCreationContent.StartingAge = age;
                Hero.MainHero.SetBirthDay(CampaignTime.YearsFromNow(-age));
            },
            null);
    }

    private void ApplyMainHeroEquipment(CharacterCreationManager characterCreationManager)
    {
        NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_age_selection_menu");
        NarrativeMenuCharacter narrativeMenuCharacter = null;
        foreach (NarrativeMenuCharacter character in narrativeMenuWithId.Characters)
        {
            if (character.StringId.Equals("player_age_selection_character"))
            {
                narrativeMenuCharacter = character;
                break;
            }
        }
        CharacterObject.PlayerCharacter.Equipment.FillFrom(narrativeMenuCharacter.Equipment.DefaultEquipment);
        CharacterObject.PlayerCharacter.FirstCivilianEquipment.FillFrom(narrativeMenuCharacter.Equipment.GetRandomCivilianEquipment());
    }

    public void SetHeroAge(float age)
    {
        Hero.MainHero.SetBirthDay(CampaignTime.YearsFromNow(0f - age));
    }

    private NarrativeMenuOption CreateVanillaOption(NarrativeOption option, NarrativeSkillUpgrades upgrades)
    {
        return new NarrativeMenuOption(option.OptionId, option.Title, option.Description,
            args => option.SetArgs(args, upgrades),
            ccm => option.Condition(ccm),
            ccm => option.OnSelect(ccm),
            null);
    }
}
