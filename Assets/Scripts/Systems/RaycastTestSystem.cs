using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public partial struct RaycastTestSystem : ISystem
{
    private Random _Random;
    private Entity _BufferEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _Random = new Random();
        _Random.InitState();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        CollisionWorld world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        float3 endDir = new(math.cos((float)math.radians(SystemAPI.Time.ElapsedTime * 360)), (float)math.sin(math.radians(SystemAPI.Time.ElapsedTime * 360)), 0);

        RaycastInput input = new RaycastInput
        {
            Start = float3.zero,
            End = endDir * 20f,
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            }
        };

        UnityEngine.Debug.DrawLine(input.Start, input.End);

        if (world.CastRay(input, out RaycastHit hit))
        {
            if (_BufferEntity == hit.Entity) return;

            _BufferEntity = hit.Entity;

            var flash = SystemAPI.GetComponentRW<ColorFlashComponent>(hit.Entity);
            if (flash.IsValid)
            {
                if (flash.ValueRO.Finished)
                    flash.ValueRW.Finished = false;

                flash.ValueRW.StartTime = SystemAPI.Time.ElapsedTime;

                flash.ValueRW.FlashColor = UnityEngine.Color.HSVToRGB(_Random.NextFloat(), 1f, 1f);
            }

            var floor = SystemAPI.GetAspect<FloorBehaviourAspect>(hit.Entity);

            floor.SetHeight(!floor.Tall);
        }
    }
}