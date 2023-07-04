using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;


[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct FloorResetJob : IJobEntity
{
    public double Time;
    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute([ChunkIndexInQuery] int key, Entity entity, ref Shrinking shrink,
        ref LocalTransform local, ref PhysicsVelocity vel, FloorBehaviourAspect behaviour)
    {
        float elapsed = (float)(Time - shrink.StartTime);

        if (elapsed >= shrink.Duration)
        {
            ecb.SetEnabled(key, entity, false);
            ecb.SetComponentEnabled<Shrinking>(key, entity, false);
            ecb.SetComponentEnabled<Simulate>(key, entity, false);

            behaviour.SetHeight(false);
            behaviour.Scale = 1;

            local.Position = 0;
            local.Rotation = quaternion.identity;

            vel.Linear = vel.Angular = 0;
        }
        else
        {
            float t = math.clamp(elapsed / shrink.Duration, 0, 1);
            behaviour.Scale = new float3(1 - t, 1 - t, math.lerp(behaviour.Tall ? 2 : 1, 0, t));
        }
    }
}
