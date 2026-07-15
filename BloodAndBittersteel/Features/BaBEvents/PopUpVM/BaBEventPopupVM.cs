using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpVM
{
    public class BaBEventPopupVM : ViewModel
    {
        private string _title;
        private string _smallText;
        private string _spriteName;
        int _height;

        const int AverageCharactersPerLine = 160;
        const int BaseHeight = 800;
        const int HeightPerLine = 15;
        public BaBEventPopupVM(string title, string smallText, string spriteName)
        {

            Title = title;
            SmallText = smallText;
            SpriteName = spriteName;
        }

        public void Close()
        {

            //RefreshValues();
            GameStateManager.Current.PopState();
        }

        public void Refresh()
        {
            Title = _title;
            SmallText = _smallText;
            SpriteName = _spriteName;
        }

        [DataSourceProperty]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                base.OnPropertyChangedWithValue(value, "Title");
            }
        }
        private int GetHeightFromDescriptionLines()
        {
            return SmallText.Length / AverageCharactersPerLine * HeightPerLine;
        }
        [DataSourceProperty]
        public int Height
        {
            get
            {
                return BaseHeight + GetHeightFromDescriptionLines();
            }
            set
            {
                _height = value;
                base.OnPropertyChangedWithValue(value, "Height");
            }
        }
        [DataSourceProperty]
        public string SmallText
        {
            get
            {
                return _smallText;
            }
            set
            {
                _smallText = value;
                base.OnPropertyChangedWithValue(value, "PopupSmallText");
            }
        }
        [DataSourceProperty]
        public string SpriteName
        {
            get
            {
                return _spriteName;
            }
            set
            {
                _spriteName = value;
                base.OnPropertyChangedWithValue(value, "SpriteName");
            }
        }

    }
}
