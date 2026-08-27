using LanceSystem.Deserialization;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace LanceSystem.DynamicLances.UI
{
    public class TroopDataRowVM : ViewModel
    {
        readonly LanceTemplateEditorVM _parent;
        int _formationClassValue;
        string _basicTroopId;
        double _likelihood;
        ImageIdentifierVM _troopVisual;
        string _likelihoodText = "";

        public TroopDataRowVM(LanceTemplateEditorVM parent, TroopData data)
        {
            _parent = parent;
            _formationClassValue = GetFormationClassValueFromCategory(data.Category);
            _basicTroopId = data.BasicTroopId;
            _likelihood = data.Likelihood;
            _troopVisual = CreateVisual(_basicTroopId);
            RefreshValues();
        }

        ImageIdentifierVM CreateVisual(string troopId)
        {
            try
            {
                var character = TaleWorlds.ObjectSystem.MBObjectManager.Instance.GetObject<CharacterObject>(troopId);
                if (character != null) return new CharacterImageIdentifierVM(CharacterCode.CreateFrom(character));
            }
            catch { }
            return new ItemImageIdentifierVM(null);
        }

        void UpdateVisual()
        {
            TroopVisual = CreateVisual(_basicTroopId);
        }


        public int GetFormationClassValueFromCategory(LanceTroopCategory category)
        {
            return category switch
            {
                LanceTroopCategory.Infantry => 0,
                LanceTroopCategory.Ranged => 1,
                LanceTroopCategory.Cavalry => 2,
                LanceTroopCategory.HorseArcher => 3,
                _ => 0
            };
        }
        public LanceTroopCategory IntToLanceTroopCategory(int value)
        {
            return value switch
            {
                0 => LanceTroopCategory.Infantry,
                1 => LanceTroopCategory.Ranged,
                2 => LanceTroopCategory.Cavalry,
                3 => LanceTroopCategory.HorseArcher,
                _ => LanceTroopCategory.Infantry
            };
        }

        [DataSourceProperty]
        public int FormationClassValue
        {
            get
            {
                return this._formationClassValue;
            }
            set
            {
                if (value != this._formationClassValue)
                {
                    this._formationClassValue = value;
                    base.OnPropertyChangedWithValue(value, "FormationClassValue");
                    _parent.MarkDirty();
                }
            }
        }

        [DataSourceProperty]
        public string BasicTroopId
        {
            get => _basicTroopId;
            set { if (value != _basicTroopId) { _basicTroopId = value; OnPropertyChangedWithValue(value, nameof(BasicTroopId)); UpdateVisual(); _parent.MarkDirty(); _parent.UpdateLongestTroopName(); } }
        }

        [DataSourceProperty]
        public string LongestTroopName => _parent.LongestTroopName;

        public void RefreshLongestName() => OnPropertyChangedWithValue(LongestTroopName, nameof(LongestTroopName));

        [DataSourceProperty]
        public string LikelihoodText
        {
            get => _likelihoodText;
            set { if (value != _likelihoodText) { _likelihoodText = value; OnPropertyChangedWithValue(value, nameof(LikelihoodText)); } }
        }

        [DataSourceProperty]
        public ImageIdentifierVM TroopVisual
        {
            get => _troopVisual;
            set { if (value != _troopVisual) { _troopVisual = value; OnPropertyChangedWithValue(value, nameof(TroopVisual)); } }
        }

        public double Likelihood
        {
            get => _likelihood;
            set
            {
                var clamped = Math.Max(0, Math.Min(1, value));
                if (Math.Abs(clamped - _likelihood) > 0.0001)
                {
                    _likelihood = clamped;
                    LikelihoodText = _likelihood.ToString("0.##");
                    _parent.MarkDirty();
                    _parent.RefreshLikelihoodSum();
                }
            }
        }
        public TroopData ToTroopData() => new TroopData(IntToLanceTroopCategory(_formationClassValue), _likelihood, _basicTroopId);

        public override void RefreshValues()
        {
            base.RefreshValues();
            LikelihoodText = _likelihood.ToString("0.##");
        }

        public void ExecuteRemove()
        {
            InformationManager.DisplayMessage(new InformationMessage("Remove clicked"));
            _parent.RemoveRow(this);
        }

        public void ExecuteEditLikelihood()
        {
            InformationManager.DisplayMessage(new InformationMessage("EditLikelihood clicked"));
            InformationManager.ShowTextInquiry(new TextInquiryData("Edit Likelihood", "Enter value 0-1", true, true, "Confirm", "Cancel", OnLikelihoodEntered, null, false, null, _likelihood.ToString("0.##"), ""));
        }

        void OnLikelihoodEntered(string input)
        {
            if (double.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var val) || double.TryParse(input, out val))
            {
                if (val < 0 || val > 1)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Likelihood must be between 0 and 1"));
                    return;
                }
                Likelihood = val;
            }
            else InformationManager.DisplayMessage(new InformationMessage("Invalid number"));
        }

        public void ExecuteChangeCategory()
        {
            InformationManager.DisplayMessage(new InformationMessage("ChangeCategory clicked"));
        }

        public void ExecuteFormationClicked()
        {
            InformationManager.DisplayMessage(new InformationMessage("ExecuteFormationClicked pressed"));
        }

        public void ExecuteCategoryClicked()
        {
            InformationManager.DisplayMessage(new InformationMessage("ExecuteCategoryClicked pressed"));
        }

        public void ExecuteChangeTroop()
        {
            InformationManager.DisplayMessage(new InformationMessage("ChangeTroop clicked"));
        }

        public void ExecuteTroopClicked()
        {
            InformationManager.DisplayMessage(new InformationMessage("TroopClicked pressed"));
        }

        public void ExecuteLink()
        {
            InformationManager.DisplayMessage(new InformationMessage("ExecuteLink pressed"));
            ExecuteTroopClicked();
        }

        public void ExecuteBeginHint()
        {
            InformationManager.DisplayMessage(new InformationMessage("ExecuteBeginHint pressed"));
        }

        public void ExecuteEndHint()
        {
            InformationManager.DisplayMessage(new InformationMessage("ExecuteEndHint pressed"));
        }

        public void ExecuteOpenTroopEditor()
        {
            InformationManager.DisplayMessage(new InformationMessage("OpenTroopEditor pressed"));
            _parent.TryOpenTroopEditor(_basicTroopId);
        }
    }
}
