using System.IO;
using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

using Bannerlord.UIExtenderEx;
using HarmonyLib;

using ClanManager.Behaviors;

namespace ClanManager
{
    public class SubModule : MBSubModuleBase
    {

        protected override void OnSubModuleLoad()
        {
            var extender = new UIExtender("ClanManager");
            extender.Register(typeof(SubModule).Assembly);
            extender.Enable();

            base.OnSubModuleLoad();
            new Harmony("mod.bannerlord.clanmanager").PatchAll();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=e3pC77k9}Clan Creator loaded").ToString()));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            Campaign campaign = game.GameType as Campaign;
            if (campaign != null)
            {
                CampaignGameStarter starter = (CampaignGameStarter)gameStarterObject;
                starter.AddBehavior(new ClanCreationBehavior());
                starter.AddBehavior(new CMLordConversationsCampaignBehavior());
            }
        }

        public static void Log(string message)
        {
            string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord", "Logs");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            string path = Path.Combine(text, "ClanCreation.txt");
            using (StreamWriter streamWriter = new StreamWriter(path, true))
            {
                streamWriter.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message);
            }
        }
    }
}