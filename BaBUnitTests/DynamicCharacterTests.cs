using LanceSystem.DynamicTroops;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BaBUnitTests
{
    [TestClass]
    public sealed class DynamicCharacterTierMapperTests
    {
        [TestMethod]
        public void GetLevelForTier_KnownTiers_ReturnsMapped()
        {
            Assert.AreEqual(1, TierToLevelMapper.GetLevelForTier(0));
            Assert.AreEqual(6, TierToLevelMapper.GetLevelForTier(1));
            Assert.AreEqual(11, TierToLevelMapper.GetLevelForTier(2));
            Assert.AreEqual(16, TierToLevelMapper.GetLevelForTier(3));
            Assert.AreEqual(21, TierToLevelMapper.GetLevelForTier(4));
            Assert.AreEqual(26, TierToLevelMapper.GetLevelForTier(5));
            Assert.AreEqual(31, TierToLevelMapper.GetLevelForTier(6));
            Assert.AreEqual(36, TierToLevelMapper.GetLevelForTier(7));
        }

        [TestMethod]
        public void GetLevelForTier_UnknownTier_FallbackFormula()
        {
            Assert.AreEqual(36, TierToLevelMapper.GetLevelForTier(10));
            Assert.AreEqual(36, TierToLevelMapper.GetLevelForTier(99));
        }
    }

    [TestClass]
    public sealed class DynamicCharacterXmlHelperTests
    {
        [TestMethod]
        public void BuildFaceXml_NullUsesDefault()
        {
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml("my_troop", false, FormationClass.Infantry, 2, null, null, null, null);
            var shouldHave = "BodyProperty." + DynamicTroopsXmlHelper.DefaultFaceKeyTemplateId;
            Assert.Contains(shouldHave, xml);
        }

        [TestMethod]
        public void BuildFaceXml_WithTemplate_UsesTemplateId()
        {
            MBBodyProperty prop = (MBBodyProperty)FormatterServices.GetUninitializedObject(typeof(MBBodyProperty));
            prop.StringId = "fighter_empire";
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml("my_troop", false, FormationClass.Infantry, 2, null, null, null, prop);
            Assert.Contains("BodyProperty.fighter_empire", xml);
            Assert.DoesNotMatchRegex(new System.Text.RegularExpressions.Regex("BodyProperty\\.looter"), xml);
        }

        [TestMethod]
        public void BuildNpcCharacterXml_NameEqualsId_NameEscaped()
        {
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml("my_troop", false, FormationClass.Infantry, 2, null, null, null, null);
            Assert.Contains("id=\"my_troop\"", xml);
            Assert.Contains("name=\"{=! }my_troop\"".Replace(" ", ""), xml); // check contains name attr start
            // actual: name="{=! }my_troop"
            Assert.IsTrue(xml.Contains("id=\"my_troop\"") && xml.Contains("{=! }my_troop") || xml.Contains("{=!}") );
        }

        [TestMethod]
        public void BuildNpcCharacterXml_TierMapsToLevel()
        {
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml("t", false, FormationClass.Infantry, 3, null, null, null, null);
            int expectedLevel = TierToLevelMapper.GetLevelForTier(3);
            Assert.Contains($"level=\"{expectedLevel}\"", xml);
        }

        [TestMethod]
        public void BuildNpcCharacterXml_IsFemaleAndGroupSerialized()
        {
            string xmlF = DynamicTroopsXmlHelper.BuildNpcCharacterXml("t", true, FormationClass.Ranged, 1, null, null, null, null);
            Assert.Contains("is_female=\"true\"", xmlF);
            Assert.Contains("default_group=\"Ranged\"", xmlF);

            string xmlM = DynamicTroopsXmlHelper.BuildNpcCharacterXml("t", false, FormationClass.Cavalry, 1, null, null, null, null);
            Assert.Contains("is_female=\"false\"", xmlM);
            Assert.Contains("default_group=\"Cavalry\"", xmlM);
        }

        [TestMethod]
        public void BuildUpgradeTargetsXml_Empty_ReturnsEmpty()
        {
            Assert.AreEqual("", DynamicTroopsXmlHelper.BuildUpgradeTargetsXml(null));
            Assert.AreEqual("", DynamicTroopsXmlHelper.BuildUpgradeTargetsXml(new List<CharacterObject>()));
        }

        [TestMethod]
        public void BuildUpgradeTargetsXml_WithTargets_ContainsAll()
        {
            CharacterObject a = (CharacterObject)FormatterServices.GetUninitializedObject(typeof(CharacterObject));
            a.StringId = "upgrade_a";
            CharacterObject b = (CharacterObject)FormatterServices.GetUninitializedObject(typeof(CharacterObject));
            b.StringId = "upgrade_b";
            string xml = DynamicTroopsXmlHelper.BuildUpgradeTargetsXml(new List<CharacterObject> { a, b });
            Assert.Contains("NPCCharacter.upgrade_a", xml);
            Assert.Contains("NPCCharacter.upgrade_b", xml);
            Assert.Contains("<upgrade_targets>", xml);
        }

        [TestMethod]
        public void BuildEquipmentsXml_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", DynamicTroopsXmlHelper.BuildEquipmentsXml(null));
        }

        [TestMethod]
        public void BuildNpcCharacterXml_IsWellFormedXml()
        {
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml("my_troop", false, FormationClass.Infantry, 2, null, null, null, null);
            XmlDocument doc = new();
            doc.LoadXml(xml);
            Assert.AreEqual("NPCCharacter", doc.DocumentElement.Name);
            Assert.AreEqual("my_troop", doc.DocumentElement.Attributes["id"].Value);
        }

        [TestMethod]
        public void BuildNpcCharacterXml_EscapesSpecialChars()
        {
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml("a&b", false, FormationClass.Infantry, 1, null, null, null, null);
            XmlDocument doc = new();
            doc.LoadXml(xml);
            Assert.AreEqual("a&b", doc.DocumentElement.Attributes["id"].Value);
        }

        [TestMethod]
        public void EquipmentIndexToXmlSlot_RoundTrips()
        {
            foreach (EquipmentIndex idx in new[] { EquipmentIndex.Weapon0, EquipmentIndex.Weapon1, EquipmentIndex.Weapon2, EquipmentIndex.Weapon3, EquipmentIndex.Head, EquipmentIndex.Body, EquipmentIndex.Leg, EquipmentIndex.Gloves, EquipmentIndex.Cape, EquipmentIndex.Horse, EquipmentIndex.HorseHarness })
            {
                string slot = DynamicTroopsXmlHelper.EquipmentIndexToXmlSlot(idx);
                EquipmentIndex back = DynamicTroopsXmlHelper.XmlSlotToEquipmentIndex(slot);
                Assert.AreEqual(idx, back);
            }
        }
    }

    [TestClass]
    [DoNotParallelize]
    public sealed class BasicCharacterObjectExtensionTests
    {
        [TestInitialize]
        public void Setup()
        {
            typeof(DynamicTroopsService).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, null);
            DynamicTroopsService.Instance.Reset();
        }

        [TestMethod]
        public void IsDynamicCharacter_NotMarked_ReturnsFalse()
        {
            BasicCharacterObject obj = (BasicCharacterObject)FormatterServices.GetUninitializedObject(typeof(CharacterObject));
            obj.StringId = "some_troop";
            Assert.IsFalse(obj.IsDynamicCharacter());
        }

        [TestMethod]
        public void IsDynamicCharacter_Marked_ReturnsTrue()
        {
            BasicCharacterObject obj = (BasicCharacterObject)FormatterServices.GetUninitializedObject(typeof(CharacterObject));
            obj.StringId = "dyn_troop";
            DynamicTroopsService.Instance.MarkDynamic("dyn_troop");
            Assert.IsTrue(obj.IsDynamicCharacter());
        }
    }

    [TestClass]
    [DoNotParallelize]
    public sealed class DynamicCharacterServiceTests
    {
        class FakeFactory : IDynamicTroopsFactory
        {
            public int CreateCalls;
            public int UpdateCalls;
            public CharacterObject ReturnObject = (CharacterObject)FormatterServices.GetUninitializedObject(typeof(CharacterObject));
            public CharacterObject CreateCharacterFromData(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate)
            {
                CreateCalls++;
                ReturnObject.StringId = name;
                return ReturnObject;
            }
            public void UpdateCharacterFromData(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate)
            {
                UpdateCalls++;
            }
        }    }

    [TestClass]
    [DoNotParallelize]
    public sealed class DynamicCharacterPersistenceTests
    {
        string? _tempPath;
        DynamicTroopsXmlSaver _persistence;

        [TestInitialize]
        public void Setup()
        {
            _tempPath = Path.Combine(Path.GetTempPath(), $"dynamic_troops_{Guid.NewGuid():N}.xml");
            _persistence = new DynamicTroopsXmlSaver(_tempPath);
            if (File.Exists(_tempPath))
                File.Delete(_tempPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempPath))
                File.Delete(_tempPath);
        }

        [TestMethod]
        public void EnsureFileExists_CreatesFileIfNotExists()
        {
            Assert.IsFalse(File.Exists(_tempPath));
            _persistence.CreateCharacterXmlIfNeeded();
            Assert.IsTrue(File.Exists(_tempPath));
            string content = File.ReadAllText(_tempPath);
            Assert.Contains("<NPCCharacters", content);
        }

        [TestMethod]
        public void EnsureFileExists_DoesNotOverwriteExisting()
        {
            _persistence.CreateCharacterXmlIfNeeded();
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml("keep_me", false, FormationClass.Infantry, 1, null, null, null, null);
            _persistence.SaveToXml("keep_me", xml);
            _persistence.CreateCharacterXmlIfNeeded();
            string content = File.ReadAllText(_tempPath);
            Assert.Contains("keep_me", content);
        }

        [TestMethod]
        public void PersistCharacter_AppendCreatesEntry()
        {
            _persistence.CreateCharacterXmlIfNeeded();
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml("troop_a", false, FormationClass.Infantry, 1, null, null, null, null);
            _persistence.SaveToXml("troop_a", xml);
            string content = File.ReadAllText(_tempPath);
            Assert.Contains("id=\"troop_a\"", content);
            Assert.Contains("BodyProperty.looter", content);
        }

        [TestMethod]
        public void PersistCharacter_SecondAppendKeepsBoth()
        {
            _persistence.CreateCharacterXmlIfNeeded();
            string xmlA = DynamicTroopsXmlHelper.BuildNpcCharacterXml("troop_a", false, FormationClass.Infantry, 1, null, null, null, null);
            string xmlB = DynamicTroopsXmlHelper.BuildNpcCharacterXml("troop_b", true, FormationClass.Ranged, 2, null, null, null, null);
            _persistence.SaveToXml("troop_a", xmlA);
            _persistence.SaveToXml("troop_b", xmlB);
            string content = File.ReadAllText(_tempPath);
            Assert.Contains("id=\"troop_a\"", content);
            Assert.Contains("id=\"troop_b\"", content);
        }

        [TestMethod]
        public void PersistCharacter_UpdateReplacesExisting()
        {
            _persistence.CreateCharacterXmlIfNeeded();
            string xml1 = DynamicTroopsXmlHelper.BuildNpcCharacterXml("troop_a", false, FormationClass.Infantry, 1, null, null, null, null);
            _persistence.SaveToXml("troop_a", xml1);
            string xml2 = DynamicTroopsXmlHelper.BuildNpcCharacterXml("troop_a", true, FormationClass.Cavalry, 5, null, null, null, null);
            _persistence.SaveToXml("troop_a", xml2);
            XmlDocument doc = new();
            doc.Load(_tempPath);
            var nodes = doc.DocumentElement.SelectNodes("NPCCharacter[@id='troop_a']");
            Assert.HasCount(1, nodes);
            Assert.Contains("is_female=\"true\"", nodes[0].OuterXml);
            Assert.Contains("default_group=\"Cavalry\"", nodes[0].OuterXml);
        }

        [TestMethod]
        public void PersistCharacter_IsWellFormedAfterMultipleWrites()
        {
            _persistence.CreateCharacterXmlIfNeeded();
            for (int i = 0; i < 5; i++)
            {
                string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml($"t_{i}", i % 2 == 0, FormationClass.Infantry, i, null, null, null, null);
                _persistence.SaveToXml($"t_{i}", xml);
            }
            XmlDocument doc = new();
            doc.Load(_tempPath);
            Assert.HasCount(5, doc.DocumentElement.ChildNodes);
        }

        [TestMethod]
        public void FilePath_ReturnsConstructorPath()
        {
            string expected = Path.Combine(LanceSystem.PathHelper.OutsideConfigPath, "dynamic_troops.xml");
            DynamicTroopsXmlSaver persistence = new(expected);
            Assert.AreEqual(expected, persistence.FilePath);
        }
    }
}
