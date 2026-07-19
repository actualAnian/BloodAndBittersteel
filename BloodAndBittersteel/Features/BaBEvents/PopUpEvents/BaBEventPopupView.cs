using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;
namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents
{
    [OverrideView(typeof(BaBEventPopupView))]
    public class BaBEventPopupView : MapView
    {
        private GauntletLayer? _layer;
        private BaBEventVM? _dataSource;
        private GauntletMovieIdentifier? _movie;

        private CampaignTimeControlMode? _previousTimeControlMode;

        public string Title { get; }
        public string SmallText { get; }
        public string SpriteName { get; }

        public BaBEventPopupView(string title, string smallText, string spriteName)
        {
            Title = title;
            SmallText = smallText;
            SpriteName = spriteName;
        }

#pragma warning disable CS8618 // needs an empty constructor for MapView to work
        public BaBEventPopupView() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        protected override void CreateLayout()
        {
            base.CreateLayout();
            _dataSource = new BaBEventVM(Title, SmallText, SpriteName, () => OnPopupClosed());
            _layer = new GauntletLayer("BaBEventPopupLayer", 203);
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
            _layer.InputRestrictions.SetInputRestrictions();
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);

            _movie = _layer.LoadMovie("BaBEventPopup", _dataSource);
            MapScreen.AddLayer(_layer);

            BaBEventMapScreenExtensions.SetIsBaBEventPopupActive(true);
            _previousTimeControlMode = Campaign.Current.TimeControlMode;
            Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
            Campaign.Current.SetTimeControlModeLock(false);
        }
        protected override void OnFinalize()
        {
            _layer?.ReleaseMovie(_movie);
            MapScreen.RemoveLayer(_layer!);
            BaBEventMapScreenExtensions.SetIsBaBEventPopupActive(false);
            Campaign.Current.SetTimeControlModeLock(false);
            Campaign.Current.TimeControlMode = _previousTimeControlMode ?? CampaignTimeControlMode.UnstoppablePlay;

            _layer?.InputRestrictions.ResetInputRestrictions();
            base.OnFinalize();
        }

        protected override bool IsEscaped()
        {
            ClosePopup();
            return true;
        }

        protected override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
        {
            return true;
        }

        private void OnPopupClosed()
        {
            BaBEventMapScreenExtensions.CloseBaBEventPopup();
        }

        private void ClosePopup()
        {
            _dataSource?.ExecuteClose();
        }
    }
}