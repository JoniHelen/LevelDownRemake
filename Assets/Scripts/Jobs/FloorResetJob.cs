using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;


[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct FloorResetJob : IJobEntity
{
    public double Time;
    public FloorPhysicsBlobs PhysicsBlobs;
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute([ChunkIndexInQuery] int key, Entity entity, ref Shrinking shrink, FloorBehaviourAspect behaviour)
    {
        var elapsed = (float)(Time - shrink.StartTime);

        if (elapsed >= shrink.Duration)
        {
            Ecb.SetEnabled(key, entity, false);
            Ecb.SetComponentEnabled<Shrinking>(key, entity, false);
            Ecb.SetComponentEnabled<Simulate>(key, entity, false);

            behaviour.Reset(PhysicsBlobs);
        }
        else
        {
            behaviour.Scale = math.lerp(behaviour.Tall ? behaviour.TallScale : 1, 0, elapsed / shrink.Duration);
        }
    }
}
