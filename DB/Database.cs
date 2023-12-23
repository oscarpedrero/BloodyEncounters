using BepInEx;
using Bloodstone.API;
using BloodyEncounters.DB.Models;
using BloodyEncounters.Exceptions;
using ProjectM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Unity.Entities;

namespace BloodyEncounters.DB
{
    internal static class Database
    {
        private static readonly Random Random = new();

        public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "BloodyEncounters");
        public static string NPCSListFile = Path.Combine(ConfigPath, "NPCS.json");
        public static string WORLDBOSSListFile = Path.Combine(ConfigPath, "WORLDBOSS.json");

        public static List<NpcEncounterModel> NPCS { get; set; } = new();
        public static List<BossEncounterModel> WORLDBOSS { get; set; } = new();

        public static void Initialize()
        {
            createDatabaseFiles();
            loadDatabase();
        }

        /*
         * 
         * 
         * DATABASE
         * 
         * 
         * 
         */

        public static bool createDatabaseFiles()
        {
            if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);
            if (!File.Exists(NPCSListFile)) File.WriteAllText(NPCSListFile, "[]");
            if (!File.Exists(WORLDBOSSListFile)) File.WriteAllText(WORLDBOSSListFile, "[]");
            Plugin.Logger.LogDebug($"Create Database: OK");
            return true;
        }

        public static bool saveDatabase()
        {
            try
            {
                var jsonOutPut = JsonSerializer.Serialize(NPCS, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(NPCSListFile, jsonOutPut);
                jsonOutPut = JsonSerializer.Serialize(WORLDBOSS, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(WORLDBOSSListFile, jsonOutPut);
                Plugin.Logger.LogDebug($"Save Database: OK");
                return true;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error SaveDatabase: {error.Message}");
                return false;
            }
        }

        public static bool loadDatabase()
        {
            try
            {
                string json = File.ReadAllText(NPCSListFile);
                NPCS = JsonSerializer.Deserialize<List<NpcEncounterModel>>(json);
                json = File.ReadAllText(WORLDBOSSListFile);
                WORLDBOSS = JsonSerializer.Deserialize<List<BossEncounterModel>>(json);
                Plugin.Logger.LogDebug($"Load Database: OK");
                return true;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error LoadDatabase: {error.Message}");
                return false;
            }
        }

        /*
         * 
         * 
         * NPC
         * 
         * 
         * 
         */

        public static bool GetNPC(string NPCName, out NpcEncounterModel npc)
        {
            npc = NPCS.Where(x => x.name == NPCName).FirstOrDefault();
            if (npc == null)
            {
                return false;
            }
            return true;
        }

        public static bool GetNPCFromEntity(Entity npcEntity , out NpcEncounterModel npc)
        {
            npc = NPCS.Where(x => x.npcEntity.Equals(npcEntity)).FirstOrDefault();
            if (npc == null)
            {
                return false;
            }
            return true;
        }

        public static bool AddNPC(string NPCName, int prefabGUIDOfNPC, int levelAbove, int lifetime)
        {
            if (GetNPC(NPCName, out NpcEncounterModel npc))
            {
                throw new NPCExistException();
            }

            npc = new NpcEncounterModel();
            npc.name = NPCName;
            npc.PrefabGUID = prefabGUIDOfNPC;
            npc.levelAbove = levelAbove;
            npc.Lifetime = lifetime;


            NPCS.Add(npc);
            saveDatabase();
            return true;

        }

        public static bool RemoveNPC(string NPCName)
        {
            if (GetNPC(NPCName, out NpcEncounterModel npc))
            {

                NPCS.Remove(npc);
                saveDatabase();
                return true;
            }

            throw new NPCDontExistException();
        }


        /*
         * 
         * 
         * WORLD BOSS
         * 
         * 
         * 
         */

        public static bool GetBoss(string NPCName, out BossEncounterModel boss)
        {
            boss = WORLDBOSS.Where(x => x.name == NPCName).FirstOrDefault();
            if (boss == null)
            {
                return false;
            }
            return true;
        }

        public static bool GetBOSSFromEntity(Entity npcEntity , out BossEncounterModel boss)
        {
            boss = WORLDBOSS.Where(x => x.bossEntity.Equals(npcEntity)).FirstOrDefault();
            if (boss == null)
            {
                return false;
            }
            return true;
        }

        public static bool AddBoss(string NPCName, int prefabGUIDOfNPC, int level, int multiplier, int lifetime)
        {
            if (GetBoss(NPCName, out BossEncounterModel boss))
            {
                throw new WorldBossExistException();
            }

            var assetName = VWorld.Server.GetExistingSystem<PrefabCollectionSystem>().PrefabDataLookup[new PrefabGUID(prefabGUIDOfNPC)].AssetName.ToString();
            boss = new BossEncounterModel();
            boss.name = NPCName;
            boss.PrefabGUID = prefabGUIDOfNPC;
            boss.AssetName = assetName;
            boss.level = level;
            boss.multiplier = multiplier;
            boss.Lifetime = lifetime;


            WORLDBOSS.Add(boss);
            saveDatabase();
            return true;

        }

        public static bool RemoveBoss(string BossName)
        {
            if (GetBoss(BossName, out BossEncounterModel boss))
            {

                WORLDBOSS.Remove(boss);
                saveDatabase();
                return true;
            }

            throw new WorldBossDontExistException();
        }

        public static T GetRandomItem<T>(this List<T> items)
        {
            if (items == null || items.Count == 0)
            {
                return default;
            }

            return items[Random.Next(items.Count)];
        }


    }
}
