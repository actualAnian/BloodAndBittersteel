using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.TwoDimension;
using TaleWorlds.ScreenSystem;
using TaleWorlds.Library;

namespace LanceSystem.DynamicLances.UI
{
    public static class LanceTemplateEditorManager
    {
        static GauntletLayer _layer;
        static GauntletMovieIdentifier _movie;
        static LanceTemplateEditorVM _vm;
        static SpriteCategory _orderCategory;
        static LanceTemplateEditorVM Vm => _vm;
        public static void CreateLayer()
        {
            if (_layer != null) return;
            _orderCategory = UIResourceManager.LoadSpriteCategory("ui_order");
            _layer = new GauntletLayer("LanceTemplateEditorLayer", 1001);
            _vm = new LanceTemplateEditorVM();
            _vm.RefreshValues();
            _movie = _layer.LoadMovie("LanceTemplateEditor", _vm);
            _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            ScreenManager.TopScreen.AddLayer(_layer);
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
        }

        public static void CreateLayer(string lanceId)
        {
            if (_layer != null) DeleteLayer();
            _orderCategory = UIResourceManager.LoadSpriteCategory("ui_order");
            _layer = new GauntletLayer("LanceTemplateEditorLayer", 1001);
            var lance = Deserialization.LanceTemplateManager.Instance.GetLanceFromId(lanceId);
            _vm = new LanceTemplateEditorVM(lance);
            _vm.RefreshValues();
            _movie = _layer.LoadMovie("LanceTemplateEditor", _vm);
            _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            ScreenManager.TopScreen.AddLayer(_layer);
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
        }

        public static void DeleteLayer()
        {
            var top = ScreenManager.TopScreen;
            if (_layer != null)
            {
                _layer.InputRestrictions.ResetInputRestrictions();
                _layer.IsFocusLayer = false;
                if (_movie != null) _layer.ReleaseMovie(_movie);
                top.RemoveLayer(_layer);
            }
            _orderCategory?.Unload();
            _orderCategory = null;
            _layer = null;
            _movie = null;
            _vm = null;
        }
    }
}
