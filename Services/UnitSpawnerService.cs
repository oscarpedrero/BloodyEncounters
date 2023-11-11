using HarmonyLib;
using ProjectM;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace BloodyEncounters.Services;

internal class UnitSpawnerService
{
    private static Entity empty_entity = new Entity();

    internal const int DEFAULT_MINRANGE = 1;
    internal const int DEFAULT_MAXRANGE = 1;
    public static UnitSpawnerService UnitSpawner { get; internal set; } = new();

    public void SpawnWithCallback(Entity user, PrefabGUID unit, float2 position, float duration, Action<Entity> postActions)
    {
        var translation = Plugin.World.EntityManager.GetComponentData<Translation>(user);
        var f3pos = new float3(position.x, translation.Value.y, position.y); // TODO: investigate this copypasta
        var usus = Plugin.World.GetExistingSystem<UnitSpawnerUpdateSystem>();

        UnitSpawnerReactSystem_Patch.Enabled = true;

        var durationKey = NextKey();
        usus.SpawnUnit(empty_entity, unit, f3pos, 1, DEFAULT_MINRANGE, DEFAULT_MAXRANGE, durationKey);
        PostActions.Add(durationKey, (duration, postActions));
    }

    internal long NextKey()
    {
        System.Random r = new();
        long key;
        int breaker = 5;
        do
        {
            key = r.NextInt64(10000) * 3;
            breaker--;
            if (breaker < 0)
            {
                throw new Exception($"Failed to generate a unique key for UnitSpawnerService");
            }
        } while (PostActions.ContainsKey(key));
        return key;
    }

    internal Dictionary<long, (float actualDuration, Action<Entity> Actions)> PostActions = new();

    [HarmonyPatch(typeof(UnitSpawnerReactSystem), nameof(UnitSpawnerReactSystem.OnUpdate))]
    public static class UnitSpawnerReactSystem_Patch
    {
        public static bool Enabled { get; set; } = false;

        public static void Prefix(UnitSpawnerReactSystem __instance)
        {
            if (!Enabled) return;

            var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

            Plugin.Logger.LogDebug($"Processing {entities.Length} in UnitSpawnerReactionSystem");

            foreach (var entity in entities)
            {
                Plugin.Logger.LogDebug($"Checking if {entity.Index} has lifetime..");

                if (!Plugin.World.EntityManager.HasComponent<LifeTime>(entity)) return;

                var lifetimeComp = Plugin.World.EntityManager.GetComponentData<LifeTime>(entity);
                var durationKey = (long)Mathf.Round(lifetimeComp.Duration);
                Plugin.Logger.LogDebug($"Found durationKey {durationKey} from LifeTime({lifetimeComp.Duration})");
                if (UnitSpawner.PostActions.TryGetValue(durationKey, out var unitData))
                {
                    var (actualDuration, actions) = unitData;
                    Plugin.Logger.LogDebug($"Found post actions for {durationKey} with {actualDuration} duration");
                    UnitSpawner.PostActions.Remove(durationKey);

                    var endAction = actualDuration < 0 ? LifeTimeEndAction.None : LifeTimeEndAction.Destroy;

                    var newLifeTime = new LifeTime()
                    {
                        Duration = actualDuration,
                        EndAction = endAction
                    };

                    Plugin.World.EntityManager.SetComponentData(entity, newLifeTime);

                    actions(entity);
                }
            }
        }
    }
}
