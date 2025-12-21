using System.Collections.Generic;
using System.Linq;

using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

using ClanManager.Actions;
using TaleWorlds.Library;

namespace ClanManager
{
    internal class ClanCreator
    {
        public static void CreateClan(Clan oldClan = null!)
        {
            IEnumerable<CultureObject> worldCultures = MBObjectManager.Instance.GetObjectTypeList<CultureObject>().Where((CultureObject o) => o.IsMainCulture);
            //Select a random world culture. If preservecultures setting is true, select old clan culture if this is also a clan creation on destruction or the culture with the least clans if this is a clan creation on interval.
            CultureObject culture = worldCultures.GetRandomElementInefficiently();
            if (Settings.Current.PreserveCultures)
            {
                if (oldClan == null)
                {
                    Dictionary<CultureObject, int> cultures = new Dictionary<CultureObject, int>();
                    foreach (CultureObject c in worldCultures)
                    {
                        cultures.Add(c, (from d in Campaign.Current.Clans where d.Culture == c select d).Count());
                    }
                    if (!cultures.IsEmpty())
                    {
                        CultureObject toPreserve = (from e in cultures orderby e.Value ascending select e).FirstOrDefault().Key;
                        if (toPreserve != null)
                        {
                            culture = toPreserve;
                        }
                    }
                }
                else
                {
                    culture = oldClan.Culture;
                }
            }
            Settlement settlement = oldClan != null ? oldClan.Leader.HomeSettlement : Settlement.All.GetRandomElementWithPredicate((s) => s.Culture == culture && s.IsTown) ?? SettlementHelper.GetRandomTown();
            TextObject name = NameGenerator.Current.GenerateClanName(culture, settlement);
            if (name == null)
            {
                return;
            }
            int index = Settings.Current.DuplicateClanNames.SelectedIndex;
            if (index < 2 && Clan.All.Count((Clan x) => x.Name.ToString().ToLower() == name.ToString().ToLower() && (index == 0 || !x.IsEliminated)) > 0)
            {

                TextObject message = new TextObject("{=Ns9N34XI}Clan Manager Warning: Attempt to create new clan failed! Please add more names to ModuleData\\CultureClanNames.xml or set 'Duplicate Name Policy' MCM option to 'Eliminated Only' / 'All'.");
                InformationManager.DisplayMessage(new InformationMessage(message.ToString(), Color.FromUint(0x00F16D26)));
                SubModule.Log(message.ToString());
                return;
            }
            CreateClanAction.ApplyByCreateAutonomously(name, oldClan!, culture, settlement);
        }
    }
}
