using System.Linq;
using LanceSystem.Deserialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;
using LanceSystem.DynamicTroops.UI.Services;
using LanceSystem.DynamicTroops.UI;

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
        Lance _originalSnapshot;
        bool _isDirty;
        string _longestTroopName = "Basic Troop";
        int _templateNameFontSize = 42;

        public LanceTemplateEditorVM()
        {
            _templateBannerVisual = new BannerImageIdentifierVM(null);
            _troopRows.Add(new TroopDataRowVM(this, new TroopData(LanceTroopCategory.Infantry, 0.5, "looter")));
            _troopRows.Add(new TroopDataRowVM(this, new TroopData(LanceTroopCategory.Ranged, 0.5, "looter")));
            UpdateBannerVisual();
            UpdateTemplateNameFontSize();
            UpdateLongestTroopName();
            CaptureSnapshot();
            RefreshLikelihoodSum();
        }

        public LanceTemplateEditorVM(Lance lance) : this()
        {
            _templateName = lance.Name;
            _bannerCode = lance.bannerKey ?? "";
            _troopRows.Clear();
            foreach (var t in lance.TroopsTemplate.TroopTypes) _troopRows.Add(new TroopDataRowVM(this, t));
            UpdateBannerVisual();
            UpdateTemplateNameFontSize();
            UpdateLongestTroopName();
            CaptureSnapshot();
            _isDirty = false;
            RefreshLikelihoodSum();
        }

        [DataSourceProperty]
        public string TemplateName
        {
            get => _templateName;
            set { if (value != _templateName) { _templateName = value; OnPropertyChangedWithValue(value, nameof(TemplateName)); UpdateTemplateNameFontSize(); MarkDirty(); } }
        }

        [DataSourceProperty]
        public string LongestTroopName
        {
            get => _longestTroopName;
            set { if (value != _longestTroopName) { _longestTroopName = value; OnPropertyChangedWithValue(value, nameof(LongestTroopName)); } }
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
            UpdateLongestTroopName();
            RefreshLikelihoodSum();
        }

        void AddRow(TroopData data) { _troopRows.Add(new TroopDataRowVM(this, data)); MarkDirty(); UpdateLongestTroopName(); RefreshLikelihoodSum(); }

        public void UpdateLongestTroopName()
        {
            var longest = _troopRows.OrderByDescending(r => r.BasicTroopId?.Length ?? 0).FirstOrDefault()?.BasicTroopId;
            if (string.IsNullOrWhiteSpace(longest)) longest = "Basic Troop";
            if (longest.Length < "Basic Troop".Length) longest = "Basic Troop";
            LongestTroopName = longest;
            foreach (var r in _troopRows) r.RefreshLongestName();
        }

        void UpdateTemplateNameFontSize()
        {
            var len = _templateName?.Length ?? 0;
            int size = len <= 15 ? 42 : len <= 22 ? 36 : len <= 30 ? 28 : len <= 40 ? 22 : 18;
            TemplateNameFontSize = size;
        }

        void CaptureSnapshot()
        {
            try { _originalSnapshot = BuildLanceInternal(); _isDirty = false; } catch { _isDirty = false; }
        }

        Lance BuildLanceInternal()
        {
            var troops = _troopRows.Select(r => r.ToTroopData()).ToList();
            var normalized = LanceDataDeserializer.NormalizeTroopLikelihoods(troops);
            return new Lance(_templateName.ToLower().Replace(" ", "_"), _templateName, null, null, LanceTemplateOriginType.All, new LanceTroopsTemplate(normalized), 1, string.IsNullOrWhiteSpace(_bannerCode) ? null : _bannerCode);
        }

        void UpdateBannerVisual()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_bannerCode))
                {
                    var banner = new Banner(_bannerCode);
                    TemplateBannerVisual = new BannerImageIdentifierVM(banner);
                    return;
                }
            }
            catch { }
            try
            {
                var clan = TaleWorlds.CampaignSystem.Campaign.Current != null ? TaleWorlds.CampaignSystem.Clan.PlayerClan?.Banner : null;
                if (clan != null) { TemplateBannerVisual = new BannerImageIdentifierVM(clan); return; }
            }
            catch { }
            TemplateBannerVisual = new BannerImageIdentifierVM(null);
        }

        public void MarkDirty() => _isDirty = true;

        public bool HasUnsavedChanges => _isDirty;

        public void TryOpenTroopEditor(string troopId)
        {
            if (string.IsNullOrWhiteSpace(troopId)) { InformationManager.DisplayMessage(new InformationMessage("TroopId empty")); return; }
            void DoOpen()
            {
                LanceTemplateEditorController.DeleteLayer();
                CharacterObject character = MBObjectManager.Instance.GetObject<CharacterObject>(troopId) ?? Game.Current.ObjectManager.GetObject<CharacterObject>(troopId);
                if (character == null) return;
                TroopEditorController manager = new TroopEditorController(character);
                TroopEditorViewService.Create(manager.Vm);
            }
            if (HasUnsavedChanges)
            {
                InformationManager.ShowInquiry(new InquiryData("Unsaved Changes", "There are unsaved changes, do you want to discard them and continue?", true, true, "Yes", "No", () => DoOpen(), null, "", 0f, null, null, null), true, false);
            }
            else DoOpen();
        }

        public Lance BuildLance()
        {
            var troops = _troopRows.Select(r => r.ToTroopData()).ToList();
            var normalized = LanceDataDeserializer.NormalizeTroopLikelihoods(troops);
            return new Lance(_templateName.ToLower().Replace(" ", "_"), _templateName, null, null, LanceTemplateOriginType.All, new LanceTroopsTemplate(normalized), 1, string.IsNullOrWhiteSpace(_bannerCode) ? null : _bannerCode);
        }

        public void ExecuteEditBanner()
        {
            InformationManager.DisplayMessage(new InformationMessage("EditBanner clicked"));
        }

        public void ExecuteSwitchTemplate()
        {
            InformationManager.DisplayMessage(new InformationMessage("SwitchTemplate clicked"));
            //var lances = LanceTemplateManager.Instance.Lances.Values.ToList();
            //if (lances.Count == 0) return;
            //var elements = lances.Select(l => new InquiryElement(l.StringId, l.Name, null)).ToList();
            //InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Switch Template", "Select template to load", elements, true, 1, 1, "Load", null, args =>
            //{
            //    if (args == null || !args.Any()) return;
            //    InformationManager.HideInquiry();
            //    var id = args.First().Identifier as string;
            //    var lance = LanceTemplateManager.Instance.GetLanceFromId(id);
            //    TemplateName = lance.Name;
            //    BannerCode = lance.bannerKey ?? "";
            //    _troopRows.Clear();
            //    foreach (var t in lance.TroopsTemplate.TroopTypes) _troopRows.Add(new TroopDataRowVM(this, t));
            //    RefreshLikelihoodSum();
            //}, null, "", false), false, false);
        }

        public void ExecuteCreateNewTemplate()
        {
            InformationManager.DisplayMessage(new InformationMessage("CreateNewTemplate clicked"));
            TemplateName = "New Lance";
            BannerCode = "";
            _troopRows.Clear();
            AddRow(new TroopData(LanceTroopCategory.Infantry, 0.5, "looter"));
            AddRow(new TroopData(LanceTroopCategory.Ranged, 0.5, "looter"));
        }

        public void ExecuteRenameTemplate()
        {
            InformationManager.DisplayMessage(new InformationMessage("RenameTemplate clicked"));
            InformationManager.ShowTextInquiry(new TextInquiryData("Rename Template", "Enter new name", true, true, "Confirm", "Cancel", s => { TemplateName = s; }, null, false, null, TemplateName, ""), false, false);
        }

        public void ExecuteBannerClicked()
        {
            InformationManager.DisplayMessage(new InformationMessage("BannerClicked pressed"));
        }

        public void ExecuteAddTroop()
        {
            InformationManager.DisplayMessage(new InformationMessage("AddNew clicked"));
            //var categories = new List<InquiryElement>
            //{
            //    new InquiryElement(LanceTroopCategory.Infantry, "Infantry", null),
            //    new InquiryElement(LanceTroopCategory.Ranged, "Ranged", null),
            //    new InquiryElement(LanceTroopCategory.Cavalry, "Cavalry", null),
            //    new InquiryElement(LanceTroopCategory.HorseArcher, "HorseArcher", null)
            //};
            //InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Add Troop", "Select category", categories, true, 1, 1, "Next", null, catArgs =>
            //{
            //    if (catArgs == null || !catArgs.Any()) return;
            //    InformationManager.HideInquiry();
            //    var cat = (LanceTroopCategory)catArgs.First().Identifier;
            //    InformationManager.ShowTextInquiry(new TextInquiryData("Basic Troop Id", "Enter CharacterObject StringId", true, true, "Add", "Cancel", troopId =>
            //    {
            //        if (string.IsNullOrWhiteSpace(troopId)) return;
            //        AddRow(new TroopData(cat, 0.1, troopId.Trim().Trim('"')));
            //    }, null, false, null, "looter", ""), false, false);
            //}, null, "", false), false, false);
        }

        public void ExecuteSaveTemplate()
        {
            InformationManager.DisplayMessage(new InformationMessage("SaveTemplate clicked"));
            var lance = BuildLance();
            if (DynamicLancesService.Instance.IsDynamic(lance.StringId))
                DynamicLancesService.Instance.UpdateLanceFromData(lance.StringId, lance.Name, lance.CultureId, lance.ClanId, lance.LanceOriginType, lance.TroopsTemplate, lance.weight, lance.bannerKey);
            else
                DynamicLancesService.Instance.CreateLanceFromData(lance.Name, lance.CultureId, lance.ClanId, lance.LanceOriginType, lance.TroopsTemplate, lance.weight, lance.bannerKey);
            _isDirty = false;
            _originalSnapshot = lance;
            RefreshLikelihoodSum();
        }

        public void ExecuteClose()
        {
            InformationManager.DisplayMessage(new InformationMessage("Close clicked"));
            LanceTemplateEditorController.DeleteLayer();
        }

        public void ExecuteSaveAndClose()
        {
            InformationManager.DisplayMessage(new InformationMessage("SaveAndClose clicked"));
            ExecuteSaveTemplate();
            ExecuteClose();
        }
    }
}
