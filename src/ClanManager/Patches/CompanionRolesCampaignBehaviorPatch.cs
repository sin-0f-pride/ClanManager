using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

using HarmonyLib;

namespace ClanManager.Patches
{
    [HarmonyPatch(typeof(CompanionRolesCampaignBehavior), "turn_companion_to_lord_on_condition")]
    public static class CompanionRolesCampaignBehaviorPatch
    {
        public static bool Prefix(ref bool __result)
        {
            Hero hero = Hero.OneToOneConversationHero;
            if (hero != null && !hero.IsChild)
            {
                Hero mainHero = Hero.MainHero;
                List<Hero> relatives = new();
                relatives.AddRange(mainHero.Children);
                relatives.AddRange(mainHero.Siblings);
                foreach (Hero relative in relatives)
                {
                    if (hero == relative)
                    {
                        __result = true;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}