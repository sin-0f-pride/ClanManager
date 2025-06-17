using System;
using System.Collections.Generic;
using System.Linq;

using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

using ClanManager.Actions;

namespace ClanManager
{
    internal class Commands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("add_random_hero_to_clan", "clanmanager")]
        public static string AddRandomHeroToClan(List<string> strings)
        {
            if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
            {
                return CampaignCheats.ErrorType;
            }
            string text = "Format is \"clanmanager.add_random_hero_to_clan [ClanName]\".";
            if (CampaignCheats.CheckHelp(strings) || strings.Count != 1)
            {
                return text;
            }
            Clan clan = null!;
            string clanName = "";
            if (!CampaignCheats.CheckParameters(strings, 0))
            {
                clanName = CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count));
                clan = CampaignCheats.GetClan(clanName);
            }
            if (clan == null || clan.IsEliminated)
            {
                return clanName + " is not found.\n" + text;
            }
            CultureObject culture = clan.Culture;
            if (culture == null)
            {
                return clanName + " does not have a valid culture. Report this to Clan Manager author on Nexus.\n";
            }
            MBReadOnlyList<CharacterObject> templates = culture.LordTemplates;
            if (templates == null || templates.IsEmpty())
            {
                return clanName + " does not have a valid character template. Report this to Clan Manager author on Nexus.\n";
            }
            CharacterObject character = templates.GetRandomElementWithPredicate((CharacterObject x) => x.Occupation == Occupation.Lord);
            if (character == null)
            {
                return clanName + " does not have a valid character template. Report this to Clan Manager author on Nexus.\n";
            }
            Settlement settlement = clan.HomeSettlement ?? clan.Leader.HomeSettlement ?? Settlement.All.GetRandomElementWithPredicate((s) => s.Culture == culture && s.IsTown) ?? SettlementHelper.GetRandomTown();
            if (settlement == null)
            {
                return clanName + " does not have a valid settlement target. Report this to Clan Manager author on Nexus.\n";
            }
            Hero hero = CreateHeroAction.ApplyInternal(character, settlement, clan, MBRandom.RandomInt(Settings.Current.MinimumHeroAge, Settings.Current.MaximumHeroAge));
            EnterSettlementAction.ApplyForCharacterOnly(hero, settlement);
            GiveGoldAction.ApplyBetweenCharacters(null, clan.Leader, (int)(Settings.Current.ExtraStartingGoldPerHero) * 1000, true);
            return string.Format("{0} is added to the {1} clan.", hero.Name, clan.Name);
        }
    }
}