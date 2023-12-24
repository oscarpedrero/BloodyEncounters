using Bloodstone.API;
using BloodyEncounters.Commands;
using BloodyEncounters.DB;
using BloodyEncounters.DB.Models;
using BloodyEncounters.Systems;
using Cpp2IL.Core.Api;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Rendering.HighDefinition;


/**
 * 
 * Based in Code By syllabicat from VBloodKills (https://github.com/syllabicat/VBloodKills)
 * 
**/
namespace BloodyEncounters.Patch;

[HarmonyPatch]
public class VBloodSystem_Patch
{

    private const double SendMessageDelay = 2;
    private static bool checkKiller = false;
    private static Dictionary<string, DateTime> lastKillerUpdate = new();
    private static EntityManager entityManager = VWorld.Server.EntityManager;

    [HarmonyPatch(typeof(VBloodSystem), nameof(VBloodSystem.OnUpdate))]
    [HarmonyPrefix]
    public static void OnUpdate_Prefix(VBloodSystem __instance)
    {

        if (__instance.EventList.Length > 0)
        {
            foreach (var event_vblood in __instance.EventList)
            {
                if (entityManager.HasComponent<PlayerCharacter>(event_vblood.Target))
                {
                    var player = entityManager.GetComponentData<PlayerCharacter>(event_vblood.Target);
                    var user = entityManager.GetComponentData<User>(player.UserEntity);
                    var vblood = __instance._PrefabCollectionSystem.PrefabDataLookup[event_vblood.Source].AssetName;

                    var modelBoss = Database.WORLDBOSS.Where(x => x.AssetName == vblood.ToString() && x.bossEntity != null).FirstOrDefault();

                    if (modelBoss != null)
                    {      
                        WorldBossSystem.AddKiller(vblood.ToString(), user.CharacterName.ToString());
                        lastKillerUpdate[vblood.ToString()] = DateTime.Now;
                        checkKiller = true;
                    }
                }
            }
        }
        else if (checkKiller)
        {
            var didSkip = false;
            foreach (KeyValuePair<string, DateTime> kvp in lastKillerUpdate)
            {

                var lastUpdateTime = kvp.Value;
                if (DateTime.Now - lastUpdateTime < TimeSpan.FromSeconds(SendMessageDelay))
                {
                    didSkip = true;
                    continue;
                }
                var modelBoss = Database.WORLDBOSS.Where(x => x.AssetName == kvp.Key && x.bossEntity != null).FirstOrDefault();
                if (modelBoss != null)
                {
                    var vBloodQuery = __instance.EntityManager.CreateEntityQuery(new EntityQueryDesc()
                    {
                        All = new ComponentType[] {
                                ComponentType.ReadOnly<VBloodUnit>(),
                                ComponentType.ReadOnly<LifeTime>()
                            },
                        Options = EntityQueryOptions.IncludeAll
                    });

                    var vBloods = vBloodQuery.ToEntityArray(Allocator.Temp);

                    foreach (var entity in vBloods)
                    {
                        if (entity.Equals(modelBoss.bossEntity))
                        {
                            WorldBossCommand._lastBossSpawnModel = null;
                            WorldBossSystem.SendAnnouncementMessage(kvp.Key, modelBoss);
                            break;
                        }
                    }
                }
            }
            checkKiller = didSkip;
        }
        
    }
}
