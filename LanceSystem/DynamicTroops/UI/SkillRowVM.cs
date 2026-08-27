using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
namespace LanceSystem.DynamicTroops.TroopCreation
{
    public class SkillRowVM : ViewModel
    {
        readonly SkillObject _skill;
        readonly Func<int> _getPointsLeft;
        readonly Action<SkillObject, int> _updateSkill;
        string _skillId = "";
        int _skillValue;
        public SkillRowVM(SkillObject skill, int value, Func<int> getPointsLeft, Action<SkillObject, int> updateSkill)
        {
            _skill = skill;
            _getPointsLeft = getPointsLeft;
            _updateSkill = updateSkill;
            SkillId = skill.StringId;
            SkillValue = value;
        }
        [DataSourceProperty] public string SkillId { get => _skillId; set { if (value != _skillId) { _skillId = value; OnPropertyChangedWithValue(value, "SkillId"); } } }
        [DataSourceProperty] public int SkillValue { get => _skillValue; set { if (value != _skillValue) { _skillValue = value; OnPropertyChangedWithValue(value, "SkillValue"); } } }
        public void ExecutePlus()
        {
            int delta = ResolveDelta();
            int allowed = _getPointsLeft();
            _updateSkill(_skill, Math.Min(delta, allowed));
        }
        int ResolveDelta()
        {
            if (Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift)) return 5;
            if (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl)) return 1023;
            return 1;
        }
        public void ExecuteMinus() => _updateSkill(_skill, -1);
    }
}
