using LanceSystem.Common;
using LanceSystem.Deserialization;
using LanceSystem.DynamicTroops.UI;
using LanceSystem.DynamicTroops.UI.ItemSelection;
using LanceSystem.DynamicTroops.UI.Services;
using LanceSystem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;
using static LanceSystem.DynamicTroops.UI.ItemSelection.Filters.FilterFactory;

namespace LanceSystem.DynamicLances.UI
{
    public class LanceTemplateEditorController
    {
        static readonly TextObject _changeCategoryTitle = new("{=lance_change_category_title}Change Category");
        static readonly TextObject _changeCategoryHint = new("{=lance_change_category_hint}Select category");
        static readonly TextObject _switchTemplateTitle = new("{=lance_switch_template_title}Switch Template");
        static readonly TextObject _switchTemplateHint = new("{=lance_switch_template_hint}Select template to load");
        static readonly TextObject _loadText = new("{=lance_load}Load");
        static readonly TextObject _noTemplatesText = new("{=lance_no_templates}No lance templates available");
        static readonly TextObject _savedPrefix = new("{=lance_saved_prefix}Saved: ");
        static readonly TextObject _copiedText = new("{=lance_copied}Lance template copied to clipboard");
        static readonly TextObject _noTemplateClipboardText = new("{=lance_no_template_clipboard}No lance template in clipboard. Copy a template first.");
        static readonly TextObject _pasteSuccessText = new("{=lance_paste_success}Lance template pasted");
        static readonly TextObject _troopIdEmptyText = new("{=lance_troop_id_empty}TroopId empty");
        static readonly TextObject _infantryText = new("{=lance_infantry}Infantry");
        static readonly TextObject _rangedText = new("{=lance_ranged}Ranged");
        static readonly TextObject _cavalryText = new("{=lance_cavalry}Cavalry");
        static readonly TextObject _horseArcherText = new("{=lance_horse_archer}HorseArcher");
        static readonly TextObject _newLanceText = new("{=lance_new_template_name}New Lance");

        GauntletLayer? _layer;
        GauntletMovieIdentifier? _movie;
        SpriteCategory? _orderCategory;
        public LanceTemplateEditorVM Vm { get; internal set; }

        LanceTemplateEditorController(Lance lance)
        {
            Vm = new LanceTemplateEditorVM(this, lance);
        }
        public static void CreateLayer(string lanceId)
        {
            var lance = LanceTemplateManager.Instance.GetLanceFromId(lanceId);
            var controller = new LanceTemplateEditorController(lance);
            controller.OpenLayer();
        }
        public static void CreateLayer()
        {
            var troopTemplates = new LanceTroopsTemplate(new() { new(LanceTroopCategory.Infantry, 0.5, "imperial_recruit") });
            var lance = new Lance("temp", _newLanceText.ToString(), null, null, LanceTemplateOriginType.All, troopTemplates);
            var controller = new LanceTemplateEditorController(lance);
            controller.OpenLayer();
        }


        void OpenLayer()
        {
            _orderCategory = UIResourceManager.LoadSpriteCategory("ui_order");
            _layer = new GauntletLayer("LanceTemplateEditorLayer", 1001);
            Vm.RefreshValues();
            _movie = _layer.LoadMovie("LanceTemplateEditor", Vm);
            _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            ScreenManager.TopScreen.AddLayer(_layer);
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
        }

        public void DeleteLayer()
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
        }
        public void EditBanner(string bannerCode, Action<string?> onBannerEdited)
        {
            var clan = Clan.PlayerClan;
            var originalBanner = clan.Banner;
            var color1 = clan.Color;
            var color2 = clan.Color2;
            var editBanner = new Banner(clan.Banner.BannerCode);
            clan.Banner = editBanner;
            Game.Current.GameStateManager.PushState(
                Game.Current.GameStateManager.CreateState<BannerEditorState>(() =>
                {
                    var result = clan.Banner.BannerCode;
                    clan.Banner = originalBanner;
                    clan.Color = color1;
                    clan.Color2 = color2;
                    onBannerEdited(result != Clan.PlayerClan.Banner.BannerCode ? result : null);
                }));
        }

