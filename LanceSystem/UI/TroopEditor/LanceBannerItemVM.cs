using System;
using LanceSystem.Deserialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace LanceSystem.UI.TroopEditor
{
    public class LanceBannerItemVM : ViewModel
    {
        readonly Lance _lance;
        readonly Action<string> _onClicked;
        ImageIdentifierVM _bannerVisual;
        string _lanceName;

        public LanceBannerItemVM(Lance lance, Action<string> onClicked)
        {
            _lance = lance;
            _onClicked = onClicked;
            _lanceName = lance.Name;
            _bannerVisual = CreateBannerVisual(lance.bannerKey);
        }

        ImageIdentifierVM CreateBannerVisual(string? bannerKey)
        {
            if (!string.IsNullOrWhiteSpace(bannerKey))
                return new BannerImageIdentifierVM(new Banner(bannerKey));
            var clan = Clan.PlayerClan;
            if (clan?.Banner != null)
                return new BannerImageIdentifierVM(new Banner(clan.Banner.BannerCode));
            return new BannerImageIdentifierVM(null);
        }

        [DataSourceProperty]
        public ImageIdentifierVM BannerVisual
        {
            get => _bannerVisual;
            set { if (value != _bannerVisual) { _bannerVisual = value; OnPropertyChangedWithValue(value, nameof(BannerVisual)); } }
        }

        [DataSourceProperty]
        public string LanceName
        {
            get => _lanceName;
            set { if (value != _lanceName) { _lanceName = value; OnPropertyChangedWithValue(value, nameof(LanceName)); } }
        }

        public void ExecuteClick() => _onClicked?.Invoke(_lance.StringId);
    }
}
