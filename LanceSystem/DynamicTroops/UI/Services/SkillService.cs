using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops.UI.Services
{
    public class SkillService
    {
        readonly Func<int> _getTier;
        readonly Action _refresh;
        readonly Dictionary<SkillObject, int> _skills = new();
        public SkillService(CharacterObject character, Func<int> getTier, Action refresh)
        {
            _getTier = getTier;
            _refresh = refresh;
            _skills[DefaultSkills.OneHanded] = character.GetSkillValue(DefaultSkills.OneHanded);
            _skills[DefaultSkills.TwoHanded] = character.GetSkillValue(DefaultSkills.TwoHanded);
            _skills[DefaultSkills.Polearm] = character.GetSkillValue(DefaultSkills.Polearm);
            _skills[DefaultSkills.Bow] = character.GetSkillValue(DefaultSkills.Bow);
            _skills[DefaultSkills.Crossbow] = character.GetSkillValue(DefaultSkills.Crossbow);
            _skills[DefaultSkills.Throwing] = character.GetSkillValue(DefaultSkills.Throwing);
            _skills[DefaultSkills.Riding] = character.GetSkillValue(DefaultSkills.Riding);
            _skills[DefaultSkills.Athletics] = character.GetSkillValue(DefaultSkills.Athletics);
        }
        public int GetSkillValue(SkillObject skill) => _skills.TryGetValue(skill, out int value) ? value : 0;
        public int GetCurrentSkillSum()
        {
            int sum = 0;
            foreach (var kvp in _skills) sum += kvp.Value;
            return sum;
        }
        public int GetSuggestedMaxSkillSum()
        {
            if (TroopEditorController.DisableSkillTotalRestriction) return 10000;
            return _getTier() switch
            { 
                1 => 80, 
                2 => 200,
                3 => 350,
                4 => 500,
                5 => 700,
                6 => 900,
                _ => 1200
            };
        }
        public void UpdateSkill(SkillObject skill, int amount)
        {
            int current = GetSkillValue(skill);
            int newValue = Math.Max(0, current + amount);
            _skills[skill] = newValue;
            _refresh();
        }
    }
}
