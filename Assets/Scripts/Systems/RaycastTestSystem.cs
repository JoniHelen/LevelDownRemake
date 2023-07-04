using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Mathematics;

public partial struct RaycastTestSystem : ISystem
{
    private Random _Random;
    private Entity _BufferEntity;

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnCreate(ref SystemState state)
    {
        _Random = new Random();
        _Random.InitState();
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state)
    {
        CollisionWorld world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        float3 endDir = new(math.cos((float)math.radians(SystemAPI.Time.ElapsedTime * 360) / 2f), (float)math.sin(math.radians(SystemAPI.Time.ElapsedTime * 360) / 2f), 0);

        RaycastInput input = new()
        {
            Start = new float3(0, 0, -1),
            End = endDir * 20f + new float3(0, 0, -1),
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            }
        };
        /*
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
        }*/
    }
}