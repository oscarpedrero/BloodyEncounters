# BloodyEncounters

**BloodyEncounters** represents an enhanced iteration of the RandonmEncounters mod by [@adainrivers](https://github.com/adainrivers), functioning as a server-side mod that introduces a thrilling dynamic to gameplay. It randomly spawns NPCs near online players at unpredictable intervals, challenging players to defeat these NPCs within a set time limit to earn random item rewards.

**NEW** V 1.5.0 now added a new World Boss (Dynamic World Boss) function that will allow you to create World Bosses for the entire server to participate in killing them.

![BloodyEncounters](https://github.com/oscarpedrero/BloodyEncounters/blob/master/Images/BloodyEncounters.png?raw=true)

<details>
<summary>Changelog</summary>

`1.5.0`
- Added World Boss

`1.0.0`
- Initial public release of the mod
</details>

## Mod Features
BloodyEncounters elevates gameplay by injecting an element of unpredictability and formidable challenges. As players venture outside their castles, there's a chance a random NPC will spawn nearby. You can customize the NPC level difference, ensuring balanced encounters for all players. This mod is highly configurable, offering an array of options to tailor the experience to your preferences.

Ensure the following mods are installed for seamless integration:

1. [BepInEx](https://github.com/BepInEx/BepInEx)
2. [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework)
3. [Bloodstone](https://github.com/decaprime/Bloodstone)

## Installation
1. Copy `BloodyEncounters.dll` to your `BepInEx/Plugins` directory.
2. Launch the server once to generate the config file; configurations will be located in the `BepInEx/Config` directory.

## Commands
It's crucial to note that for any command containing a name argument such as `<NameOfNPC>` or `<ItemName>`, if your name consists of more than one word, include it inside `""` to ensure proper functionality (e.g., "Rifle Man" or "Blood Rose Potion").

```ansi
.encounter npc create <NameOfNPC> <PrefabGUIDOfNPC> <LevelsAbovePlayer> <LifeTime>
```
- Create your desired NPCs to include in the encounter randomized pool.
  - **NameOfNPC**: The NPC name that will appear in the chat when the player triggers an encounter event.
  - **PrefabGUIDOfNPC**: The GUID of the NPC you prefer to use. 
  - **LevelsAbovePlayer**: Specify how many levels you want the NPC to be above the player level. For example, if a player is level 10 and the value is 10, the NPC will spawn at level 20.
  - **LifeTime**: The duration the player has to kill the NPC encounters in seconds.
  - Example: `.encounter npc create "Rifle Man" 1148936156 10 300`
```ansi
.encounter npc remove <NameOfNPC>
```
- Removes an NPC from the encounter randomized pool.
  -  **NameOfNPC**: The NPC name you want to remove.
  - Example: `.encounter npc remove "Rifle Man"`
```ansi
.encounter npc items add <NameOfNPC> <ItemName> <ItemPrefabID> <Stack>
```
- Adds items/rewards to the randomized pool that the player will receive from killing a particular NPC encounter.
  - **NameOfNPC**: The NPC name to which you want to add items.
  - **ItemName**: The name of the item/reward appearing in the chat once the player wins the encounter.
  - **ItemPrefabID**: The GUID for the item you want to add.
  - **Stack**: The quantity of items the player will gain upon winning the encounter (e.g., x25 Blood Potions).
  - Example: `.encounter items add "Rifle Man" "Blood Rose Potion" 429052660 25`
```ansi
.encounter npc items list (NPCName)
```
- Displays the list of items included within a particular NPC.
  - Example: `.encounter npc items list "Rifle Man"`
```ansi
.encounter enable
```
- Enables the BloodyEncounter mod.
  - Example: `.encounter enable`
```ansi
.encounter disable
```
- Disables the BloodyEncounter mod.
  - Example: `.encounter disable`
```ansi
.encounter reload
```
- Reloads the mod configuration in real-time.
  - Example: `.encounter reload`
```ansi
.encounter start
```
- Triggers an encounter for a random online player.
  - Example: `.encounter start`
```ansi
.encounter me
```
- Triggers an encounter for yourself.
  - Example: `.encounter me`
```ansi
.encounter player <PlayerName>
```
- Triggers an encounter for a specific player.
  - Example: `.encounter player Vex`

## **(NEW)** World Boss Commands:
prefix: `.encounter worldboss`.

```ansi
.encounter worldboss create <NameOfBOSS> <PrefabGUIDOfBOSS> <Level> <Multiplier> <LifeTimeSeconds>
```
- Create your desired Boss to include in the World Boss list.
  - **NameOfBOSS**: The Boss name that will appear in the chat when the World Boss spawn.
  - **PrefabGUIDOfNPC**: The GUID of the Boss you prefer to use. 
  - **Level**: Specify the level you want the Boss  to be.
  - **Multiplier**: Specify the HP multiplier based on how many players are online. For example, if the multiplier is 2 and there are 2 players online then the Boss HP will be x4 (2 for `multiplier value` x 2 `players online`) 
  - **LifeTimeSeconds**: The duration the player has to kill the Boss in seconds.
  - Example: `.encounter worldboss create "Alpha Wolf" -1905691330 90 1 1800`

```ansi
.encounter worldboss remove (bossName)
```
- Remove a Boss from the World Boss list.
  - **bossName**: The Boss name that you want to remove from the list.
  - Example: `.encounter worldboss remove "Alpha Wolf"`

```ansi
.encounter worldboss list
```
- List all the available Bosses to spawn from the World Boss list.
  - Example: `.encounter worldboss list`

```ansi
.encounter worldboss set location <NameOfWorldBoss>
```
- Specify the location at which a specific World Boss will spawn based on where you currently at in the game, meaning that where you stand is where the boss will spawn.
  - **NameOfWorldBoss**: The Boss name you want to specify the spawn location of.
  - Example: `.encounter worldboss location set "Alpha Wolf"`

```ansi
.encounter worldboss items add <NameOfWorldBoss> <ItemName> <ItemPrefabID> <Stack> <Chance>
```
- Adds items/rewards to the randomized pool that the player will receive from defeating a particular World Boss.
  - **NameOfWorldBoss**: The Boss name to which you want to add items.
  - **ItemName**: The name of the item/reward appearing in the chat once the player defeats the World Boss.
  - **ItemPrefabID**: The GUID for the item you want to add.
  - **Stack**: The quantity of items the player will gain upon winning the encounter (e.g., x25 Blood Potions).
  - **Chance**: The chance of that item to get upon defeating the World Boss (while 1 is equal to 100% and 0.9 is 90%... etc).
  - Example: `.encounter worldboss items add "Alpha Worlf" "Blood Rose Potion" 429052660 25 1`

```ansi
.encounter worldboss items remove <NameOfWorldBoss> <ItemName>
```
- Removes items/rewards from the randomized pool that the player will receive from defeating a particular World Boss.
  - **NameOfWorldBoss**: The Boss name to which you want to remove items.
  - **ItemName**: The name of the item/reward you want to remove.
  - Example: `.encounter worldboss items remove "Alpha Worlf" "Blood Rose"`

```ansi
.encounter worldboss items list <NameOfWorldBoss>
```
- Display all items/rewards from the randomized pool that the player will receive from defeating a particular World Boss.
  - **NameOfWorldBoss**: The Boss name to which you want to display items/rewards.
  - Example: `.encounter worldboss items list "Alpha Worlf"`

```ansi
.encounter worldboss start <NameOfWorldBoss>
```
- Manually spawn the World Boss in its specified location.
  - **NameOfWorldBoss**: The Boss name you want to start.
  - Example: `.encounter worldboss start "Alpha Worlf"`

```ansi
.encounter worldboss tp
```
- A player command that they will use in order to teleport to the World Boss location.
  - Example: `.encounter worldboss tp"`

# Resources

[Complete items list of prefabs/GUID](https://discord.com/channels/978094827830915092/1117273637024714862/1117273642817044571)

# Credits

This mod was originally developed by [@adainrivers](https://github.com/adainrivers/randomencounters) and was ported/updated to be compatible with the Gloomrot update.

[V Rising Mod Community](https://discord.gg/vrisingmods) is the premier community of mods for V Rising.

[@Deca](https://github.com/decaprime), thank you for the exceptional frameworks [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework) and [BloodStone](https://github.com/decaprime/Bloodstone), based on [WetStone](https://github.com/molenzwiebel/Wetstone) by [@Molenzwiebel](https://github.com/molenzwiebel).

[@Backxtar](https://github.com/Backxtar), owner & founder of [Bloody Mary](https://discord.gg/sE2hqbxUU4) server, and [@LecherousCthulhu](https://github.com/HasturDev) & [@Willis](https://github.com/emelonakos) for being amazing community modders and part of the *BloodyTeam*.

**Special thanks to the testers and supporters of the project:**

- @Vex, owner & founder of [Vexor RPG](https://discord.gg/JpVsKVvKNR) server, a tester and great supporter who provided his server as a test platform and took care of all the graphics and documentation.

![Bloody](https://github.com/oscarpedrero/BloodyMerchant/blob/master/Images/Bloody.png?raw=true)
