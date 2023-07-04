using Unity.Entities;
using Unity.Burst;

[WithAll(typeof(Floor)), WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct SetSimulateJob : IJobEntity
{
    public bool boolToSet;
    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute([ChunkIndexInQuery] int key, Entity entity) 
        => ecb.SetComponentEnabled<Simulate>(key, entity, boolToSet);
}
