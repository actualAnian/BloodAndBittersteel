using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops.UI.Services
{
    public class UpgradeService
    {
        readonly System.Action _refresh;
        List<CharacterObject> _upgradeTargets;
        public UpgradeService(CharacterObject character, System.Action refresh)
        {
            _refresh = refresh;
            _upgradeTargets = character.UpgradeTargets?.ToList() ?? new();
        }
        public List<CharacterObject> GetUpgradeTargets() => _upgradeTargets;
        public void AddUpgradeTarget(CharacterObject target)
        {
            if (_upgradeTargets.Contains(target)) return;
            _upgradeTargets.Add(target);
            _refresh();
        }
        public void RemoveUpgradeTarget(CharacterObject target)
        {
            _upgradeTargets.Remove(target);
            _refresh();
        }
        public void SetUpgradeTargets(List<CharacterObject> targets)
        {
            _upgradeTargets = targets;
            _refresh();
        }
    }
}
