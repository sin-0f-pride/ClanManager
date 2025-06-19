using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace ClanManager.Actions
{
    public static class CreateHeroAction
    {
        public static Hero ApplyInternal(CharacterObject template, Settlement bornSettlement, Clan clan, int age)
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
            hero.UpdateHomeSettlement();
            hero.ChangeState(Hero.CharacterStates.Active);
            return hero;
        }
    }
}
