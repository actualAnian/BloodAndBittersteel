using System;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents
{
    public class BaBEventVM : ViewModel
    {
        private readonly Action _onClose;

        private string _title;
        private string _smallText;
        private string _spriteName;

        const int AverageCharactersPerLine = 160;
        const int BaseHeight = 800;
        const int HeightPerLine = 15;
        public BaBEventVM(string title, string smallText, string spriteName, Action onClose)
        {
            _onClose = onClose;
            _title = title;
            _smallText = smallText;
            _spriteName = spriteName;
        }

        [DataSourceProperty]
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChangedWithValue(value, nameof(Title)); }
        }

        [DataSourceProperty]
        public string SmallText
        {
            get => _smallText;
            set { _smallText = value; OnPropertyChangedWithValue(value, nameof(SmallText)); }
        }

        [DataSourceProperty]
        public string SpriteName
        {
            get => _spriteName;
            set { _spriteName = value; OnPropertyChangedWithValue(value, nameof(SpriteName)); }
        }

        [DataSourceProperty]
        public int Height
        {
            get => BaseHeight + (SmallText?.Length ?? 0) / AverageCharactersPerLine * HeightPerLine;
        }

        public void Close()
        {
            _onClose?.Invoke();
        }
        public void ExecuteClose()
        {
            OnFinalize();
            _onClose?.Invoke();
        }
        public override void OnFinalize()
        {
            base.OnFinalize();
            // cleanup bindings
        }
    }
}
