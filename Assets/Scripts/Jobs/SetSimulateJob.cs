using Unity.Entities;
using Unity.Burst;

[WithAll(typeof(Floor)), WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct SetSimulateJob : IJobEntity
{
    public bool BoolToSet;
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute([ChunkIndexInQuery] int key, Entity entity) 
        => Ecb.SetComponentEnabled<Simulate>(key, entity, BoolToSet);
}
