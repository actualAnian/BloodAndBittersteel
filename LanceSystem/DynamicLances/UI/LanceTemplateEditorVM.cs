using System.Linq;
using LanceSystem.Deserialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;
using TaleWorlds.Localization;

namespace LanceSystem.DynamicLances.UI
{
    public class LanceTemplateEditorVM : ViewModel
    {
        string _templateName = "New Lance";
        string _bannerCode = "";
        string _likelihoodSumText = "";
        string _infoText = "Likelihood has to sum to 1. Values are normalized automatically.";
        MBBindingList<TroopDataRowVM> _troopRows = new();
        ImageIdentifierVM _templateBannerVisual;
        bool _isDirty;
        string _troopName;
        int _templateNameFontSize = 42;
        readonly LanceTemplateEditorController _controller;
        public LanceTemplateEditorVM(LanceTemplateEditorController controller, Lance lance)
        {
            _controller = controller;
            _templateName = lance.Name;
            _bannerCode = lance.bannerKey ?? "";
            _troopRows.Clear();
            foreach (var t in lance.TroopsTemplate.TroopTypes) _troopRows.Add(new TroopDataRowVM(this, _controller, t));
            var banner = new Banner(_bannerCode);
            _templateBannerVisual = new BannerImageIdentifierVM(banner);
            UpdateBannerVisual();
            UpdateTemplateNameFontSize();
            _isDirty = false;
            RefreshLikelihoodSum();
            _troopName = new TextObject("Troop Name").ToString();
        }

        [DataSourceProperty]
        public string TemplateName
        {
            get => _templateName;
            set { if (value != _templateName) { _templateName = value; OnPropertyChangedWithValue(value, nameof(TemplateName)); UpdateTemplateNameFontSize(); MarkDirty(); } }
        }

        [DataSourceProperty]
        public string TroopName
        {
            get => _troopName;
            set { if (value != _troopName) { _troopName = value; OnPropertyChangedWithValue(value, nameof(_troopName)); } }
        }
        [DataSourceProperty]
        public int TemplateNameFontSize
        {
            get => _templateNameFontSize;
            set { if (value != _templateNameFontSize) { _templateNameFontSize = value; OnPropertyChangedWithValue(value, nameof(TemplateNameFontSize)); } }
        }

        [DataSourceProperty]
        public string BannerCode
        {
            get => _bannerCode;
            set { if (value != _bannerCode) { _bannerCode = value; OnPropertyChangedWithValue(value, nameof(BannerCode)); UpdateBannerVisual(); MarkDirty(); } }
        }

        [DataSourceProperty]
        public ImageIdentifierVM TemplateBannerVisual
        {
            get => _templateBannerVisual;
            set { if (value != _templateBannerVisual) { _templateBannerVisual = value; OnPropertyChangedWithValue(value, nameof(TemplateBannerVisual)); } }
        }

        [DataSourceProperty]
        public MBBindingList<TroopDataRowVM> TroopRows
        {
            get => _troopRows;
            set { if (value != _troopRows) { _troopRows = value; OnPropertyChangedWithValue(value, nameof(TroopRows)); } }
        }

        [DataSourceProperty]
        public string LikelihoodSumText
        {
            get => _likelihoodSumText;
            set { if (value != _likelihoodSumText) { _likelihoodSumText = value; OnPropertyChangedWithValue(value, nameof(LikelihoodSumText)); } }
        }

        [DataSourceProperty]
        public string InfoText
        {
            get => _infoText;
            set { if (value != _infoText) { _infoText = value; OnPropertyChangedWithValue(value, nameof(InfoText)); } }
        }

        public void RefreshLikelihoodSum()
        {
            var sum = _troopRows.Sum(r => r.Likelihood);
            LikelihoodSumText = $"Sum: {sum:0.##} / 1.0 {(Mathf.Abs((float)sum - 1f) < 0.001f ? "(OK)" : "(must sum to 1)")}";
        }

        public void RemoveRow(TroopDataRowVM row)
        {
            _troopRows.Remove(row);
            MarkDirty();
            RefreshLikelihoodSum();
        }

        public void AddTroopRow(TroopData data)
        {
            _troopRows.Add(new TroopDataRowVM(this, _controller, data));
            MarkDirty();
            RefreshLikelihoodSum();
        }
        void UpdateTemplateNameFontSize()
        {
            var len = _templateName?.Length ?? 0;
            int size = len <= 15 ? 42 : len <= 22 ? 36 : len <= 30 ? 28 : len <= 40 ? 22 : 18;
            TemplateNameFontSize = size;
        }

        void UpdateBannerVisual()
        {
            if (!string.IsNullOrWhiteSpace(_bannerCode))
            {
                var banner = new Banner(_bannerCode);
                TemplateBannerVisual = new BannerImageIdentifierVM(banner);
                return;
            }
            var clan = Campaign.Current != null ? Clan.PlayerClan?.Banner : null;
            if (clan != null) { TemplateBannerVisual = new BannerImageIdentifierVM(clan); return; }
            TemplateBannerVisual = new BannerImageIdentifierVM(null);
        }
        public void MarkDirty() => _isDirty = true;
        public void ClearDirty() => _isDirty = false;
        public bool HasUnsavedChanges => _isDirty;
        public void LoadFromLance(Lance lance)
        {
            TemplateName = lance.Name;
            BannerCode = lance.bannerKey ?? "";
            _troopRows.Clear();
            foreach (var t in lance.TroopsTemplate.TroopTypes) _troopRows.Add(new TroopDataRowVM(this, _controller, t));
            UpdateBannerVisual();
            UpdateTemplateNameFontSize();
            RefreshLikelihoodSum();
        }

        public void ExecuteEditBanner()
        {
            _controller.EditBanner(_bannerCode, OnBannerEdited);
        }

        void OnBannerEdited(string? newBannerCode)
        {
            if (newBannerCode != null)
                BannerCode = newBannerCode;
            _controller.ReactivateLayer();
        }

        public void ExecuteSwitchTemplate() => _controller.SwitchTemplate();
        public void ExecuteSaveTemplate() => _controller.Save();
        public void ExecuteCopyTemplate() => _controller.CopyTemplate();
        public void ExecutePasteTemplate() => _controller.PasteTemplate();
        public void ExecuteCreateNewTemplate() => _controller.CreateNewTemplate();
        public void ExecuteAddTroop() => _controller.AddTroop();
        public void ExecuteClose() => _controller.DeleteLayer();
        public void ExecuteSaveAndClose() => _controller.SaveAndClose();

        public void TryOpenTroopEditor(string troopId) => _controller.OpenTroopEditor(troopId);

        public void ExecuteRenameTemplate()
        {
            InformationManager.ShowTextInquiry(new TextInquiryData("Rename Template", "Enter new name", true, true, "Confirm", "Cancel", s => { TemplateName = s; }, null, false, null, TemplateName, ""), false, false);
        }

        public void ExecuteBannerClicked()
        {
            InformationManager.DisplayMessage(new InformationMessage("BannerClicked pressed"));
        }
    }
}
