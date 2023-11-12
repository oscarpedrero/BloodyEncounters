# BloodyEncounters

**BloodyEncounters** represents an enhanced iteration of the RandonmEncounters mod by [@adainrivers](https://github.com/adainrivers), functioning as a server-side mod that introduces a thrilling dynamic to gameplay. It randomly spawns NPCs near online players at unpredictable intervals, challenging players to defeat these NPCs within a set time limit to earn random item rewards.

![BloodyEncounters](https://github.com/oscarpedrero/BloodyEncounters/blob/master/Images/BloodyEncounters.png?raw=true)

<details>
<summary>Changelog</summary>

`0.0.1`
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
.encounter items add <NameOfNPC> <ItemName> <ItemPrefabID> <Stack>
```
- Adds items/rewards to the randomized pool that the player will receive from killing a particular NPC encounter.
  - **NameOfNPC**: The NPC name to which you want to add items.
  - **ItemName**: The name of the item/reward appearing in the chat once the player wins the encounter.
  - **ItemPrefabID**: The GUID for the item you want to add.
  - **Stack**: The quantity of items the player will gain upon winning the encounter (e.g., x25 Blood Potions).
  - Example: `.encounter items add "Rifle Man" "Blood Rose Potion" 429052660 25`
```ansi
.encounter items list (NPCName)
```
- Displays the list of items included within a particular NPC.
  - Example: `.encounter items list "Rifle Man"`
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
