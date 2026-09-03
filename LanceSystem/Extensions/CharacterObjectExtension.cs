using System.Reflection;
using System.Runtime.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.Extensions
{
    public static class CharacterObjectExtension
    {
        static readonly FieldInfo _originCharacterField = typeof(CharacterObject).GetField("_originCharacter", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _occupationField = typeof(CharacterObject).GetField("_occupation", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _personaField = typeof(CharacterObject).GetField("_persona", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _characterTraitsField = typeof(CharacterObject).GetField("_characterTraits", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _isMarinerField = typeof(CharacterObject).GetField("_isMariner", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _civilianEquipmentTemplateField = typeof(CharacterObject).GetField("_civilianEquipmentTemplate", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _battleEquipmentTemplateField = typeof(CharacterObject).GetField("_battleEquipmentTemplate", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _characterRestrictionFlagsField = typeof(CharacterObject).GetField("_characterRestrictionFlags", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly MethodInfo _fillFromMethod = typeof(BasicCharacterObject).GetMethod("FillFrom", BindingFlags.Instance | BindingFlags.NonPublic);

        public static CharacterObject CreateFromWithoutAddingToManager(CharacterObject character, StaticBodyProperties? staticBodyProperties = null)
        {
            var characterObject = (CharacterObject)FormatterServices.GetUninitializedObject(typeof(CharacterObject));
            _occupationField.SetValue(characterObject, Occupation.Soldier);
            _characterTraitsField.SetValue(characterObject, new PropertyOwner<TraitObject>());
            _characterRestrictionFlagsField.SetValue(characterObject, CharacterRestrictionFlags.None);
            var originCharacter = _originCharacterField.GetValue(character);
            _originCharacterField.SetValue(characterObject, originCharacter ?? character);
            if (characterObject.IsHero)
            {
                if (staticBodyProperties != null)
                {
                    characterObject.HeroObject.StaticBodyProperties = staticBodyProperties.Value;
                }
                else
                {
                    characterObject.HeroObject.StaticBodyProperties = (character.IsHero ? character.HeroObject.StaticBodyProperties : character.GetBodyPropertiesMin(false).StaticProperties);
                }
            }
            _occupationField.SetValue(characterObject, _occupationField.GetValue(character));
            _personaField.SetValue(characterObject, _personaField.GetValue(character));
            _characterTraitsField.SetValue(characterObject, new PropertyOwner<TraitObject>((PropertyOwner<TraitObject>)_characterTraitsField.GetValue(character)));
            _isMarinerField.SetValue(characterObject, character.IsMariner);
            _civilianEquipmentTemplateField.SetValue(characterObject, _civilianEquipmentTemplateField.GetValue(character));
            _battleEquipmentTemplateField.SetValue(characterObject, _battleEquipmentTemplateField.GetValue(character));
            characterObject.HiddenInEncyclopedia = character.HiddenInEncyclopedia;
            _fillFromMethod.Invoke(characterObject, new object[] { character });
            characterObject.StringId = characterObject.Name.ToString();
            return characterObject;
        }
    }
}
