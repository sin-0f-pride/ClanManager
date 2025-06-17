using System.Collections.Generic;
using System.Xml;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace ClanManager.GauntletUI
{
    internal class EncyclopediaClanPageFormatPatch
    {
        [PrefabExtension("EncyclopediaClanPage", "descendant::ListPanel[@SuggestedWidth='370']/Children")]
        internal sealed class EncyclopediaClanPageChangeName : PrefabExtensionInsertPatch
        {
            public override InsertType Type => InsertType.Child;
            public override int Index => 0;

            private readonly XmlDocument _document;

            public EncyclopediaClanPageChangeName()
            {
                _document = new XmlDocument();
                _document.LoadXml(@"<ChangeNameEncyclopediaClanPage />");
            }

            [PrefabExtensionXmlDocument]
            public XmlDocument GetPrefabExtension() => _document;
        }
        [PrefabExtension("EncyclopediaClanPage", "descendant::ButtonWidget[@Id='BookmarkButton']")]
        internal class EncyclopediaClanPageBookmarkFormat : PrefabExtensionSetAttributePatch
        {

            public override List<Attribute> Attributes => new()
            {
                new Attribute("MarginTop", "-12")
            };
        }
        [PrefabExtension("EncyclopediaClanPage", "descendant::RichTextWidget[@Text='@NameText']")]
        internal class EncyclopediaClanPageNameTextFormat : PrefabExtensionSetAttributePatch
        {

            public override List<Attribute> Attributes => new()
            {
                new Attribute("MarginLeft", "10")
            };
        }
        [PrefabExtension("EncyclopediaClanPage", "descendant::TextWidget[@Text='@PartOfText']")]
        internal class EncyclopediaClanPagePartOfFactionTextFormat : PrefabExtensionSetAttributePatch
        {

            public override List<Attribute> Attributes => new()
            {
                new Attribute("MarginTop", "-10")
            };
        }
        [PrefabExtension("EncyclopediaClanPage", "descendant::BrushWidget[@SuggestedHeight='150']")]
        internal sealed class EncyclopediaClanPageHideShadow : PrefabExtensionSetAttributePatch
        {
            public override List<Attribute> Attributes => new()
            {
                new Attribute("IsHidden", "true")
            };
        }
    }
}
