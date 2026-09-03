using System.Xml;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace LanceSystem.ClanScreenUI
{
    [PrefabExtension("ClanScreen", "descendant::ClanScreenWidget/Children")]
    public class ClanScreenLancesPatch : PrefabExtensionInsertPatch
    {
        readonly XmlDocument _document;
        public override InsertType Type => InsertType.Child;
        public override int Index => 999;
        public ClanScreenLancesPatch()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"
<Widget Id=""LancesEditorsPanel"" WidthSizePolicy=""Fixed"" HeightSizePolicy=""CoverChildren"" SuggestedWidth=""265"" HorizontalAlignment=""Right"" VerticalAlignment=""Center"" MarginRight=""10"" PositionYOffset=""-160"">
  <Children>
    <ListPanel WidthSizePolicy=""StretchToParent"" HeightSizePolicy=""CoverChildren"" StackLayout.LayoutMethod=""VerticalTopToBottom"">
      <Children>
        <ButtonWidget WidthSizePolicy=""StretchToParent"" HeightSizePolicy=""Fixed"" SuggestedHeight=""50"" Brush=""ButtonBrush1"" Command.Click=""ExecuteOpenTroopEditor"" DoNotPassEventsToChildren=""true"">
          <Children>
            <TextWidget WidthSizePolicy=""StretchToParent"" HeightSizePolicy=""StretchToParent"" HorizontalAlignment=""Center"" VerticalAlignment=""Center"" Brush=""Button.Text"" Text=""Troop Editor""  />
          </Children>
        </ButtonWidget>
        <ButtonWidget WidthSizePolicy=""StretchToParent"" HeightSizePolicy=""Fixed"" SuggestedHeight=""50"" Brush=""ButtonBrush1"" Command.Click=""ExecuteOpenLanceTemplateEditor"" MarginTop=""30"" DoNotPassEventsToChildren=""true"">
          <Children>
            <TextWidget WidthSizePolicy=""StretchToParent"" HeightSizePolicy=""StretchToParent"" HorizontalAlignment=""Center"" VerticalAlignment=""Center"" Brush=""Button.Text"" Text=""Lance Editor"" />
          </Children>
        </ButtonWidget>
      </Children>
    </ListPanel>
  </Children>
</Widget>");
        }

        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
}
