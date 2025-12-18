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
            if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"clanmanager.add_random_hero_to_clan [ClanName]\".";
            }
            CampaignCheats.TryGetObject(strings[0], out Clan clan, out string str, (Clan c) => !c.IsEliminated);
            if (clan == null)
            {
                return "Clan is not found.";
            }
            CultureObject culture = clan.Culture;
            if (culture == null)
            {
                return "Clan does not have a valid culture. Report this to Clan Manager author on Nexus.\n";
            }
            MBReadOnlyList<CharacterObject> templates = culture.LordTemplates;
            if (templates == null || templates.IsEmpty())
            {
                return "Clan does not have a valid character template. Report this to Clan Manager author on Nexus.\n";
            }
            CharacterObject character = templates.GetRandomElementWithPredicate((CharacterObject x) => x.Occupation == Occupation.Lord);
            if (character == null)
            {
                return "Clan does not have a valid character template. Report this to Clan Manager author on Nexus.\n";
            }
            Settlement settlement = clan.HomeSettlement ?? clan.Leader.HomeSettlement ?? Settlement.All.GetRandomElementWithPredicate((s) => s.Culture == culture && s.IsTown) ?? SettlementHelper.GetRandomTown();
            if (settlement == null)
            {
                return "Clan does not have a valid settlement target. Report this to Clan Manager author on Nexus.\n";
            }
            Hero hero = CreateHeroAction.ApplyInternal(character, settlement, clan, culture, MBRandom.RandomInt(Settings.Current.MinimumHeroAge, Settings.Current.MaximumHeroAge));
            EnterSettlementAction.ApplyForCharacterOnly(hero, settlement);
            GiveGoldAction.ApplyBetweenCharacters(null, clan.Leader, Settings.Current.ExtraStartingGoldPerHero * 1000, true);
            return string.Format("{0} is added to the {1} clan.", hero.Name, clan.Name);
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_clan_kingdom", "clanmanager")]
        public static string SetClanKingdom(List<string> strings)
        {
            if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
            {
                return CampaignCheats.ErrorType;
            }
            string text = "Format is \"clanmanager.set_clan_kingdom [ClanName] | [KingdomName / FirstTwoCharactersOfKingdomName]\".";
            if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckParameters(strings, 2) || CampaignCheats.CheckHelp(strings))
            {
                return text;
            }
            List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings);
            if (separatedNames.Count != 2)
            {
                return text;
            }
            CampaignCheats.TryGetObject(separatedNames[0], out Clan clan, out string str, (Clan c) => !c.IsEliminated);
            if (clan == null)
            {
                return "Clan is not found.\n" + text;
            }
            string kingdomName = separatedNames[1].Replace(" ", "");
            CampaignCheats.TryGetObject(separatedNames[1], out Kingdom kingdom, out string str2, (Kingdom k) => !k.IsEliminated);
            if (kingdom == null)
            {
                return "Kingdom is not found.\n" + text;
            }
            if (kingdom == clan.Kingdom)
            {
                return "Clan is already in the kingdom.\n";
            }
            if (clan.Leader == clan.Kingdom.Leader || clan.IsUnderMercenaryService)
            {
                return "Clan is not a valid candidate to join the kingdom.\nClan must be an active non ruling clan not currently under a mercenary contract.\n" + text;
            }
            if (clan.Kingdom != null)
            {
                ChangeKingdomAction.ApplyByLeaveKingdom(clan);
            }
            ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom);
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
            if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckParameters(strings, 2) || CampaignCheats.CheckHelp(strings))
            {
                return text;
            }
            List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings);
            if (separatedNames.Count < 2)
            {
                return text;
            }
            CampaignCheats.TryGetObject(separatedNames[0], out Clan clan, out string str, (Clan c) => !c.IsEliminated);
            if (clan == null || clan.IsEliminated)
            {
                return "Clan is not found.\n" + text;
            }
            if (clan == Clan.PlayerClan)
            {
                return "Can not change leader of the player clan!";
            }
            CampaignCheats.TryGetObject(separatedNames[1], out Hero hero, out string str2, (Hero x) => x.IsAlive);
            if (hero == null) {
                return "Clan is not found.\n" + text;
            }
            if (clan != hero.Clan || hero.IsChild || hero.IsClanLeader || hero.IsDead)
            {
                return "Clan is not a valid candidate to lead.\nHero must be a living, adult member of the target clan, and not already leading a clan.\n" + text;
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
            if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckParameters(strings, 2) || CampaignCheats.CheckHelp(strings))
            {
                return text;
            }
            List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings);
            if (separatedNames.Count < 2)
            {
                return text;
            }
            CampaignCheats.TryGetObject(separatedNames[0], out Hero hero, out string str, (Hero x) => x.IsAlive);
            if (hero == null || !hero.IsActive)
            {
                return "Hero is not found.\n" + text;
            }
            if (hero.IsClanLeader || hero.IsDead)
            {
                return "Hero's clan can not be changed. Hero must be alive and not leading a clan.\n" + text;
            }
            CampaignCheats.TryGetObject(separatedNames[1], out Clan clan, out string str2, (Clan c) => !c.IsEliminated);
            if (clan == null || clan.IsEliminated)
            {
                return "Clan is not found.\n" + text;
            }
            if (clan == hero.Clan)
            {
                return "Hero is already in the clan.\n";
            }
            hero.Clan = clan;
            return string.Format("{0} has been moved to the {1} clan.", hero.Name, clan.Name);
        }
    }
}