using TaleWorlds.CampaignSystem;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
namespace LanceSystem.DynamicTroops.UI.Services
{
    public class SkillService
    {
        readonly CharacterObject _character;
        readonly System.Action _refresh;
        public SkillService(CharacterObject character, System.Action refresh)
        {
            _character = character;
            _refresh = refresh;
        }
        public int GetAvailablePoints()
        {
            if (TroopEditorController.DisableSkillTotalRestriction) return 10000;
            return _character.Tier switch { 1 => 80, 2 => 200, 3 => 350, 4 => 500, 5 => 700, 6 => 900, _ => 1200 };
        }
        public int GetPointsLeft()
        {
            return GetAvailablePoints() - _character.GetSkillValue(DefaultSkills.OneHanded) - _character.GetSkillValue(DefaultSkills.TwoHanded) - _character.GetSkillValue(DefaultSkills.Polearm) - _character.GetSkillValue(DefaultSkills.Bow) - _character.GetSkillValue(DefaultSkills.Crossbow) - _character.GetSkillValue(DefaultSkills.Throwing) - _character.GetSkillValue(DefaultSkills.Riding) - _character.GetSkillValue(DefaultSkills.Athletics);
        }
        public int GetSkillCap()
        {
            if (TroopEditorController.DisableSkillCapRestriction) return 1023;
            return _character.Tier switch { 0 => 25, 1 => 25, 2 => 50, 3 => 90, 4 => 120, 5 => 170, 6 => 260, _ => 330 };
        }
        public void UpdateSkill(SkillObject skill, int amount)
        {
            MBCharacterSkills skills = MBObjectManager.Instance.CreateObject<MBCharacterSkills>(_character.StringId);
            CopyExistingSkills(skills);
            int newValue = System.Math.Min(GetSkillCap(), System.Math.Max(0, _character.GetSkillValue(skill) + amount));
            skills.Skills.SetPropertyValue(skill, newValue);
            FieldInfo field = _character.GetType().GetField("DefaultCharacterSkills", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            field?.SetValue(_character, skills);
            _refresh();
        }
        void CopyExistingSkills(MBCharacterSkills target)
        {
            target.Skills.SetPropertyValue(DefaultSkills.Crossbow, _character.GetSkillValue(DefaultSkills.Crossbow));
            target.Skills.SetPropertyValue(DefaultSkills.Bow, _character.GetSkillValue(DefaultSkills.Bow));
            target.Skills.SetPropertyValue(DefaultSkills.Throwing, _character.GetSkillValue(DefaultSkills.Throwing));
            target.Skills.SetPropertyValue(DefaultSkills.OneHanded, _character.GetSkillValue(DefaultSkills.OneHanded));
            target.Skills.SetPropertyValue(DefaultSkills.TwoHanded, _character.GetSkillValue(DefaultSkills.TwoHanded));
            target.Skills.SetPropertyValue(DefaultSkills.Polearm, _character.GetSkillValue(DefaultSkills.Polearm));
            target.Skills.SetPropertyValue(DefaultSkills.Athletics, _character.GetSkillValue(DefaultSkills.Athletics));
            target.Skills.SetPropertyValue(DefaultSkills.Riding, _character.GetSkillValue(DefaultSkills.Riding));
        }
    }
}

