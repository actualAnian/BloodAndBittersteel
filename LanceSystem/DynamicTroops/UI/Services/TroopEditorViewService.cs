using LanceSystem.DynamicTroops.TroopCreation;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;
namespace LanceSystem.DynamicTroops.TroopCreation.Services
{
    public static class TroopEditorViewService
    {
        static GauntletLayer? _layer;
        static GauntletMovieIdentifier? _movie;
        public static TroopEditorVM? Vm { get; private set; }
        public static void Create(TroopEditorVM vm)
        {
            if (_layer != null) return;
            Vm = vm;
            _layer = new GauntletLayer("TroopEditorLayer", 1000);
            vm.RefreshValues();
            _movie = _layer.LoadMovie("TroopEditor", vm);
            _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            ScreenManager.TopScreen.AddLayer(_layer);
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
        }
        public static void Delete()
        {
            if (_layer == null) return;
            _layer.InputRestrictions.ResetInputRestrictions();
            _layer.IsFocusLayer = false;
            if (_movie != null) _layer.ReleaseMovie(_movie);
            ScreenManager.TopScreen.RemoveLayer(_layer);
            _layer = null;
            _movie = null;
            Vm = null;
        }
        public static void Refresh()
        {
            Vm?.RefreshValues();
        }
    }
}
