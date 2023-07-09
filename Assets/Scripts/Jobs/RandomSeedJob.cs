using Unity.Entities;
using Unity.Burst;
using LevelDown.Components;
using LevelDown.Noise;


namespace LevelDown.Jobs
{
    /// <summary>
    /// Seeds the random components of floor tiles.
    /// </summary>
    [BurstCompile, WithAll(typeof(Floor)), WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
    public partial struct RandomSeedJob : IJobEntity
    {
        public void Execute([ChunkIndexInQuery] int chunkIndex, [EntityIndexInChunk] int entityIndex, ref RandomValue random)
            => random.Value = new(SmallXXHash.Seed(chunkIndex + entityIndex + 1));
    }
}