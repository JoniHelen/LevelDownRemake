using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

[WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct LevelGenerationJob : IJobEntity
{
    public float2 Extents;
    public float InvHeight;
    public float TallThreshold;
    public FloorPhysicsBlobs PhysicsBlobs;
    public EntityCommandBuffer.ParallelWriter Ecb;

    [ReadOnly]
    public NativeArray<float> Noise;

    public void Execute([ChunkIndexInQuery] int key, [EntityIndexInQuery] int entityIndex, Entity entity, FloorBehaviourAspect behaviour)
    {
        if (entityIndex >= Extents.x * Extents.y) return;

        var x = math.floor(InvHeight * entityIndex + 0.00001f);
        var y = entityIndex - Extents.y * x;

        Ecb.SetEnabled(key, entity, true);

        behaviour.Position = new(x - (Extents.x / 2 - 0.5f), y - (Extents.y / 2f - 0.5f), 0);
        behaviour.SetHeight((Noise[entityIndex] + 1) / 2 > TallThreshold, PhysicsBlobs);
    }
}