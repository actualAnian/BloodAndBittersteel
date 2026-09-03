using System.Linq;
using LanceSystem.Deserialization;
using LanceSystem.UI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.TwoDimension;

namespace LanceSystem.DynamicLances.UI
{
    public class LanceTemplateEditorVM : ViewModel
    {
        static readonly TextObject _infoTextObj = new("{=lance_info_text}Likelihood has to sum to 1. Values are normalized automatically when saved. Each value must be between 0 and 1.");
        static readonly TextObject _troopNameText = new("{=lance_troop_name}Troop Name");
        static readonly TextObject _sumPrefix = new("{=lance_sum_prefix}Sum");
        static readonly TextObject _mustSumText = new("{=lance_must_sum}(must sum to 1)");
        static readonly TextObject _renameTitle = new("{=lance_rename_title}Rename Template");

        static readonly TextObject _bannerClickedText = new("{=lance_banner_clicked}BannerClicked pressed");
        static readonly TextObject _templateBannerHeaderText = new("{=lance_template_banner_header}Template Banner");
        static readonly TextObject _editBannerText = new("{=lance_edit_banner}Edit Banner");
        static readonly TextObject _troopEditorHeaderText = new("{=lance_troop_editor_header}Troop Editor");
        static readonly TextObject _categoryHeaderText = new("{=lance_category_header}Category");
        static readonly TextObject _troopHeaderText = new("{=lance_troop_header}Troop");
        static readonly TextObject _likelihoodHeaderText = new("{=lance_likelihood_header}Likelihood");
        static readonly TextObject _addNewText = new("{=lance_add_new}Add New");
        static readonly TextObject _switchTemplateText = new("{=lance_switch_template}Switch Template");
        static readonly TextObject _newTemplateText = new("{=lance_new_template}New Template");
        static readonly TextObject _saveTemplateText = new("{=lance_save_template}Save Template");
        static readonly TextObject _infoHeaderText = new("{=lance_info_header}Info");
        string _templateName = "";
        string _bannerCode = "";
        string _likelihoodSumText = "";
        MBBindingList<TroopDataRowVM> _troopRows = new();
        ImageIdentifierVM _templateBannerVisual;
        bool _isDirty;
        readonly LanceTemplateEditorController _controller;
        public LanceTemplateEditorVM(LanceTemplateEditorController controller, Lance lance)
        {
            _controller = controller;
            LoadFromLance(lance);
        }

        [DataSourceProperty]
        public string TemplateName
        {
            get => _templateName;
            set { if (value != _templateName) { _templateName = value; OnPropertyChangedWithValue(value, nameof(TemplateName)); MarkDirty(); } }
        }

        [DataSourceProperty]
        public string TroopName
        {
            get => _troopNameText.ToString();
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

        [DataSourceProperty] public string InfoText => _infoTextObj.ToString();
        [DataSourceProperty] public string TemplateBannerHeaderText => _templateBannerHeaderText.ToString();
        [DataSourceProperty] public string EditBannerButtonText => _editBannerText.ToString();
        [DataSourceProperty] public string TroopEditorHeaderText => _troopEditorHeaderText.ToString();
        [DataSourceProperty] public string CategoryHeaderText => _categoryHeaderText.ToString();
        [DataSourceProperty] public string TroopHeaderText => _troopHeaderText.ToString();
        [DataSourceProperty] public string LikelihoodHeaderText => _likelihoodHeaderText.ToString();
        [DataSourceProperty] public string AddNewButtonText => _addNewText.ToString();
        [DataSourceProperty] public string SwitchTemplateButtonText => _switchTemplateText.ToString();
        [DataSourceProperty] public string NewTemplateButtonText => _newTemplateText.ToString();
        [DataSourceProperty] public string CopyTemplateButtonText => UITexts.CopyTemplate.ToString();
        [DataSourceProperty] public string PasteTemplateButtonText => UITexts.PasteTemplate.ToString();
        [DataSourceProperty] public string SaveTemplateButtonText => _saveTemplateText.ToString();
        [DataSourceProperty] public string InfoHeaderText => _infoHeaderText.ToString();
        [DataSourceProperty] public string RenameTitleText => _renameTitle.ToString();
        [DataSourceProperty] public string RenameHintText => UITexts.RenameHint.ToString();
        [DataSourceProperty] public string ConfirmButtonText => UITexts.Confirm.ToString();
        [DataSourceProperty] public string CancelButtonText => GameTexts.FindText("str_cancel").ToString();
        [DataSourceProperty] public string BannerClickedMessageText => _bannerClickedText.ToString();

        public void RefreshLikelihoodSum()
        {
            var sum = _troopRows.Sum(r => r.Likelihood);
            bool isOk = Mathf.Abs((float)sum - 1f) < 0.001f;
            MBTextManager.SetTextVariable("SUM", sum.ToString("0.##"));
            MBTextManager.SetTextVariable("STATUS", isOk ? GameTexts.FindText("str_yes", null).ToString() : _mustSumText);
            LikelihoodSumText = new TextObject("{=lance_likelihood_sum_full}{SUM_PREFIX}: {SUM} / 1.0 {STATUS}")
                .SetTextVariable("SUM_PREFIX", _sumPrefix).ToString();
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
            InformationManager.ShowTextInquiry(new TextInquiryData(RenameTitleText, RenameHintText, true, true, ConfirmButtonText, CancelButtonText, s => { TemplateName = s; }, null, false, null, TemplateName, ""), false, false);
        }

        public void ExecuteBannerClicked()
        {
            InformationManager.DisplayMessage(new InformationMessage(BannerClickedMessageText));
        }
    }
}
