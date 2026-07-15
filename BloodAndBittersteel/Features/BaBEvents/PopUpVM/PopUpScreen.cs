using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpVM
{
    public class BaBEventState : GameState
    {
        public BaBEventState(TextObject title, TextObject smallText, string spriteName)
        {
            Title = title.ToString();
            SmallText = smallText.ToString();
            SpriteName = spriteName;
        }
        public BaBEventState() { }

        public string SmallText { get; private set; }
        public string Title { get; private set; }
        public string SpriteName { get; private set; }
    }

    [GameStateScreen(typeof(BaBEventState))]
    public class BaBEventPopupScreen : ScreenBase, IGameStateListener
    {
        GauntletLayer? _layer;
        BaBEventPopupVM? _dataSource;
        private string _title;
        private string _smallText;
        private string _spriteName;

        public BaBEventPopupScreen(BaBEventState state)
        {
            _title = state.Title;
            _smallText = state.SmallText;
            _spriteName = state.SpriteName;
        }
        void IGameStateListener.OnActivate()
        {
            _layer = new GauntletLayer("GauntletLayer", 1, true);
            _dataSource = new BaBEventPopupVM(_title, _smallText, _spriteName);
            _layer.LoadMovie("BaBEventPopup", _dataSource);
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("PartyHotKeyCategory"));
            _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
            AddLayer(_layer);
        }

        void IGameStateListener.OnDeactivate()
        {
            if (_layer == null) return;
            _layer.InputRestrictions.ResetInputRestrictions();
            _layer.IsFocusLayer = false;
            RemoveLayer(_layer);
            ScreenManager.TryLoseFocus(_layer);
            _dataSource = null;
        }

        void IGameStateListener.OnFinalize()
        {
            OnFinalize();
        }
        void IGameStateListener.OnInitialize()
        {
            OnInitialize();
        }
    }
}
