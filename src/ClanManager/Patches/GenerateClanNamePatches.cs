using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

using ClanManager.ClanCreator;
using HarmonyLib;

namespace ClanManager.Patches
{
    [HarmonyPatch(typeof(NameGenerator), "GenerateClanName")]
    public static class GenerateClanNamePatch
    {
        public static void Postfix(CultureObject culture, ref TextObject __result)
        {
            if (Settings.Current!.CustomClanNames.SelectedIndex == 0)
            {
                return;
            }
            if (ClanCreationBehavior.Names.TryGetValue(culture, out List<TextObject> value) && value.Contains(__result))
            {
                value.Remove(__result);
                ClanCreationBehavior.Names.Remove(culture);
                if (!value.IsEmpty())
                {
                    ClanCreationBehavior.Names.Add(culture, value);
                }
            }
        }
    }

    [HarmonyPatch(typeof(NameGenerator), "GetClanNameListForCulture")]
    public static class GetClanNameListForCulturePatch
    {
        public static void Postfix(CultureObject clanCulture, ref TextObject[] __result)
        {
            if (Settings.Current!.CustomClanNames.SelectedIndex == 0)
            {
                return;
            }
            if (ClanCreationBehavior.Names.TryGetValue(clanCulture, out List<TextObject> namesList) && !namesList.IsEmpty())
            {
                if (Settings.Current!.CustomClanNames.SelectedIndex == 1 && !__result.IsEmpty())
                {
                    __result = __result.AddRangeToArray(namesList.ToArray());
                }
                else
                {
                    __result = namesList.ToArray();
                }
            }
        }
    }
}