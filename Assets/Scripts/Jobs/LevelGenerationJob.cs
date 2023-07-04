using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

[WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct LevelGenerationJob : IJobEntity
{
    public float2 Extents;
    public float invHeight;
    public float tallThreshold;
    public EntityCommandBuffer.ParallelWriter ecb;

    [ReadOnly]
    public NativeArray<float> Noise;

    public void Execute([ChunkIndexInQuery] int key, [EntityIndexInQuery] int entityIndex, Entity entity, FloorBehaviourAspect behaviour)
    {
        if (entityIndex >= Extents.x * Extents.y) return;

        float x = math.floor(invHeight * entityIndex + 0.00001f);
        float y = entityIndex - Extents.y * x;

        ecb.SetEnabled(key, entity, true);

        behaviour.Position = new float3(x - (Extents.x / 2 - 0.5f), y - (Extents.y / 2f - 0.5f), 0);
        behaviour.SetHeight((Noise[entityIndex] + 1) / 2 > tallThreshold);
    }
}