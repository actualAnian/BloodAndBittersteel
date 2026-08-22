using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops
{
    public static class DynamicTroopsXmlHelper
    {
        public const string DefaultFaceKeyTemplateId = "looter";

        public static string BuildNpcCharacterXml(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate)
        {
            int level = TierToLevelMapper.GetLevelForTier(tier);
            string cultureAttr = culture != null ? $" culture=\"Culture.{SecurityElement.Escape(culture.StringId)}\"" : "";
            string escapedName = SecurityElement.Escape(name);
            string isFemaleAttr = isFemale ? "true" : "false";
            string groupAttr = defaultGroup.ToString();
            string faceId = faceKeyTemplate != null ? faceKeyTemplate.StringId : DefaultFaceKeyTemplateId;
            StringBuilder sb = new();
            sb.Append($"<NPCCharacter id=\"{escapedName}\" name=\"{{=!}}{escapedName}\"{cultureAttr} occupation=\"Soldier\" level=\"{level}\" default_group=\"{groupAttr}\" is_basic_troop=\"true\" is_female=\"{isFemaleAttr}\">");
            sb.Append(BuildUpgradeTargetsXml(upgradesTo));
            sb.Append(BuildEquipmentsXml(roster));
            sb.Append(BuildFaceXml(faceId));
            sb.Append("</NPCCharacter>");
            return sb.ToString();
        }

        public static string BuildUpgradeTargetsXml(List<CharacterObject>? upgradesTo)
        {
            if (upgradesTo == null || upgradesTo.Count == 0)
                return "";
            StringBuilder sb = new();
            sb.Append("<upgrade_targets>");
            foreach (CharacterObject target in upgradesTo.Where(u => u != null))
                sb.Append($"<upgrade_target id=\"NPCCharacter.{SecurityElement.Escape(target.StringId)}\" />");
            sb.Append("</upgrade_targets>");
            return sb.ToString();
        }

        public static string BuildEquipmentsXml(MBEquipmentRoster? roster)
        {
            if (roster == null || roster.AllEquipments == null)
                return "";
            List<Equipment> equips = roster.AllEquipments.Where(e => e != null && !e.IsEmpty() && e.IsBattle).ToList();
            if (equips.Count == 0)
                equips = roster.AllEquipments.Where(e => e != null && !e.IsEmpty()).Take(1).ToList();
            if (equips.Count == 0)
                return "";
            StringBuilder sb = new();
            sb.Append("<Equipments>");
            foreach (Equipment eq in equips)
            {
                sb.Append("<EquipmentRoster>");
                for (int i = 0; i < 12; i++)
                {
                    EquipmentIndex idx = (EquipmentIndex)i;
                    EquipmentElement element = eq[idx];
                    if (element.Item == null)
                        continue;
                    string slot = EquipmentIndexToXmlSlot(idx);
                    string itemId = SecurityElement.Escape(element.Item.StringId);
                    sb.Append($"<equipment slot=\"{slot}\" id=\"Item.{itemId}\" />");
                }
                sb.Append("</EquipmentRoster>");
            }
            sb.Append("</Equipments>");
            return sb.ToString();
        }

        public static string BuildFaceXml(string faceTemplateId)
        {
            string escaped = SecurityElement.Escape(faceTemplateId);
            return $"<face><face_key_template value=\"BodyProperty.{escaped}\" /></face>";
        }

        public static string EquipmentIndexToXmlSlot(EquipmentIndex index)
        {
            return index switch
            {
                EquipmentIndex.Weapon0 => "Item0",
                EquipmentIndex.Weapon1 => "Item1",
                EquipmentIndex.Weapon2 => "Item2",
                EquipmentIndex.Weapon3 => "Item3",
                EquipmentIndex.ExtraWeaponSlot => "Item4",
                EquipmentIndex.Head => "Head",
                EquipmentIndex.Body => "Body",
                EquipmentIndex.Leg => "Leg",
                EquipmentIndex.Gloves => "Gloves",
                EquipmentIndex.Cape => "Cape",
                EquipmentIndex.Horse => "Horse",
                EquipmentIndex.HorseHarness => "HorseHarness",
                _ => index.ToString()
            };
        }

        public static EquipmentIndex XmlSlotToEquipmentIndex(string slot)
        {
            return slot switch
            {
                "Item0" => EquipmentIndex.Weapon0,
                "Item1" => EquipmentIndex.Weapon1,
                "Item2" => EquipmentIndex.Weapon2,
                "Item3" => EquipmentIndex.Weapon3,
                "Item4" => EquipmentIndex.ExtraWeaponSlot,
                "Head" => EquipmentIndex.Head,
                "Body" => EquipmentIndex.Body,
                "Leg" => EquipmentIndex.Leg,
                "Gloves" => EquipmentIndex.Gloves,
                "Cape" => EquipmentIndex.Cape,
                "Horse" => EquipmentIndex.Horse,
                "HorseHarness" => EquipmentIndex.HorseHarness,
                _ => (EquipmentIndex)Enum.Parse(typeof(EquipmentIndex), slot)
            };
        }
    }
}
