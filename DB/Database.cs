using BepInEx;
using BloodyEncounters.DB.Models;
using BloodyEncounters.Exceptions;
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

        public static List<NpcEncounterModel> NPCS { get; set; } = new();

        public static void Initialize()
        {
            createDatabaseFiles();
            loadDatabase();
        }

        public static bool createDatabaseFiles()
        {
            if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);
            if (!File.Exists(NPCSListFile)) File.WriteAllText(NPCSListFile, "[]");
            Plugin.Logger.LogDebug($"Create Database: OK");
            return true;
        }

        public static bool saveDatabase()
        {
            try
            {
                var jsonOutPut = JsonSerializer.Serialize(NPCS, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(NPCSListFile, jsonOutPut);
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
                Plugin.Logger.LogDebug($"Load Database: OK");
                return true;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error LoadDatabase: {error.Message}");
                return false;
            }
        }

        public static bool GetNPC(string NPCName, out 
            
            NpcEncounterModel npc)
        {
            npc = NPCS.Where(x => x.name == NPCName).FirstOrDefault();
            if (npc == null)
            {
                return false;
            }
            return true;
        }

        public static bool GetNPCFromEntity(Entity npcEntity , out NpcEncounterModel merchant)
        {
            merchant = NPCS.Where(x => x.npcEntity.Equals(npcEntity)).FirstOrDefault();
            if (merchant == null)
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

        public static bool RemoveNPC(string MerchantName)
        {
            if (GetNPC(MerchantName, out NpcEncounterModel merchant))
            {

                NPCS.Remove(merchant);
                saveDatabase();
                return true;
            }

            throw new NPCDontExistException();
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
