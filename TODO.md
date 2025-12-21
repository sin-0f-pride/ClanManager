# To do

## Issues
- Investigate absent equipment on heroes spawned with the mod
- Investigate empty parties wandering the map
- Investigate reported incompatibilities with Realm of Thrones, Old Realms, etc overhauls.
- Investigate report that empire culture never spawns on clans.
- Implement compatibility patch for party limit mods ie v's limited, true limits, etc.
- Add ability to modify and randomize banners.
- Investigate possibility of tying into Diplomacy faction mechanics to allow family member clans to create their own factions.
- Add better checking of names on startup to prevent crashes for duplicate names.
- Add support for ignoring destroyed or empty kingdoms when preserving kingdoms.
- Finish implementing create clan command
- Improve banner generation on spawned clans.
- Add checking of map events for the user, holding mod functions until it ends to prevent issues such as being asked to join your kingdom.

## General
- Backward version compatibility
- Banner checking on clan creation for more "solid" and neat looking banners without the user needing to modify them manually.
- If loaded for the first time into an existing save game, Clan Manager should check the current campaign for any already destroyed clans and spawn according to the settings if necessary. This should come with a disabled-by-default MCM option.
- All commands should eventually be given replacement functionality in a UI ie Interface to spawn clans to specified kingdom, etc 
- 'Kingdom Override' button on encyclopedias that overrides preservation settings to force spawned clans into the selected kingdom.
- Ability to turn minor clans into noble ones.

## Commands
- Create clan

## Settings
- 'Minimum Character level' and 'Maximum Character Level' setting to allow the user to decide the overall experience level of a generated character, so they don't level up wildly beyond sensible skill levels.
- 'Maximum spawned clans' setting to limit interval clan spawns.
- 'Spawn Heroes On Interval' and 'Spawn Heroes On Death' settings that spawn new heroes, optionally into random or smallest clans.
- 'Fill Clans On New Game' and 'Fill Kingdoms On New Game' settings to enforce a minimum amount of heroes in a clan or clans in a kingdom when a new game is created.
- 'Enable Dialogue Additions' option that allows disabling custom dialogue option for sending clan members to create a party.