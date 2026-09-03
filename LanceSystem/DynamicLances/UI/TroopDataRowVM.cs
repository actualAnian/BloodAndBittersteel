using LanceSystem.Deserialization;
using LanceSystem.UI;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace LanceSystem.DynamicLances.UI
{
    public class TroopDataRowVM : ViewModel
    {
        static readonly TextObject _removeClickedText = new("{=lance_remove_clicked}Remove clicked");
        static readonly TextObject _editLikelihoodTitle = new("{=lance_edit_likelihood_title}Edit Likelihood");
        static readonly TextObject _editLikelihoodHint = new("{=lance_edit_likelihood_hint}Enter value between 0-1");
        static readonly TextObject _likelihoodRangeError = new("{=lance_likelihood_range_error}Likelihood must be between 0 and 1");
        static readonly TextObject _invalidNumberText = new("{=lance_invalid_number}Invalid number");
        readonly LanceTemplateEditorVM _parent;
        readonly LanceTemplateEditorController _controller;
        int _formationClassValue;
        string _basicTroopId;
        string _removeButtonText;
        double _likelihood;
        string _troopName;
        ImageIdentifierVM _troopVisual;
        string _likelihoodText = "";

        public TroopDataRowVM(LanceTemplateEditorVM parent, LanceTemplateEditorController controller, TroopData data)
        {
            _parent = parent;
            _controller = controller;
            _formationClassValue = GetFormationClassValueFromCategory(data.Category);
            _basicTroopId = data.BasicTroopId;
            _likelihood = data.Likelihood;
            var character = TaleWorlds.ObjectSystem.MBObjectManager.Instance.GetObject<CharacterObject>(_basicTroopId);
            _troopName = character.Name.ToString();
            _troopVisual = CreateVisual(character);
            RemoveButtonText = UITexts.Remove.ToString();
            RefreshValues();
        }

        ImageIdentifierVM CreateVisual(CharacterObject character)
        {
            if (character != null)
                return new CharacterImageIdentifierVM(CharacterCode.CreateFrom(character));
            return new ItemImageIdentifierVM(null);
        }
        void UpdateCharacter()
        {
            var character = TaleWorlds.ObjectSystem.MBObjectManager.Instance.GetObject<CharacterObject>(_basicTroopId);
            TroopName = character.Name.ToString();
            TroopVisual = CreateVisual(character);
        }
        public int GetFormationClassValueFromCategory(LanceTroopCategory category) => category.ToInt();

        [DataSourceProperty]
        public int FormationClassValue
        {
            get
            {
                return _formationClassValue;
            }
            set
            {
                if (value != _formationClassValue)
                {
                    _formationClassValue = value;
                    OnPropertyChangedWithValue(value, "FormationClassValue");
                    _parent.MarkDirty();
                }
            }
        }
        [DataSourceProperty]
        public string OpenButtonText
        {
            get => UITexts.Open.ToString();
        }

        [DataSourceProperty]
        public string BasicTroopId
        {
            get => _basicTroopId;
            set { if (value != _basicTroopId) { _basicTroopId = value; OnPropertyChangedWithValue(value, nameof(BasicTroopId)); UpdateCharacter(); _parent.MarkDirty(); } }
        }

        [DataSourceProperty]
        public string TroopName
        {
            get => _troopName;
            set { if (value != _troopName) { _troopName = value; OnPropertyChangedWithValue(value, nameof(TroopName)); } }
        }

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
        [DataSourceProperty]
        public string RemoveButtonText { get => _removeButtonText; set { if (value != _removeButtonText) { _removeButtonText = value; OnPropertyChangedWithValue(value, nameof(RemoveButtonText)); } } }

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
        public TroopData ToTroopData() => new TroopData(_formationClassValue.ToCategory(), _likelihood, _basicTroopId);

        public override void RefreshValues()
        {
            base.RefreshValues();
            LikelihoodText = _likelihood.ToString("0.##");
        }

        public void ExecuteRemove()
        {
            InformationManager.DisplayMessage(new InformationMessage(_removeClickedText.ToString()));
            _parent.RemoveRow(this);
        }

        public void ExecuteEditLikelihood()
        {
            InformationManager.ShowTextInquiry(new TextInquiryData(_editLikelihoodTitle.ToString(), _editLikelihoodHint.ToString(), true, true, UITexts.Confirm.ToString(), GameTexts.FindText("str_cancel").ToString(), OnLikelihoodEntered, null, false, null, _likelihood.ToString("0.##"), ""));
        }
        void OnLikelihoodEntered(string input)
        {
            if (double.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var val) || double.TryParse(input, out val))
            {
                if (val < 0 || val > 1)
                {
                    InformationManager.DisplayMessage(new InformationMessage(_likelihoodRangeError.ToString()));
                    return;
                }
                Likelihood = val;
            }
            else InformationManager.DisplayMessage(new InformationMessage(_invalidNumberText.ToString()));
        }
        public void ExecuteChangeCategory()
        {
            _controller.ChangeCategoryForRow(this);
        }
        public void ExecuteTroopClicked()
        {
            _controller.SelectTroopForRow(this, _formationClassValue);
        }
        public void ExecuteLink()
        {
            ExecuteTroopClicked();
        }
        public void ExecuteOpenTroopEditor()
        {
            _parent.TryOpenTroopEditor(_basicTroopId);
        }
    }
}
