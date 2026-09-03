using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
namespace LanceSystem.UI.TroopEditor
{
    public class SkillRowVM : ViewModel
    {
        readonly SkillObject _skill;
        readonly Action<SkillObject, int> _updateSkill;
        string _skillId = "";
        int _skillValue;
        public SkillRowVM(SkillObject skill, int value, Action<SkillObject, int> updateSkill)
        {
            _skill = skill;
            _updateSkill = updateSkill;
            SkillId = skill.StringId;
            SkillValue = value;
        }
        [DataSourceProperty] public string SkillId { get => _skillId; set { if (value != _skillId) { _skillId = value; OnPropertyChangedWithValue(value, "SkillId"); } } }
        [DataSourceProperty] public int SkillValue { get => _skillValue; set { if (value != _skillValue) { _skillValue = value; OnPropertyChangedWithValue(value, "SkillValue"); } } }
        public void ExecutePlus() => _updateSkill(_skill, GetMultipliedValue(5));
        public void ExecuteMinus() => _updateSkill(_skill, GetMultipliedValue(-5));
        int GetMultipliedValue(int value)
        {
            if (Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift)) return value * 2;
            if (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl)) return value * 20;
            return value;
        }
        public void ExecuteBeginHint() => InformationManager.ShowTooltip(typeof(string), new object[] { _skill.Name.ToString() });
        public void ExecuteEndHint() => MBInformationManager.HideInformations();

    }
}