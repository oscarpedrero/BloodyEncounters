using Bloodstone.API;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using Unity.Collections;
using Unity.Entities;

namespace BloodyEncounters.Systems;

public static class Il2cppService
{
    private static Type GetType<T>() => Il2CppType.Of<T>();
    
    public static unsafe T GetComponentDataAOT<T>(this EntityManager entityManager, Entity entity) where T : unmanaged
    {
        var type = TypeManager.GetTypeIndex(GetType<T>());
        var result = (T*)entityManager.GetComponentDataRawRW(entity, type);

        return *result;
    }

    public static NativeArray<Entity> GetEntitiesByComponentTypes<T1>(bool includeAll = false)
    {
        EntityQueryOptions options = includeAll ? EntityQueryOptions.IncludeAll : EntityQueryOptions.Default;

        EntityQueryDesc queryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { 
                new ComponentType(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite) 
            },
            Options = options
        };

        var query = VWorld.Server.EntityManager.CreateEntityQuery(queryDesc);
        var entities = query.ToEntityArray(Allocator.Temp);

        return entities;
    }

    public static NativeArray<Entity> GetEntitiesByComponentTypes<T1, T2>(bool includeAll = false)
    {
        EntityQueryOptions options = includeAll ? EntityQueryOptions.IncludeAll : EntityQueryOptions.Default;

        EntityQueryDesc queryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { 
                new ComponentType(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite), 
                new ComponentType(Il2CppType.Of<T2>(), ComponentType.AccessMode.ReadWrite) 
            },
            Options = options
        };

        var query = VWorld.Server.EntityManager.CreateEntityQuery(queryDesc);
        var entities = query.ToEntityArray(Allocator.Temp);

        return entities;
    }

    public static NativeArray<Entity> GetEntitiesByComponentTypes<T1, T2, T3>(bool includeAll = false)
    {
        EntityQueryOptions options = includeAll ? EntityQueryOptions.IncludeAll : EntityQueryOptions.Default;

        EntityQueryDesc queryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { 
                new ComponentType(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite), 
                new ComponentType(Il2CppType.Of<T2>(), ComponentType.AccessMode.ReadWrite), 
                new ComponentType(Il2CppType.Of<T3>(), ComponentType.AccessMode.ReadWrite) 
            },
            Options = options
        };

        var query = VWorld.Server.EntityManager.CreateEntityQuery(queryDesc);
        var entities = query.ToEntityArray(Allocator.Temp);

        return entities;
    }
}