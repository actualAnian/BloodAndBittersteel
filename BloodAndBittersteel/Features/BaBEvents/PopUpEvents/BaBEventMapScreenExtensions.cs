using SandBox.View.Map;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents
{
    public static class BaBEventMapScreenExtensions
    {
        private static bool _isBaBEventPopupActive;
        public static bool IsBaBEventPopupActive => _isBaBEventPopupActive;

        public static void SetIsBaBEventPopupActive(bool value)
        {
            _isBaBEventPopupActive = value;
        }
        public static void CloseBaBEventPopup()
        {
            if (_isBaBEventPopupActive)
            {
                var mapView = MapScreen.Instance.GetMapView<BaBEventPopupView>();
                if (mapView != null)
                {
                    MapScreen.Instance.RemoveMapView(mapView);
                }
                _isBaBEventPopupActive = false;
            }
        }
        public static bool CanShowBaBEventPopup()
        {
            return !IsBaBEventPopupActive && GameStateManager.Current.LastOrDefault<MapState>() != null;
        }
    }
}
