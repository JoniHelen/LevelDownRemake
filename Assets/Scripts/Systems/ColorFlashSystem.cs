using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public partial struct ColorFlashSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        RefRW<FloorDataSingletonComponent> floorData = 
            SystemAPI.GetComponentRW<FloorDataSingletonComponent>(
                state.EntityManager.CreateSingleton<FloorDataSingletonComponent>("Floor Data")
                );
        floorData.ValueRW.Duration = 0.3;
        floorData.ValueRW.BaseColor = Color.white;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        FloorDataSingletonComponent floorData = SystemAPI.GetSingleton<FloorDataSingletonComponent>();

        new GlowUpdateJob {
            Time = SystemAPI.Time.ElapsedTime,
            Duration = floorData.Duration,
            BaseColor = floorData.BaseColor
        }.ScheduleParallel();
    }
}
