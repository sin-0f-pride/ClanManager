using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace ClanManager.Actions
{
    public static class CreateHeroAction
    {
        public static Hero ApplyInternal(CharacterObject template, Settlement bornSettlement, Clan clan, CultureObject culture, int age)
        {
            bool IsFemale = MBRandom.RandomFloat <= Settings.Current.FemaleChance;
            template.IsFemale = IsFemale;
            ValueTuple<CampaignTime, CampaignTime> birthAndDeathDay = Campaign.Current.Models.HeroCreationModel.GetBirthAndDeathDay(template, true, age);
            CampaignTime birth = birthAndDeathDay.Item1;
            CampaignTime death = birthAndDeathDay.Item2;
            CharacterObject character = CharacterObject.CreateFrom(template);
            Hero hero = new Hero(character.StringId, character, birth, death);
            character.GetType().GetProperty("HeroObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(character, hero);
            hero.BornSettlement = bornSettlement;
            hero.Clan = clan;
            hero.IsFemale = IsFemale;
            hero.PreferredUpgradeFormation = Campaign.Current.Models.HeroCreationModel.GetPreferredUpgradeFormation(hero);
            hero.Culture = Campaign.Current.Models.HeroCreationModel.GetCulture(hero, hero.BornSettlement, hero.Clan);
            hero.StaticBodyProperties = Campaign.Current.Models.HeroCreationModel.GetStaticBodyProperties(hero, false, 0.35f);
            DynamicBodyProperties dynamicBodyProperties = CharacterHelper.GetDynamicBodyPropertiesBetweenMinMaxRange(hero.CharacterObject);
            hero.Weight = dynamicBodyProperties.Weight;
            hero.Build = dynamicBodyProperties.Build;
            NameGenerator.Current.GenerateHeroNameAndHeroFullName(hero, out TextObject firstName, out TextObject fullName, false);
            hero.SetName(fullName, firstName);
            hero.SetNewOccupation(Occupation.Lord);
            hero.UpdateHomeSettlement();
            int minTraitLevel = Settings.Current.MinimumPersonalityTraitLevel;
            int maxTraitLevel = Settings.Current.MaximumPersonalityTraitLevel;
            hero.SetTraitLevel(DefaultTraits.Mercy, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
            hero.SetTraitLevel(DefaultTraits.Calculating, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
            hero.SetTraitLevel(DefaultTraits.Valor, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
            hero.SetTraitLevel(DefaultTraits.Honor, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
            hero.SetTraitLevel(DefaultTraits.Generosity, MBRandom.RandomInt(minTraitLevel, maxTraitLevel));
            foreach (SkillObject skill in Skills.All)
            {
                hero.SetSkillValue(skill, MBRandom.RandomInt(Settings.Current.MinimumSkillLevel, Settings.Current.MaximumSkillLevel));
            }
            if (character.Age >= Campaign.Current.Models.AgeModel.HeroComesOfAge)
            {
                hero.HeroDeveloper.InitializeHeroDeveloper();
            }
            Equipment civilianEquipment = Campaign.Current.Models.HeroCreationModel.GetCivilianEquipment(hero);
            EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, civilianEquipment);
            Equipment battleEquipment = Campaign.Current.Models.HeroCreationModel.GetBattleEquipment(hero);
            EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, battleEquipment);
            hero.HeroDeveloper.InitializeHeroDeveloper();
            hero.ChangeState(Hero.CharacterStates.Active);
            CampaignEventDispatcher.Instance.OnHeroCreated(hero, false);
            return hero;
        }
    }
}
