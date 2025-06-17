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

        [CommandLineFunctionality.CommandLineArgumentFunction("set_clan_kingdom", "clanmanager")]
        public static string SetClanKingdom(List<string> strings)
        {
            if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
            {
                return CampaignCheats.ErrorType;
            }
            string text = "Format is \"clanmanager.set_clan_leader [ClanName] | [KingdomName / FirstTwoCharactersOfKingdomName]\".";
            if (CampaignCheats.CheckHelp(strings))
            {
                return text;
            }
            List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
            if (separatedNames.Count < 2)
            {
                return text;
            }
            string clanName = separatedNames[0];
            Clan clan = CampaignCheats.GetClan(clanName);
            if (clan == null || clan.IsEliminated)
            {
                return clanName + " is not found.\n" + text;
            }
            string kingdomName = separatedNames[1].Replace(" ", "");
            Kingdom kingdom = null!;
            foreach (Kingdom k in Kingdom.All)
            {
                if (k.Name.ToString().Equals(kingdomName, StringComparison.OrdinalIgnoreCase))
                {
                    kingdomName = k.Name.ToString();
                    kingdom = k;
                    break;
                }
                if (kingdomName.Length >= 2 && k.Name.ToString().ToLower().Substring(0, 2).Equals(kingdomName.ToLower().Substring(0, 2)))
                {
                    kingdomName = k.Name.ToString();
                    kingdom = k;
                    break;
                }
            }
            if (kingdom == null)
            {
                return kingdomName + " is not found.\n" + text;
            }
            if (kingdom == clan.Kingdom)
            {
                return clanName + " is already in the " + kingdomName + " kingdom.\n";
            }
            if (clan.IsEliminated || clan.Leader == clan.Kingdom.Leader || clan.IsUnderMercenaryService)
            {
                return clanName + " is not a valid candidate to join the " + kingdomName + " kingdom.\nClan must be an active non ruling clan not currently under a mercenary contract.\n" + text;
            }
            if (clan.Kingdom != null)
            {
                ChangeKingdomAction.ApplyByLeaveKingdom(clan, true);
            }
            ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, true);
            return string.Format("{0} has been moved to the {1} kingdom.", clan.Name, kingdom.Name);
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_clan_leader", "clanmanager")]
        public static string SetClanLeader(List<string> strings)
        {
            if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
            {
                return CampaignCheats.ErrorType;
            }
            string text = "Format is \"clanmanager.set_clan_leader [ClanName] | [HeroName]\".";
            if (CampaignCheats.CheckHelp(strings))
            {
                return text;
            }
            List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
            if (separatedNames.Count < 2)
            {
                return text;
            }
            string clanName = separatedNames[0];
            Clan clan = CampaignCheats.GetClan(clanName);
            if (clan == null || clan.IsEliminated)
            {
                return clanName + " is not found.\n" + text;
            }
            if (clan == Clan.PlayerClan)
            {
                return "Can not change leader of the player clan!";
            }
            string heroName = separatedNames[1];
            Hero hero = CampaignCheats.GetHero(heroName);
            if (hero == null) {
                return clanName + " is not found.\n" + text;
            }
            if (clan != hero.Clan || hero.IsChild || hero.IsClanLeader || hero.IsDead)
            {
                return clanName + " is not a valid candidate to lead the " + clanName + " clan.\nHero must be a living member of the target clan, and not already leading a clan.\n" + text;
            }
            clan.SetLeader(hero);
            return string.Format("{0} is the new leader of the {1} clan.", hero.Name, clan.Name);
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_hero_clan", "clanmanager")]
        public static string SetHeroClan(List<string> strings)
        {
            if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
            {
                return CampaignCheats.ErrorType;
            }
            string text = "Format is \"clanmanager.set_hero_clan [HeroName] | [ClanName]\".";
            if (CampaignCheats.CheckHelp(strings))
            {
                return text;
            }
            List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, "|");
            if (separatedNames.Count < 2)
            {
                return text;
            }
            string heroName = separatedNames[0];
            Hero hero = CampaignCheats.GetHero(heroName);
            if (hero == null || !hero.IsActive)
            {
                return heroName + " is not found.\n" + text;
            }
            if (hero.IsClanLeader || hero.IsDead)
            {
                return heroName + "'s clan can not be changed. Hero must be alive and not leading a clan.\n" + text;
            }
            string clanName = separatedNames[1];
            Clan clan = CampaignCheats.GetClan(clanName);
            if (clan == null || clan.IsEliminated)
            {
                return clanName + " is not found.\n" + text;
            }
            if (clan == hero.Clan)
            {
                return heroName + " is already in the " + clanName + " clan.\n";
            }
            hero.Clan = clan;
            return string.Format("{0} has been moved to the {1} clan.", hero.Name, clan.Name);
        }
    }
}