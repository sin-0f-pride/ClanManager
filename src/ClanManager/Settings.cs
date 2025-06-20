﻿using TaleWorlds.CampaignSystem;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace ClanManager
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public Settings()
        {
            //Rmpty
        }

        private const string HeadingOther = "{=GlH54ipi}Other";
        private const string HeadingSpawnOnDestruction = "{=2L5dh4eY}Spawn On Destruction";
        private const string HeadingSpawnOnInterval = "{=vp2bEyHd}Spawn On Interval";
        private const string HeadingClanProperties = "{=SmgX9Ojn}Clan Properties";
        private const string HeadingHeroProperties = "{=FeYHk9jz}Hero Properties";

        //Spawn On Destruction
        private bool _enableSpawnOnDestruction = true;
        private bool _minorClansOnDestruction = false;
        private bool _preserveClanTier = false;
        private int _numberOfClansOnDestruction = 1;

        //Spawn On Interval
        private bool _enableSpawnOnInterval = false;
        private float _minorClanFrequencyOnInterval = 0f;
        private int _spawnInterval = 1;
        private int _numberOfClansOnInterval = 1;

        //Clan Properties
        private bool _preserveCultures = true;
        private bool _preserveKingdoms = false;
        private bool _preservePlayerKingdomInquiry = false;
        private int _minimumClanTier = 0;
        private int _maximumClanTier = 4;

        //Hero Properties
        private int _minimumHeroesSpawned = 5;
        private int _maximumHeroesSpawned = 10;
        private int _minimumLeaderHeroAge = 30;
        private int _maximumLeaderHeroAge = 50;
        private int _minimumHeroAge = 30;
        private int _maximumHeroAge = 50;
        private int _minimumPersonalityTraitLevel = -2;
        private int _maximumPersonalityTraitLevel = 2;
        //private int _minimumCharacterLevel = 1;
        //private int _maximumCharacterLevel = 10;
        //private int _minimumSkillLevel = 0;
        //private int _maximumSkillLevel = 330;
        //private bool _earnsLevelUpPoints = true;
        private float _femaleChance = 0.51f;
        private int _startingGoldForLeader = 25;
        private int _extraStartingGoldPerHero = 5;

        //Other Features
        private bool _enableEncyclopediaTweaks = false;

        public override string Id => "ClanManager_v1";

        public override string DisplayName => "Clan Manager";

        public override string FolderName => "ClanManager";

        public override string FormatType => "json2";



        [SettingPropertyBool("{=6QUoCsDJ}Encyclopedia Tweaks (See Hint)", Order = 0, RequireRestart = false, HintText = "{=hMxjqjSD}Enables a clan name change button and removes the annoying shadow at the bottom of the clan page in the encyclopedia. Recommended to toggle on and off as needed so you can choose the names of spawned clans yourself, particularly those in your kingdom. Default value is Disabled.")]
        [SettingPropertyGroup(HeadingOther, GroupOrder = 0)]
        public bool EnableEncyclopediaTweaks
        {
            get => _enableEncyclopediaTweaks;
            set
            {
                if (_enableEncyclopediaTweaks != value)
                {
                    _enableEncyclopediaTweaks = value;
                    OnPropertyChanged(nameof(EnableEncyclopediaTweaks));
                }
            }
        }

        [SettingPropertyBool(HeadingSpawnOnDestruction, IsToggle = true, Order = 0, RequireRestart = false, HintText = "{=JftwpCVj}Enables spawning of clans after the destruction of another clan. Default is Enabled.")]
        [SettingPropertyGroup(HeadingSpawnOnDestruction, GroupOrder = 1)]
        public bool EnableSpawnOnDestruction
        {
            get => _enableSpawnOnDestruction;
            set
            {
                if (_enableSpawnOnDestruction != value)
                {
                    _enableSpawnOnDestruction = value;
                    OnPropertyChanged(nameof(EnableSpawnOnDestruction));
                }
            }
        }

        [SettingPropertyBool("{=iPZcv44F}Spawn Minor Clans", Order = 1, RequireRestart = false, HintText = "{=W3134oZw}Enables spawning for Minor clans after the destruction of another Minor clan. Default is Disabled.")]
        [SettingPropertyGroup(HeadingSpawnOnDestruction, GroupOrder = 1)]
        public bool MinorClansOnDestruction
        {
            get => _minorClansOnDestruction;
            set
            {
                if (_minorClansOnDestruction != value)
                {
                    _minorClansOnDestruction = value;
                    OnPropertyChanged(nameof(MinorClansOnDestruction));
                }
            }
        }

        [SettingPropertyBool("{=2HdnOwnd}Preserve Clan Tier", Order = 2, RequireRestart = false, HintText = "{=h8BphLAm}Sets the tier of the new clan(s) to the tier of the destroyed clan. Default is Disabled.")]
        [SettingPropertyGroup(HeadingSpawnOnDestruction, GroupOrder = 1)]
        public bool PreserveClanTier
        {
            get => _preserveClanTier;
            set
            {
                if (_preserveClanTier != value)
                {
                    _preserveClanTier = value;
                    OnPropertyChanged(nameof(PreserveClanTier));
                }
            }
        }

        [SettingPropertyInteger("{=f0pDnnFx}Number Of Clans", 1, 10, Order = 3, RequireRestart = false, HintText = "{=cUS8xOZW}The number of clans to spawn for each destroyed clan. Default is 1.")]
        [SettingPropertyGroup(HeadingSpawnOnDestruction, GroupOrder = 1)]
        public int NumberOfClansOnDestruction
        {
            get => _numberOfClansOnDestruction;
            set
            {
                if (_numberOfClansOnDestruction != value)
                {
                    _numberOfClansOnDestruction = value;
                    OnPropertyChanged(nameof(NumberOfClansOnDestruction));
                }
            }
        }

        [SettingPropertyBool(HeadingSpawnOnInterval, IsToggle = true, Order = 10, RequireRestart = false, HintText = "{=cb1w2paM}Enables spawning of clans on an interval. Default is Disabled.")]
        [SettingPropertyGroup(HeadingSpawnOnInterval, GroupOrder = 10)]
        public bool EnableSpawnOnInterval
        {
            get => _enableSpawnOnInterval;
            set
            {
                if (_enableSpawnOnInterval != value)
                {
                    _enableSpawnOnInterval = value;
                    OnPropertyChanged(nameof(EnableSpawnOnInterval));
                }
            }
        }

        [SettingPropertyFloatingInteger("{=k3Pl4gSP}Minor Clan Frequency", 0f, 1f, "#0%", Order = 11, RequireRestart = false, HintText = "{=FZyYqieN}The frequency of clans to be spawned as a minor clan. Default is 0%.")]
        [SettingPropertyGroup(HeadingSpawnOnInterval, GroupOrder = 10)]
        public float MinorClanFrequencyOnInterval
        {
            get => _minorClanFrequencyOnInterval;
            set
            {
                if (_minorClanFrequencyOnInterval != value)
                {
                    _minorClanFrequencyOnInterval = value;
                    OnPropertyChanged(nameof(MinorClanFrequencyOnInterval));
                }
            }
        }

        [SettingPropertyDropdown("{=bm4gm2RA}Spawn Interval Type", Order = 12, RequireRestart = false, HintText = "{=h4O5Hyoj}The time interval type for spawning clans. Default is Daily.")]
        [SettingPropertyGroup(HeadingSpawnOnInterval, GroupOrder = 10)]
        public Dropdown<string> SpawnIntervalType { get; } = new(new string[]
        {
            "{=RBjnO8dO}Hourly",
            "{=pDlbMeee}Daily",
            "{=IXZdCfGr}Weekly"
        }, 1);

        [SettingPropertyInteger("{=W2opiqqi}Spawn Interval Value", 1, 20, Order = 13, RequireRestart = false, HintText = "{=BO64UWTy}The time interval value for spawning clans. Default is 1.")]
        [SettingPropertyGroup(HeadingSpawnOnInterval, GroupOrder = 10)]
        public int SpawnInterval
        {
            get => _spawnInterval;
            set
            {
                if (_spawnInterval != value)
                {
                    _spawnInterval = value;
                    OnPropertyChanged(nameof(SpawnInterval));
                }
            }
        }

        [SettingPropertyInteger("{=MmLKrNfA}Number Of Clans", 1, 20, Order = 14, RequireRestart = false, HintText = "{=J1lPZjss}The number of clans to spawn each interval. Default is 1.")]
        [SettingPropertyGroup(HeadingSpawnOnInterval, GroupOrder = 10)]
        public int NumberOfClansOnInterval
        {
            get => _numberOfClansOnInterval;
            set
            {
                if (_numberOfClansOnInterval != value)
                {
                    _numberOfClansOnInterval = value;
                    OnPropertyChanged(nameof(NumberOfClansOnInterval));
                }
            }
        }

        [SettingPropertyBool("{=ejm3RiiU}Preserve Cultures", HintText = "{=rm5EC51X}On destruction: Preserves the destroyed clan culture. On interval: Preserves the culture with the least clans. May pick a different culture on each iteration if 'Number Of Clans' is more than 1. Default is Enabled.", Order = 20, RequireRestart = false)]
        [SettingPropertyGroup(HeadingClanProperties, GroupOrder = 20)]
        public bool PreserveCultures
        {
            get => _preserveCultures;
            set
            {
                if (_preserveCultures != value)
                {
                    _preserveCultures = value;
                    OnPropertyChanged(nameof(PreserveCultures));
                }
            }
        }

        [SettingPropertyBool("{=7byinotg}Preserve Kingdoms", HintText = "{=F1qY6mJh}On destruction: Preserves the destroyed clan kingdom, or the kingdom with the least clans if one isn't found for the clan. Won't preserve kingdom of a mercenary clan. On interval: Preserves the kingdom with the least clans. Default is Disabled.", Order = 21, RequireRestart = false)]
        [SettingPropertyGroup(HeadingClanProperties, GroupOrder = 20)]
        public bool PreserveKingdoms
        {
            get => _preserveKingdoms;
            set
            {
                if (_preserveKingdoms != value)
                {
                    _preserveKingdoms = value;
                    OnPropertyChanged(nameof(PreserveKingdoms));
                }
            }
        }

        [SettingPropertyBool("{=QSU99UWV}Preserve Player Kingdom Inquiry", HintText = "{=wQqa4vfR}Displays an inquiry whenever the player kingdom is chosen to be preserved to allow the choice of refusing a newly created clan. Requires 'Preserve Kingdoms' to be enabled or it does nothing. Default is Disabled.", Order = 22, RequireRestart = false)]
        [SettingPropertyGroup(HeadingClanProperties, GroupOrder = 20)]
        public bool PreservePlayerKingdomInquiry
        {
            get => _preservePlayerKingdomInquiry;
            set
            {
                if (_preservePlayerKingdomInquiry != value)
                {
                    _preservePlayerKingdomInquiry = value;
                    OnPropertyChanged(nameof(PreservePlayerKingdomInquiry));
                }
            }
        }

        [SettingPropertyInteger("{=rXnBaoFZ}Minimum Clan Tier", 0, 6, HintText = "{=0kjLx3pT}The minimum tier a spawned clan can have. Does nothing on destruction if 'Preserve Clan Tier' is enabled. Default is 0.", Order = 23, RequireRestart = false)]
        [SettingPropertyGroup(HeadingClanProperties, GroupOrder = 20)]
        public int MinimumClanTier
        {
            get => _minimumClanTier;
            set
            {
                if (_minimumClanTier != value)
                {
                    _minimumClanTier = value;
                    OnPropertyChanged(nameof(MinimumClanTier));
                }
            }
        }

        [SettingPropertyInteger("{=iX5NyGT7}Maximum Clan Tier", 0, 6, HintText = "{=DPNSA51p}The maximum tier a spawned clan can have. Does nothing on destruction if 'Preserve Clan Tier' is enabled. Default is 4.", Order = 24, RequireRestart = false)]
        [SettingPropertyGroup(HeadingClanProperties, GroupOrder = 20)]
        public int MaximumClanTier
        {
            get => _maximumClanTier;
            set
            {
                if (_maximumClanTier != value)
                {
                    _maximumClanTier = value;
                    OnPropertyChanged(nameof(MaximumClanTier));
                }
            }
        }

        [SettingPropertyDropdown("{=dZC9yjvn}Custom Name Policy", HintText = "{=yeYaXEZN}Allows usage of the custom clan naming xml in located in the ModuleData folder. Must reload your save to activate mid-game. Can be freely deactivated without harming your save game. Recommended value is Mixed. Default is Native Only.", Order = 25, RequireRestart = false)]
        [SettingPropertyGroup(HeadingClanProperties, GroupOrder = 20)]
        public Dropdown<string> CustomClanNames { get; } = new(new string[]
        {
            "{=sgBd15AI}Native Only",
            "{=Hno3UosH}Mixed",
            "{=ih6m7Rw0}Custom Only"
        }, 0);

        [SettingPropertyDropdown("{=9lLuoVXp}Duplicate Name Policy", HintText = "{=3Q6WA17h}Allows spawning of a clan if the chosen name is already taken by an existing clan. By default, the mod only chooses an existing clan name if an unused name does not exist. Clan creation will be canceled if no usable name is found. Default is None.", Order = 26, RequireRestart = false)]
        [SettingPropertyGroup(HeadingClanProperties, GroupOrder = 20)]
        public Dropdown<string> DuplicateClanNames { get; } = new(new string[]
        {
            "{=PXA2f2VL}None",
            "{=nDeLJaYr}Eliminated Only",
            "{=2tqvGdAF}All"
        }, 0);

        [SettingPropertyDropdown("{=7NcciANO}Fill Clan Parties", HintText = "{=yrpmRjsG}The troop types to fill each clan party with. Note: None starts the clan from scratch, and could make their progression extremely difficult. Default is Basic troops only.", Order = 27, RequireRestart = false)]
        [SettingPropertyGroup(HeadingClanProperties, GroupOrder = 20)]
        public Dropdown<string> FillClanParties { get; } = new(new string[]
        {
            "{=PXA2f2VL}None",
            "{=V4uoxPq8}Basic troops only",
            "{=ZZMzk3mF}Basic & Noble troops",
            "{=a5Uryd6L}Noble troops only"
        }, 1);

        [SettingPropertyDropdown("{=ZCEE5IGN}Declare Wars On Spawn", HintText = "{=whZBe8YI}Declares war on specified kingdom types when the clan is spawned. Clan(s) will have less of a chance of survival, but the option is there for chaos seekers. Default is None.", Order = 28, RequireRestart = false)]
        [SettingPropertyGroup(HeadingClanProperties, GroupOrder = 20)]
        public Dropdown<string> DeclareWarsOnSpawn { get; } = new(new string[]
        {
            "{=PXA2f2VL}None",
            "{=2tqvGdAF}All",
            "{=08BvmgrA}AI Only",
            "{=7D2onF3N}Player Only",
            "{=MeuOco0u}Different Culture Only",
            "{=I1BRJ0ha}Same Culture Only"
        }, 0);

        [SettingPropertyInteger("{=uzpk2jDS}Minimum Heroes Spawned", 1, 20, HintText = "{=DtOFcKes}The minimum number of heroes for spawned clans. Default is 5.", Order = 30, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int MinimumHeroesSpawned
        {
            get => _minimumHeroesSpawned;
            set
            {
                if (_minimumHeroesSpawned != value)
                {
                    _minimumHeroesSpawned = value;
                    OnPropertyChanged(nameof(MinimumHeroesSpawned));
                }
            }
        }

        [SettingPropertyInteger("{=cqZITSPf}Maximum Heroes Spawned", 1, 20, HintText = "{=6BDzLuHZ}The maximum number of heroes for spawned clans. Default is 10.", Order = 31, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int MaximumHeroesSpawned
        {
            get => _maximumHeroesSpawned;
            set
            {
                if (_maximumHeroesSpawned != value)
                {
                    _maximumHeroesSpawned = value;
                    OnPropertyChanged(nameof(MaximumHeroesSpawned));
                }
            }
        }

        [SettingPropertyInteger("{=h9GqNhoh}Minimum Leader Hero Age", 18, 100, HintText = "{=t1CTL7Pj}The minimum age of the leader hero for spawned clans. Default is 30.", Order = 32, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int MinimumLeaderHeroAge
        {
            get => _minimumLeaderHeroAge;
            set
            {
                if (_minimumLeaderHeroAge != value)
                {
                    _minimumLeaderHeroAge = value;
                    OnPropertyChanged(nameof(MinimumLeaderHeroAge));
                }
            }
        }

        [SettingPropertyInteger("{=9rcOloAc}Maximum Leader Hero Age", 18, 100, HintText = "{=YytXehJX}The maximum age of the leader hero for spawned clans. Default is 50.", Order = 33, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int MaximumLeaderHeroAge
        {
            get => _maximumLeaderHeroAge;
            set
            {
                if (_maximumLeaderHeroAge != value)
                {
                    _maximumLeaderHeroAge = value;
                    OnPropertyChanged(nameof(MaximumLeaderHeroAge));
                }
            }
        }

        [SettingPropertyInteger("{=aAT63aqC}Minimum Hero Age", 0, 100, HintText = "{=kHOtTfxp}The minimum age of heroes for spawned clans. Default is 30.", Order = 34, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int MinimumHeroAge
        {
            get => _minimumHeroAge;
            set
            {
                if (_minimumHeroAge != value)
                {
                    _minimumHeroAge = value;
                    OnPropertyChanged(nameof(MinimumHeroAge));
                }
            }
        }

        [SettingPropertyInteger("{=7wYp9pA6}Maximum Hero Age", 0, 100, HintText = "{=R6O8s1RN}The minimum age of heroes for spawned clans. Default is 50.", Order = 35, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int MaximumHeroAge
        {
            get => _maximumHeroAge;
            set
            {
                if (_maximumHeroAge != value)
                {
                    _maximumHeroAge = value;
                    OnPropertyChanged(nameof(MaximumHeroAge));
                }
            }
        }

        [SettingPropertyInteger("{=aIna4f85}Minimum Personality Trait Level", -2, 2, HintText = "{=CgnkpKzm}Minimum level of default personality traits. Default is -2.", Order = 36, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int MinimumPersonalityTraitLevel
        {
            get => _minimumPersonalityTraitLevel;
            set
            {
                if (_minimumPersonalityTraitLevel != value)
                {
                    _minimumPersonalityTraitLevel = value;
                    OnPropertyChanged(nameof(MinimumPersonalityTraitLevel));
                }
            }
        }

        [SettingPropertyInteger("{=v73XjgH5}Maximum Personality Trait Level", -2, 2, HintText = "{=zauNoekp}Maximum level of default personality traits. Default is 2.", Order = 37, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int MaximumPersonalityTraitLevel
        {
            get => _maximumPersonalityTraitLevel;
            set
            {
                if (_maximumPersonalityTraitLevel != value)
                {
                    _maximumPersonalityTraitLevel = value;
                    OnPropertyChanged(nameof(MaximumPersonalityTraitLevel));
                }
            }
        }
        /*
        [SettingPropertyInteger("{=nfMFmDGwKf}Minimum Character Level", 1, 62, HintText = "Default is 5. -1 to always use destroyed clan tier.", Order = 38, RequireRestart = false)]
        [SettingPropertyGroup("Hero Properties", GroupOrder = 30)]
        public int MinimumCharacterLevel
        {
            get => _minimumCharacterLevel;
            set
            {
                if (_minimumCharacterLevel != value)
                {
                    _minimumCharacterLevel = value;
                    OnPropertyChanged(nameof(MinimumCharacterLevel));
                }
            }
        }

        [SettingPropertyInteger("{=nfMFmDGwKf}Maximum Character Level", 1, 62, HintText = "Default is 5. -1 to always use destroyed clan tier.", Order = 39, RequireRestart = false)]
        [SettingPropertyGroup("Hero Properties", GroupOrder = 30)]
        public int MaximumCharacterLevel
        {
            get => _maximumCharacterLevel;
            set
            {
                if (_maximumCharacterLevel != value)
                {
                    _maximumCharacterLevel = value;
                    OnPropertyChanged(nameof(MaximumCharacterLevel));
                }
            }
        }

        [SettingPropertyInteger("{=nfMFmDGwKf}Minimum Skill Level", 0, 330, HintText = "Default is 5. -1 to always use destroyed clan tier.", Order = 40, RequireRestart = false)]
        [SettingPropertyGroup("Hero Properties", GroupOrder = 30)]
        public int MinimumSkillLevel
        {
            get => _minimumSkillLevel;
            set
            {
                if (_minimumSkillLevel != value)
                {
                    _minimumSkillLevel = value;
                    OnPropertyChanged(nameof(MinimumSkillLevel));
                }
            }
        }

        [SettingPropertyInteger("{=nfMFmDGwKf}Maximum Skill Level", 0, 330, HintText = "Default is 5. -1 to always use destroyed clan tier.", Order = 41, RequireRestart = false)]
        [SettingPropertyGroup("Hero Properties", GroupOrder = 30)]
        public int MaximumSkillLevel
        {
            get => _maximumSkillLevel;
            set
            {
                if (_maximumSkillLevel != value)
                {
                    _maximumSkillLevel = value;
                    OnPropertyChanged(nameof(MaximumSkillLevel));
                }
            }
        }

        [SettingPropertyBool("{=nfMFmDGwKf}Earns Level Up Points", Order = 42, RequireRestart = false, HintText = "{=zTvbZ8Br}Enables a clan name change button in the encyclopedia. Default value is disabled.")]
        [SettingPropertyGroup("Other Features", GroupOrder = 30)]
        public bool EarnsLevelUpPoints
        {
            get => _earnsLevelUpPoints;
            set
            {
                if (_earnsLevelUpPoints != value)
                {
                    _earnsLevelUpPoints = value;
                    OnPropertyChanged(nameof(EarnsLevelUpPoints));
                }
            }
        }*/

        [SettingPropertyFloatingInteger("{=UVjVMgsd}Female Chance", 0, 1, "#0%", Order = 43, RequireRestart = false, HintText = "{=HK0uGuNJ}Percentage chance of heroes being female. Affects the leader hero. Default is 51% female.")]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public float FemaleChance
        {
            get => _femaleChance;
            set
            {
                if (_femaleChance != value)
                {
                    _femaleChance = value;
                    OnPropertyChanged(nameof(FemaleChance));
                }
            }
        }

        [SettingPropertyInteger("{=NAMwMlk9}Starting Gold For Leader", 0, 100, "0k", HintText = "{=bGfdt4Cb}The amount of gold the party leader(s) will start with. Default is 25k.", Order = 44, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int StartingGoldForLeader
        {
            get => _startingGoldForLeader;
            set
            {
                if (_startingGoldForLeader != value)
                {
                    _startingGoldForLeader = value;
                    OnPropertyChanged(nameof(StartingGoldForLeader));
                }
            }
        }

        [SettingPropertyInteger("{=s3gOtAM1}Extra Starting Gold Per Hero", 0, 100, "0k", HintText = "{=lE0vY23m}The extra amount of gold the party leader(s) will start with. Default is 5k.", Order = 45, RequireRestart = false)]
        [SettingPropertyGroup(HeadingHeroProperties, GroupOrder = 30)]
        public int ExtraStartingGoldPerHero
        {
            get => _extraStartingGoldPerHero;
            set
            {
                if (_extraStartingGoldPerHero != value)
                {
                    _extraStartingGoldPerHero = value;
                    OnPropertyChanged(nameof(ExtraStartingGoldPerHero));
                }
            }
        }
        public static Settings Current
        {
            get => Settings.Instance!;
        }
    }
}
