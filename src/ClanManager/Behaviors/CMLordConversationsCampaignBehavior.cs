using System;

using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Settlements;

using ClanManager.Actions;

namespace ClanManager
{
    internal class CMLordConversationsCampaignBehavior : CampaignBehaviorBase
    {

        private bool _playerConfirmedTheAction;

        public CMLordConversationsCampaignBehavior()
        {
            //Empty
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddDialogs(campaignGameStarter);
        }
        protected void AddDialogs(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddPlayerLine("CM_companion_talk_fire_new_clan", "hero_main_options", "CM_companion_new_clan_confirm", "{=!}I would like you to set off on your own and begin a new clan.", companion_create_clan_on_condition, null, 100, companion_create_clan_confirm_clickable_on_condition, null);
            campaignGameStarter.AddDialogLine("CM_companion_new_clan_confirm", "CM_companion_new_clan_confirm", "CM_companion_new_clan", "{=LOiZfCEy}My {?PLAYER.GENDER}lady{?}lord{\\?}, it would be an honor if you were to choose the name of my noble house.", null, CompanionCreateClanConsequence, 100, null);
            campaignGameStarter.AddDialogLine("CM_companion_new_clan_done", "CM_companion_new_clan_done", "close_window", "{=dpYhBgAC}Thank you my {?PLAYER.GENDER}lady{?}lord{\\?}. I will always remember this grand gesture.[ib:hip][if:convo_happy]", companion_create_clan_done_on_condition, companion_create_clan_done_on_consequence, 100, null);
        }

        public bool companion_create_clan_on_condition()
        {
            Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
            return oneToOneConversationHero != null && oneToOneConversationHero.Clan == Clan.PlayerClan;
        }
        private static bool companion_create_clan_done_on_condition()
        {
            return Campaign.Current.GetCampaignBehavior<CMLordConversationsCampaignBehavior>()._playerConfirmedTheAction;
        }
        private static bool companion_create_clan_confirm_clickable_on_condition(out TextObject explanation)
        {
            explanation = TextObject.GetEmpty();
            MBTextManager.SetTextVariable("CULTURE_SPECIFIC_TITLE", HeroHelper.GetTitleInIndefiniteCase(Hero.OneToOneConversationHero), false);
            bool hasRequiredInfluence = Hero.MainHero.Clan.Influence >= 1000f;
            MBTextManager.SetTextVariable("NEEDED_INFLUENCE_TO_GRANT_TITLE", 1000);
            MBTextManager.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">", false);
            if (hasRequiredInfluence)
            {
                explanation = new TextObject("{=CY3SwZFC}You will spend {NEEDED_INFLUENCE_TO_GRANT_TITLE}{INFLUENCE_ICON}.", null);
                return true;
            }
            explanation = new TextObject("{=wCRk12HK}You need {NEEDED_INFLUENCE_TO_GRANT_TITLE}{INFLUENCE_ICON}.", null);
            return false;
        }

        private static void companion_create_clan_done_on_consequence()
        {
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void CompanionCreateClanConsequence()
        {
            TextObject textObject = new TextObject("{=ntDH7J3H}This action costs {NEEDED_INFLUENCE_TO_GRANT_FIEF}{INFLUENCE_ICON}.");
            textObject.SetTextVariable("NEEDED_INFLUENCE_TO_GRANT_FIEF", 1000);
            textObject.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">");
            InformationManager.ShowInquiry(new InquiryData(new TextObject("{=awjomtnJ}Are you sure?", null).ToString(), textObject.ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), ConfirmCompanionCreateClanConsequence, RejectCompanionCreateClanConsequence, "", 0f, null, null, null), false, false);
        }

        private static void ConfirmCompanionCreateClanConsequence()
        {
            Campaign.Current.GetCampaignBehavior<CMLordConversationsCampaignBehavior>()._playerConfirmedTheAction = true;
            object obj = new TextObject("{=4eStbG4S}Select {COMPANION.NAME}{.o} clan name: ", null);
            StringHelpers.SetCharacterProperties("COMPANION", Hero.OneToOneConversationHero.CharacterObject, null, false);
            InformationManager.ShowTextInquiry(new TextInquiryData(obj.ToString(), string.Empty, true, false, GameTexts.FindText("str_done", null).ToString(), null, ClanNameSelectionIsDone, null, false, new Func<string, Tuple<bool, string>>(FactionHelper.IsClanNameApplicable), "", ""), false, false);
        }
        private static void RejectCompanionCreateClanConsequence()
        {
            Campaign.Current.GetCampaignBehavior<CMLordConversationsCampaignBehavior>()._playerConfirmedTheAction = false;
            Campaign.Current.ConversationManager.ContinueConversation();
        }

        private static void ClanNameSelectionIsDone(string clanName)
        {
            Hero hero = Hero.OneToOneConversationHero;
            RemoveCompanionAction.ApplyByByTurningToLord(Hero.MainHero.Clan, hero);
            hero.SetNewOccupation(Occupation.Lord);
            CultureObject culture = hero.Culture;
            Settlement settlement = Hero.MainHero.HomeSettlement;
            TextObject name = GameTexts.FindText("str_generic_clan_name", null);
            name.SetTextVariable("CLAN_NAME", new TextObject(clanName, null));
            CreateClanAction.ApplyByTurningToLord(name, hero, settlement);
            Campaign.Current.ConversationManager.ContinueConversation();
        }

        public override void SyncData(IDataStore dataStore)
        {
            //Empty
        }
    }
}