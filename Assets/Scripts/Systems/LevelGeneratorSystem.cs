using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Jobs;

[BurstCompile]
public partial struct LevelGeneratorSystem : ISystem, ISystemStartStop
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<EntityPrefabComponent>().Build());
        state.EntityManager.CreateSingletonBuffer<EntityDynamicBufferComponent>();
        state.EntityManager.CreateSingleton<GameStateDataSingletonComponent>();

        SystemAPI.GetSingletonRW<GameStateDataSingletonComponent>().ValueRW.RequireNewLevel = true;
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        var instance = state.EntityManager.Instantiate(SystemAPI.GetSingleton<EntityPrefabComponent>().Value, 1152, Allocator.Temp);
        SystemAPI.GetSingletonBuffer<EntityDynamicBufferComponent>().AddRange(instance.Reinterpret<EntityDynamicBufferComponent>());
        instance.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var data = SystemAPI.GetSingletonRW<GameStateDataSingletonComponent>();

        if (!data.ValueRO.RequireNewLevel) return;

        var entities = SystemAPI.QueryBuilder().WithAll<FloorComponent, PostTransformMatrix, Disabled>().Build()
            .ToComponentDataArray<PostTransformMatrix>(Allocator.TempJob);

        var slice = entities.Slice(0, 576);

        JobHandle gen = new LevelGenerationJob()
        {
            FreeEntities = slice
        }.ScheduleBatch(slice.Length, 18);

        gen.Complete();

        entities.Dispose();

        data.ValueRW.RequireNewLevel = false;
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {

    }
}