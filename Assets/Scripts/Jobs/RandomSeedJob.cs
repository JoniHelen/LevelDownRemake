using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[WithAll(typeof(Floor)), WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct RandomSeedJob : IJobEntity
{
    public void Execute([ChunkIndexInQuery] int chunkIndex, [EntityIndexInChunk] int entityIndex, ref RandomValue random)
        => random.Value = new(SmallXXHash.Seed(chunkIndex + entityIndex + 1));
}