        public void ReactivateLayer()
        {
            if (_layer != null)
            {
                _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
                _layer.IsFocusLayer = true;
                ScreenManager.TrySetFocus(_layer);
            }
        }

        public void AddTroop()
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier)
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>(UITexts.SelectTroop.ToString(), allCharacters, OnAddTroopSelected, (character, close) => new CharacterCardVM(character, OnAddTroopSelected, close), CreateCharacterFilters());
            controller.Open();
        }

        void OnAddTroopSelected(CharacterObject character)
        {
            var category = FormationClassToLanceTroopCategory(character.DefaultFormationClass);
            Vm.AddTroopRow(new TroopData(category, 0, character.StringId));
        }

        public void SelectTroopForRow(TroopDataRowVM row, int formationClassValue)
        {
            var category = formationClassValue.ToCategory();
            var matchingFormationClasses = GetFormationClassesForCategory(category);
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier && matchingFormationClasses.Contains(c.DefaultFormationClass))
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>(UITexts.SelectTroop.ToString(), allCharacters, c => OnTroopSelectedForRow(row, c), (character, close) => new CharacterCardVM(character, c => OnTroopSelectedForRow(row, c), close), CreateCharacterFilters());
            controller.Open();
        }

        void OnTroopSelectedForRow(TroopDataRowVM row, CharacterObject character)
        {
            row.BasicTroopId = character.StringId;
            var category = FormationClassToLanceTroopCategory(character.DefaultFormationClass);
            row.FormationClassValue = row.GetFormationClassValueFromCategory(category);
        }

        public void ChangeCategoryForRow(TroopDataRowVM row)
        {
            var elements = new List<InquiryElement>
            {
                new((int)LanceTroopCategory.Infantry, _infantryText.ToString(), null),
                new((int)LanceTroopCategory.Ranged, _rangedText.ToString(), null),
                new((int)LanceTroopCategory.Cavalry, _cavalryText.ToString(), null),
                new((int)LanceTroopCategory.HorseArcher, _horseArcherText.ToString(), null)
            };
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(_changeCategoryTitle.ToString(), _changeCategoryHint.ToString(), elements, true, 1, 1, UITexts.Confirm.ToString(), null, args =>
            {
                if (args == null || !args.Any()) return;
                var selected = (LanceTroopCategory)(int)args.First().Identifier;
                var formationValue = row.GetFormationClassValueFromCategory(selected);
                row.FormationClassValue = formationValue;
                row.BasicTroopId = SetCharacterToFirstFromCategory(formationValue);
            }, null, "", false), false, false);
        }
        private string SetCharacterToFirstFromCategory(int formationValue)
        {
            var character = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>().FirstOrDefault(t => t.Occupation == Occupation.Soldier && (int)t.DefaultFormationClass == formationValue);
            return character?.StringId ?? "";
        }
        static LanceTroopCategory FormationClassToLanceTroopCategory(FormationClass formationClass)
        {
            return formationClass switch
            {
                FormationClass.Ranged or FormationClass.Skirmisher => LanceTroopCategory.Ranged,
                FormationClass.Cavalry or FormationClass.HeavyCavalry or FormationClass.LightCavalry => LanceTroopCategory.Cavalry,
                FormationClass.HorseArcher => LanceTroopCategory.HorseArcher,
                _ => LanceTroopCategory.Infantry,
            };
        }

        static FormationClass[] GetFormationClassesForCategory(LanceTroopCategory category)
        {
            return category switch
            {
                LanceTroopCategory.Ranged => new[] { FormationClass.Ranged, FormationClass.Skirmisher },
                LanceTroopCategory.Cavalry => new[] { FormationClass.Cavalry, FormationClass.HeavyCavalry, FormationClass.LightCavalry },
                LanceTroopCategory.HorseArcher => new[] { FormationClass.HorseArcher },
                _ => new[] { FormationClass.Infantry },
            };
        }

        public void SwitchTemplate()
        {
            var lances = DynamicLancesService.Instance.GetAllLances().ToList();
            if (lances.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(_noTemplatesText.ToString()));
                return;
            }
            var elements = lances.Select(l => new InquiryElement(l.StringId, l.Name, null)).ToList();
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(_switchTemplateTitle.ToString(), _switchTemplateHint.ToString(), elements, true, 1, 1, _loadText.ToString(), null, args =>
            {
                if (args == null || !args.Any()) return;
                var id = (string)args.First().Identifier;
                var lance = DynamicLancesService.Instance.GetLance(id);
                if (lance == null) return;
                Vm.LoadFromLance(lance);
            }, null, "", false), false, false);
        }

        public Lance BuildLance()
        {
            var troops = Vm.TroopRows.Select(r => r.ToTroopData()).ToList();
            var normalized = LanceDataDeserializer.NormalizeTroopLikelihoods(troops);
            string stringId = Vm.TemplateName.ToLower().Replace(" ", "_");
            return new Lance(stringId, Vm.TemplateName, null, null, LanceTemplateOriginType.All, new LanceTroopsTemplate(normalized), 1, string.IsNullOrWhiteSpace(Vm.BannerCode) ? null : Vm.BannerCode);
        }
        public void Save()
        {
            var lance = BuildLance();
            var result = DynamicLancesService.Instance.SaveLance(lance.Name, lance.CultureId, lance.ClanId, lance.LanceOriginType, lance.TroopsTemplate, lance.weight, lance.bannerKey);
            if (!result.IsSuccess)
            {
                InformationManager.DisplayMessage(new InformationMessage(result.ErrorMessage!, new Color(1, 0, 0)));
                return;
            }
            Vm.ClearDirty();
            Vm.RefreshLikelihoodSum();
            MBTextManager.SetTextVariable("NAME", lance.Name);
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=lance_saved_full}{SAVED_PREFIX}{NAME}")
                .SetTextVariable("SAVED_PREFIX", _savedPrefix).ToString()));
        }

        public void CopyTemplate()
        {
            var lance = BuildLance();
            EditorTemplateHolder.Instance.LanceTemplate = lance;
            InformationManager.DisplayMessage(new InformationMessage(_copiedText.ToString()));
        }

        public void PasteTemplate()
        {
            var template = EditorTemplateHolder.Instance.LanceTemplate;
            if (template == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(_noTemplateClipboardText.ToString()));
                return;
            }
            if (Vm.HasUnsavedChanges)
            {
                InformationManager.ShowInquiry(new InquiryData(UITexts.UnsavedTitle.ToString(), UITexts.UnsavedMessage.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), () => ApplyPaste(template), null, "", 0f, null, null, null), true, false);
                return;
            }
            ApplyPaste(template);
        }

        void ApplyPaste(Lance template)
        {
            Vm.LoadFromLance(template);
            InformationManager.DisplayMessage(new InformationMessage(_pasteSuccessText.ToString()));
        }

        public void CreateNewTemplate()
        {
            var troopTemplates = new LanceTroopsTemplate(new() { new(LanceTroopCategory.Infantry, 0.5, "imperial_recruit"), new(LanceTroopCategory.Ranged, 0.5, "imperial_recruit") });
            var lance = new Lance("temp", _newLanceText.ToString(), null, null, LanceTemplateOriginType.All, troopTemplates);
            Vm.LoadFromLance(lance);
        }

        public void OpenTroopEditor(string troopId)
        {
            if (string.IsNullOrWhiteSpace(troopId)) { InformationManager.DisplayMessage(new InformationMessage(_troopIdEmptyText.ToString())); return; }
            void DoOpen()
            {
                DeleteLayer();
                CharacterObject character = MBObjectManager.Instance.GetObject<CharacterObject>(troopId) ?? Game.Current.ObjectManager.GetObject<CharacterObject>(troopId);
                if (character == null) return;
                var controller = new TroopEditorController(character);
                TroopEditorViewService.Create(controller.Vm);
            }
            if (Vm.HasUnsavedChanges)
                InformationManager.ShowInquiry(new InquiryData(UITexts.UnsavedTitle.ToString(), UITexts.UnsavedMessage.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), () => DoOpen(), null, "", 0f, null, null, null), true, false);
            else DoOpen();
        }

        public void SaveAndClose()
        {
            Save();
            DeleteLayer();
        }
    }
}
