using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.Library;

namespace ClanManager.Actions
{
    public static class CreateClanAction
    {
        public enum CreateClanDetail
        {
            ByCreateAutonomously,
            ByTurningToLord
        }

        private static void ApplyInternal(TextObject name, Clan oldClan, CultureObject culture, Hero leader, Settlement settlement, CreateClanDetail detail)
        {
            Clan clan = Clan.CreateClan("CC_" + Clan.All.Count);
            clan.InitializeClan(name, name, culture, Banner.CreateRandomClanBanner(-1), settlement.GatePosition, false);
            clan.UpdateHomeSettlement(settlement);
            MBReadOnlyList<CharacterObject> templates = culture.LordTemplates;
            CharacterObject character = templates.GetRandomElementWithPredicate((CharacterObject x) => x.Occupation == Occupation.Lord);
            bool mercenary = false;
            if (detail == CreateClanDetail.ByCreateAutonomously)
            {
                if ((oldClan != null && (oldClan.IsMinorFaction || oldClan.IsClanTypeMercenary) && Settings.Current.MinorClansOnDestruction) || (oldClan == null && MBRandom.RandomInt(1, 100) <= (int)(Settings.Current.MinorClanFrequencyOnInterval * 100)))
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
                leader = CreateHeroAction.ApplyInternal(character, settlement, clan, MBRandom.RandomInt(Settings.Current.MinimumLeaderHeroAge, Settings.Current.MaximumLeaderHeroAge));
            }
            clan.SetLeader(leader);
            int minimumTier = Settings.Current.MinimumClanTier;
            int maximumTier = Settings.Current.MaximumClanTier;
            int tier = minimumTier <= maximumTier ? MBRandom.RandomInt(minimumTier, maximumTier) : MBRandom.RandomInt(maximumTier, minimumTier);
            int[] TierLowerRenown = new int[] { 0, 50, 150, 350, 900, 2350, 6150 };
            clan.AddRenown(TierLowerRenown[tier]);
            clan.Renown = Campaign.Current.Models.ClanTierModel.CalculateInitialRenown(clan);
            if (detail == CreateClanDetail.ByCreateAutonomously)
            {
                EnterSettlementAction.ApplyForCharacterOnly(leader, settlement);
            }
            else
            {
                if (leader.PartyBelongedTo == MobileParty.MainParty)
                {
                    MobileParty.MainParty.MemberRoster.AddToCounts(leader.CharacterObject, -1, false, 0, 0, true, -1);
                }
            }
            GiveGoldAction.ApplyBetweenCharacters(null, leader, Settings.Current.StartingGoldForLeader * 1000, false);
            int j = MBRandom.RandomInt(Settings.Current.MinimumHeroesSpawned, Settings.Current.MaximumHeroesSpawned);
            for (int i = 0; i < j; i++)
            {
                Hero hero = CreateHeroAction.ApplyInternal(character, settlement, clan, MBRandom.RandomInt(Settings.Current.MinimumHeroAge, Settings.Current.MaximumHeroAge));
                EnterSettlementAction.ApplyForCharacterOnly(hero, settlement);
                GiveGoldAction.ApplyBetweenCharacters(null, leader, Settings.Current.ExtraStartingGoldPerHero * 1000, true);
            }
            //Decides whether troops should be added based on the setting, then creates a single party for the clan and populates it with culture troops until the clan strength is met or close to being met without going over.
            int selectedIndex = Settings.Current.FillClanParties.SelectedIndex;
            if (selectedIndex != 0)
            {
                for (int parties = 0; parties < clan.CommanderLimit; parties++)
                {
                    Hero strongest = null!;
                    int strongestSum = 0;
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
                    Vec2 position = detail == CreateClanDetail.ByCreateAutonomously ? settlement.GatePosition : MobileParty.MainParty.Position2D;
                    float radius = detail == CreateClanDetail.ByCreateAutonomously ? 150f : 3f;
                    MobileParty party = LordPartyComponent.CreateLordParty("CC_" + MobileParty.All.Count, strongest, position, radius, settlement, strongest);
                    CharacterObject basicTroop = selectedIndex == 1 || (selectedIndex == 2 && MBRandom.RandomInt(1) == 0) ? culture.BasicTroop : culture.EliteBasicTroop;
                    party.AddElementToMemberRoster(basicTroop, party.LimitedPartySize - party.MemberRoster.Count);
                    if (party.MemberRoster.Count < 1)
                    {
                        party.RemoveParty();
                    }
                }
            }
            //Don't preserve kingdom for mercenaries
            if (detail == CreateClanDetail.ByCreateAutonomously)
            {
                if (Settings.Current.PreserveKingdoms && !mercenary)
                {
                    // Preserve old clan kingdom. If it's null, preserve the kingdom with the least active clans. If the kingdom is owned by the player, give them the option. If the player refuses, move to next kingdom.
                    IEnumerable<Kingdom> kingdoms = from k in Kingdom.All where !k.IsEliminated orderby (from j in k.Clans where !j.IsEliminated select j).Count() ascending select k;
                    // TODO fix, poor implementation
                    Kingdom kingdom = oldClan != null && oldClan.Kingdom != null ? oldClan.Kingdom : kingdoms.ElementAtOrDefault(0);
                    if (kingdom != null)
                    {
                        if (Settings.Current.PreservePlayerKingdomInquiry && kingdom.Leader == Hero.MainHero)
                        {
                            InformationManager.ShowInquiry(
                                new InquiryData(
                                    new TextObject("{=XnI75682}A new clan emerged.").ToString(),
                                    new TextObject("{=sFGXUicT}A rising new clan, {CLAN_NAME}, has been spotted nearby and would like to serve our kingdom. Do you accept them?").SetTextVariable("CLAN_NAME", clan.Name).ToString(),
                                    true, true,
                                    new TextObject("{=ZdqbU2gw}Accept").ToString(),
                                    new TextObject("{=D9WJXQ9Z}Reject").ToString(),
                                    () =>
                                    {
                                        ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom);
                                    },
                                    () =>
                                    {
                                        if (kingdoms.Count() > 1)
                                        {
                                            ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdoms.ElementAtOrDefault(1));
                                        }
                                    }
                                ), true);
                        }
                        else
                        {
                            ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom);
                        }
                    }
                }
                TextObject message = new TextObject("{=ShdkpllV}A rising new clan, {CLAN_NAME}, has been spotted near {SETTLEMENT_NAME}.").SetTextVariable("CLAN_NAME", clan.Name).SetTextVariable("SETTLEMENT_NAME", settlement.Name);
                InformationManager.DisplayMessage(new InformationMessage(message.ToString(), Color.White));
            }
            else
            {
                ChangeKingdomAction.ApplyByJoinToKingdom(clan, Clan.PlayerClan.Kingdom);
                GainKingdomInfluenceAction.ApplyForDefault(Hero.MainHero, -1000f);
                ChangeRelationAction.ApplyPlayerRelation(leader, 50, true, true);
                CampaignEventDispatcher.Instance.OnCompanionClanCreated(clan);
            }
        }

        public static void ApplyByCreateAutonomously(TextObject name, Clan oldClan, CultureObject culture, Settlement settlement)
        {
            ApplyInternal(name, oldClan, culture, null!, settlement, CreateClanDetail.ByCreateAutonomously);
        }

        public static void ApplyByTurningToLord(TextObject name, Hero leader, Settlement settlement)
        {
            ApplyInternal(name, null!, leader.Culture, leader, settlement, CreateClanDetail.ByTurningToLord);
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
