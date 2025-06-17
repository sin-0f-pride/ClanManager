using System.Collections.Generic;
using System.Linq;
using System.Xml;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;

namespace ClanManager.Behaviors
{
    internal class ClanCreationBehavior : CampaignBehaviorBase
    {
        private ClanCreatorData _clanCreatorData;

        public static Dictionary<CultureObject, List<TextObject>> Names = new Dictionary<CultureObject, List<TextObject>>();

        public ClanCreationBehavior()
        {
            _clanCreatorData ??= new();
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, OnClanDestroyed);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, Deserialize);
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, Deserialize);
        }

        //Create new clan with the mod settings whenever a clan is destroyed 
        private void OnClanDestroyed(Clan c)
        {
            if (c == null || !Settings.Current.EnableSpawnOnDestruction || !Settings.Current.MinorClansOnDestruction && (c.IsMinorFaction || c.IsClanTypeMercenary || c.Tier == 0)) return;
            for (int n = 0; n < Settings.Current.NumberOfClansOnDestruction; n++)
            {
                ClanCreator.CreateClan(c);
            }
        }

        //Create new clan with the mod settings whenever the interval is passed 
        private void OnHourlyTick()
        {
            if (!Settings.Current!.EnableSpawnOnInterval) return;
            if (_clanCreatorData.HasExceededCooldown())
            {
                for (int n = 0; n < Settings.Current.NumberOfClansOnInterval; n++)
                {
                    ClanCreator.CreateClan();
                }
            }
        }

        private void Deserialize(CampaignGameStarter starter)
        {
            if (Settings.Current!.CustomClanNames.SelectedIndex == 0)
            {
                return;
            }
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreComments = true;
            string path = ModuleHelper.GetModuleFullPath("ClanManager") + "ModuleData/CultureClanNames.xml";
            using (XmlReader reader = XmlReader.Create(path, readerSettings))
            {
                XmlDocument document = new XmlDocument();
                document.Load(reader);
                XmlNode root = document.ChildNodes[1];
                foreach (XmlNode c in root.ChildNodes)
                {
                    CultureObject culture = MBObjectManager.Instance.GetObjectTypeList<CultureObject>().Where((d) => d.IsMainCulture && d.Name.ToString().ToLower() == c.Attributes["id"].Value).FirstOrDefault();
                    if (culture != null)
                    {
                        List<TextObject> _clanNameList = new List<TextObject>();
                        foreach (XmlNode node in c.ChildNodes)
                        {
                            if (node.Name == "names")
                            {
                                foreach (XmlNode child in node.ChildNodes)
                                {
                                    TextObject name = new TextObject(child.Attributes["name"].Value);
                                    if (!_clanNameList.Contains(name))
                                    {
                                        _clanNameList.Add(name);
                                    }
                                }
                            }
                            else if (node.Name == "words")
                            {
                                IEnumerable<string> wordsToNames = new List<string> { null };
                                foreach (XmlNode child in node.ChildNodes)
                                {
                                    if (child.HasChildNodes)
                                    {
                                        if (child.Name == "word1")
                                        {
                                            List<string> _wordList1 = new List<string>();
                                            foreach (XmlNode word in child.ChildNodes)
                                            {
                                                _wordList1.Add(word.Attributes["word"].Value);
                                            }
                                            wordsToNames = wordsToNames.SelectMany(o => _wordList1.Select(s => o + (s != "" ? s + " " : "")));
                                        }
                                        else if (child.Name == "word2")
                                        {
                                            List<string> _wordList2 = new List<string>();
                                            foreach (XmlNode word in child.ChildNodes)
                                            {
                                                _wordList2.Add(word.Attributes["word"].Value);
                                            }
                                            wordsToNames = wordsToNames.SelectMany(o => _wordList2.Select(s => o + (s != "" ? s + " " : "")));
                                        }
                                        else if (child.Name == "word3")
                                        {
                                            List<string> _wordList3 = new List<string>();
                                            foreach (XmlNode word in child.ChildNodes)
                                            {
                                                _wordList3.Add(word.Attributes["word"].Value);
                                            }
                                            wordsToNames = wordsToNames.SelectMany(o => _wordList3.Select(s => o + (s != "" ? s + " " : "")));
                                        }
                                    }
                                }
                                if (wordsToNames.ElementAtOrDefault(0) != null)
                                {
                                    List<TextObject> names = new List<TextObject>();
                                    foreach (string n in wordsToNames)
                                    {
                                        TextObject name = new TextObject(n.Trim());
                                        if (!_clanNameList.Contains(name))
                                        {
                                            _clanNameList.Add(name); // Trimming is hacky, i should fix the selectmany logic.
                                        }
                                    }
                                }
                            }
                        }
                        Names.Add(culture, _clanNameList);
                    }
                }
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_clanCreatorData", ref _clanCreatorData);

            if (dataStore.IsLoading)
            {
                _clanCreatorData ??= new();
            }
        }
    }
}
