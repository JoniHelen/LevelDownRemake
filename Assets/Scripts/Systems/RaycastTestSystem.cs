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

        float3 endDir = new float3(math.cos((float)math.radians(SystemAPI.Time.ElapsedTime * 360) / 2), (float)math.sin(math.radians(SystemAPI.Time.ElapsedTime * 360) / 2), 0);

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
            var flash = SystemAPI.GetComponentRW<ColorFlashComponent>(hit.Entity);
            if (flash.IsValid)
            {
                if (flash.ValueRO.Finished)
                    flash.ValueRW.Finished = false;

                flash.ValueRW.StartTime = SystemAPI.Time.ElapsedTime;

                float3 rnd = math.normalize(_Random.NextFloat3());

                flash.ValueRW.FlashColor = UnityEngine.Color.HSVToRGB(_Random.NextFloat(), 1f, 1f);//new UnityEngine.Color(rnd.x, rnd.y, rnd.z);
            }
        }
    }
}