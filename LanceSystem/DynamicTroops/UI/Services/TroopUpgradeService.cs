using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace LanceSystem.DynamicTroops.UI.Services
{
    public class TroopUpgradeService
    {
        readonly Action _refresh;
        List<CharacterObject> _upgradeTargets;
        public TroopUpgradeService(CharacterObject character, Action refresh)
        {
            _refresh = refresh;
            _upgradeTargets = character.UpgradeTargets?.ToList() ?? new();
        }
        public List<CharacterObject> GetTroopUpgradeTargets() => _upgradeTargets;
        public void AddTroopUpgradeTarget(CharacterObject target)
        {
            if (_upgradeTargets.Contains(target)) return;
            _upgradeTargets.Add(target);
            _refresh();
        }
        public void RemoveTroopUpgradeTarget(CharacterObject target)
        {
            _upgradeTargets.Remove(target);
            _refresh();
        }
        public void SetTroopUpgradeTargets(List<CharacterObject> targets)
        {
            _upgradeTargets = targets;
            _refresh();
        }
    }
}
