using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.LinQuick;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using System;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.GameComponents;

namespace ClanManager.ClanCreator
{
    public class ClanCreator
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
            if (name == null || (Settings.Current.DuplicateClanNames.SelectedIndex < 2 && Clan.All.Count((Clan x) => x.Name.ToString().ToLower() == name.ToString().ToLower() && (Settings.Current.DuplicateClanNames.SelectedIndex == 0 || !x.IsEliminated)) > 0))
            {
                return;
            }
            Clan clan = Clan.CreateClan("CC_" + Clan.All.Count);
            clan.InitializeClan(name, name, culture, Banner.CreateRandomClanBanner(-1), settlement.GatePosition, false);
            clan.UpdateHomeSettlement(settlement);
            MBReadOnlyList<CharacterObject> templates = culture.LordTemplates;
            CharacterObject character = templates.GetRandomElementWithPredicate((CharacterObject x) => x.Occupation == Occupation.Lord);
            bool mercenary = false;
            if ((oldClan != null && (oldClan.IsMinorFaction || oldClan.IsClanTypeMercenary) && Settings.Current.MinorClansOnDestruction) || (oldClan == null && MBRandom.RandomInt(1, 100) <= Settings.Current.MinorClanFrequencyOnInterval))
            {
                clan.GetType().GetProperty("IsMinorFaction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(clan, true);
                templates = culture.RebelliousHeroTemplates ?? templates;
                character = templates.GetRandomElementInefficiently() ?? character;
                mercenary = (oldClan != null && oldClan.IsClanTypeMercenary) || (oldClan == null && MBRandom.RandomInt(0, 1) == 0);
                if (mercenary)
                {
                    clan.GetType().GetProperty("IsClanTypeMercenary", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(clan, true);
                }
            }
            if (character == null)
            {
                return;
            }
            int minTraitLevel = Settings.Current.MinimumPersonalityTraitLevel;
            int maxTraitLevel = Settings.Current.MaximumPersonalityTraitLevel;
            Hero leader = CreateNewHero(character, settlement, clan, MBRandom.RandomInt(Settings.Current.MinimumLeaderHeroAge, Settings.Current.MaximumLeaderHeroAge));
            clan.SetLeader(leader);
            int minimumTier = Settings.Current.MinimumClanTier;
            int maximumTier = Settings.Current.MaximumClanTier;
            if (oldClan != null && Settings.Current.PreserveClanTier)
            {
                minimumTier = oldClan.Tier;
                maximumTier = oldClan.Tier;
            }
            int tier = minimumTier <= maximumTier ? MBRandom.RandomInt(minimumTier, maximumTier) : MBRandom.RandomInt(maximumTier, minimumTier);
            int[] TierLowerRenown = new int[] { 0, 50, 150, 350, 900, 2350, 6150 };
            clan.AddRenown(TierLowerRenown[tier]);
            clan.Renown = Campaign.Current.Models.ClanTierModel.CalculateInitialRenown(clan);
            EnterSettlementAction.ApplyForCharacterOnly(leader, settlement);
            GiveGoldAction.ApplyBetweenCharacters(null, leader, (int)(Settings.Current.StartingGoldForLeader) * 1000, false);
            int j = MBRandom.RandomInt(Settings.Current.MinimumHeroesSpawned, Settings.Current.MaximumHeroesSpawned);
            for (int i = 0; i < j; i++)
            {
                Hero hero = CreateNewHero(character, settlement, clan, MBRandom.RandomInt(Settings.Current.MinimumHeroAge, Settings.Current.MaximumHeroAge));
                EnterSettlementAction.ApplyForCharacterOnly(hero, settlement);
                GiveGoldAction.ApplyBetweenCharacters(null, leader, (int)(Settings.Current.ExtraStartingGoldPerHero) * 1000, true);
            }
            //Decides whether troops should be added based on the setting, then creates a single party for the clan and populates it with culture troops until the clan strength is met or close to being met without going over.
            Hero strongest = null;
            int strongestSum = 0;
            int parties = clan.CommanderLimit;
            for (int p = 0; p < clan.CommanderLimit; p++)
            {
                foreach (Hero h in clan.Heroes)
                {
                    if (h.IsPartyLeader)
                    {
                        continue;
                    }
                    int sum = 0;
                    foreach (KeyValuePair<SkillObject, int> skill in GetAllSkillLevels(h))
                    {
                        sum += skill.Value;
                    }
                    if (sum > strongestSum)
                    {
                        strongest = h;
                        strongestSum = sum;
                    }
                }
                if (strongest == null)
                {
                    break;
                }
                MobileParty party = LordPartyComponent.CreateLordParty("CC_" + MobileParty.All.Count, strongest, settlement.GatePosition, 150f, settlement, strongest);
                int settingIndex = Settings.Current.ClanStrength.SelectedIndex;
                if (settingIndex != 0)
                {
                    party.AddElementToMemberRoster(settingIndex == 1 || (settingIndex == 2 && MBRandom.RandomInt(1) == 0) ? culture.BasicTroop : culture.EliteBasicTroop, party.LimitedPartySize - party.MemberRoster.Count);
                }
            }
            //Don't preserve kingdom for mercenaries
            if (Settings.Current.PreserveKingdoms && !mercenary)
            {
                // Preserve old clan kingdom. If it's null, preserve the kingdom with the least active clans. If the kingdom is owned by the player, give them the option. If the player refuses, move to next kingdom.
                IEnumerable<Kingdom> kingdoms = from k in Kingdom.All orderby (from j in k.Clans where !j.IsEliminated select j).Count() ascending select k;
                Kingdom kingdom = oldClan != null && oldClan.Kingdom != null ? oldClan.Kingdom : kingdoms.ElementAtOrDefault(0);
                if (kingdom != null)
                {
                    if (Settings.Current.PreservePlayerKingdomInquiry && kingdom.Leader == Hero.MainHero)
                    {
                        InformationManager.ShowInquiry(
                            new InquiryData(
                                new TextObject("A new clan emerged").ToString(),
                                new TextObject("A rising new clan, {ClanName}, has been spotted nearby and would like to serve our kingdom. Do you accept them?").SetTextVariable("ClanName", clan.Name).ToString(),
                                true, true,
                                new TextObject("Accept").ToString(),
                                new TextObject("Refuse").ToString(),
                                () =>
                                {
                                    ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom);
                                },
                                () =>
                                {
                                    ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdoms.ElementAtOrDefault(1));
                                }
                            ), true);
                    }
                    else
                    {
                        ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom);
                    }
                }
            }
            TextObject message = new TextObject("A rising new clan, {ClanName}, has been spotted near {SettlementName}.").SetTextVariable("ClanName", clan.Name).SetTextVariable("SettlementName", settlement.Name);
            InformationManager.DisplayMessage(new InformationMessage(message.ToString(), Color.White));
        }

        private static Hero CreateNewHero(CharacterObject template, Settlement bornSettlement, Clan clan, int age)
        {
            bool IsFemale = MBRandom.RandomInt(1, 100) <= (int)(Settings.Current.FemaleChance * 100);
            template.IsFemale = IsFemale;
            CharacterObject character = CharacterObject.CreateFrom(template);
            Hero hero = Hero.CreateHero(character.StringId);
            hero.SetCharacterObject(character);
            character.GetType().GetProperty("HeroObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(character, hero);
            CampaignTime birthday = HeroHelper.GetRandomBirthDayForAge(age);
            hero.SetBirthDay(birthday);
            hero.BornSettlement = bornSettlement;
            hero.GetType().GetProperty("StaticBodyProperties", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(hero, BodyProperties.GetRandomBodyProperties(template.Race, IsFemale, template.GetBodyPropertiesMin(false), template.GetBodyPropertiesMax(), 0, MBRandom.RandomInt(1, 2000), character.HairTags, character.BeardTags, character.TattooTags).StaticProperties);
            hero.UpdatePlayerGender(IsFemale);
            if (character.Age >= Campaign.Current.Models.AgeModel.HeroComesOfAge)
            {
                int minTraitLevel = Settings.Current.MinimumPersonalityTraitLevel;
                int maxTraitLevel = Settings.Current.MaximumPersonalityTraitLevel;
                hero.SetTraitLevel(DefaultTraits.Mercy, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
                hero.SetTraitLevel(DefaultTraits.Calculating, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
                hero.SetTraitLevel(DefaultTraits.Valor, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
                hero.SetTraitLevel(DefaultTraits.Honor, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
                hero.SetTraitLevel(DefaultTraits.Generosity, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
                hero.HeroDeveloper.InitializeHeroDeveloper();
            }
            hero.SetNewOccupation(Occupation.Lord);
            MBEquipmentRoster roster = Campaign.Current.Models.EquipmentSelectionModel.GetEquipmentRostersForHeroComeOfAge(hero, false).GetRandomElementInefficiently();
            if (roster != null)
            {
                IEnumerable<Equipment> equipItems = roster.GetBattleEquipments();
                if (equipItems != null && equipItems.Count() > 0)
                {
                    Equipment equip = new Equipment(false);
                    equip.FillFrom(equipItems.GetRandomElementInefficiently(), false);
                    EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, equip);
                }
            }
            NameGenerator.Current.GenerateHeroNameAndHeroFullName(hero, out TextObject firstName, out TextObject fullName, false);
            hero.SetName(fullName, firstName);
            hero.Clan = clan;
            CampaignEventDispatcher.Instance.OnHeroCreated(hero);
            BodyProperties bodyPropertiesMin = hero.CharacterObject.GetBodyPropertiesMin(true);
            DynamicBodyProperties dynamicBodyProperties = bodyPropertiesMin.DynamicProperties;
            if (dynamicBodyProperties == DynamicBodyProperties.Invalid)
            {
                dynamicBodyProperties = DynamicBodyProperties.Default;
            }
            hero.Build = dynamicBodyProperties.Build;
            hero.Weight = dynamicBodyProperties.Weight;
            hero.Level = 0;
            TextObject text = new TextObject("{=9Obe3S6L}{NAME} is a member of the {CLAN_NAME}, a rising new clan. {GENDER} {REPUTATION}.");
            text.SetTextVariable("NAME", firstName);
            text.SetTextVariable("CLAN_NAME", clan.Name);
            text.SetTextVariable("GENDER", IsFemale ? "She" : "He");
            if (hero.GetTraitLevel(DefaultTraits.Mercy) == 0 && hero.GetTraitLevel(DefaultTraits.Honor) == 0 && hero.GetTraitLevel(DefaultTraits.Generosity) == 0 && hero.GetTraitLevel(DefaultTraits.Valor) == 0 && hero.GetTraitLevel(DefaultTraits.Calculating) == 0)
            {
                TextObject reputation = new TextObject("is still making {GENDER} reputation.");
                reputation.SetTextVariable("GENDER", IsFemale ? "her" : "his");
                text.SetTextVariable("REPUTATION", reputation);
            }
            else
            {
                text.SetTextVariable("REPUTATION", "has the reputation of being " + CharacterHelper.GetReputationDescription(hero.CharacterObject));
            }
            hero.EncyclopediaText = text;
            hero.UpdateHomeSettlement();
            hero.ChangeState(Hero.CharacterStates.Active); 
            return hero;
        }

        private static Dictionary<SkillObject, int> GetAllSkillLevels(Hero h)
        {
            Dictionary<SkillObject, int> dictionary = new Dictionary<SkillObject, int>
            {
                { DefaultSkills.Engineering, h.GetSkillValue(DefaultSkills.Engineering) },
                { DefaultSkills.Medicine, h.GetSkillValue(DefaultSkills.Medicine) },
                { DefaultSkills.Leadership, h.GetSkillValue(DefaultSkills.Leadership) },
                { DefaultSkills.Steward, h.GetSkillValue(DefaultSkills.Steward) },
                { DefaultSkills.Trade, h.GetSkillValue(DefaultSkills.Trade) },
                { DefaultSkills.Charm, h.GetSkillValue(DefaultSkills.Charm) },
                { DefaultSkills.Roguery, h.GetSkillValue(DefaultSkills.Roguery) },
                { DefaultSkills.Scouting, h.GetSkillValue(DefaultSkills.Scouting) },
                { DefaultSkills.Tactics, h.GetSkillValue(DefaultSkills.Tactics) },
                { DefaultSkills.Crafting, h.GetSkillValue(DefaultSkills.Crafting) },
                { DefaultSkills.Athletics, h.GetSkillValue(DefaultSkills.Athletics) },
                { DefaultSkills.Riding, h.GetSkillValue(DefaultSkills.Riding) },
                { DefaultSkills.Throwing, h.GetSkillValue(DefaultSkills.Throwing) },
                { DefaultSkills.Crossbow, h.GetSkillValue(DefaultSkills.Crossbow) },
                { DefaultSkills.Bow, h.GetSkillValue(DefaultSkills.Bow) },
                { DefaultSkills.Polearm, h.GetSkillValue(DefaultSkills.Polearm) },
                { DefaultSkills.TwoHanded, h.GetSkillValue(DefaultSkills.TwoHanded) },
                { DefaultSkills.OneHanded, h.GetSkillValue(DefaultSkills.OneHanded) }
            };
            return dictionary;
        }
    }
}
