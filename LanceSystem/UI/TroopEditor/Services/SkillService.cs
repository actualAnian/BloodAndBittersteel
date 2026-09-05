using LanceSystem.DynamicTroops;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace LanceSystem.UI.TroopEditor.Services
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
            foreach (var skill in TroopSkills.All)
                _skills[skill] = character.GetSkillValue(skill);
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
            return _getTier() switch
            { 
                1 => 90, 
                2 => 200,
                3 => 400,
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
