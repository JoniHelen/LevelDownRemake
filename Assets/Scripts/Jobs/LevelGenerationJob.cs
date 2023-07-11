using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using LevelDown.Components;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Generates a new level with a random color.
    /// </summary>
    [BurstCompile, WithOptions(EntityQueryOptions.IncludeDisabledEntities, EntityQueryOptions.IgnoreComponentEnabledState), WithAll(typeof(Disabled))]
    public partial struct LevelGenerationJob : IJobEntity
    {
        public float2 Extents;
        public float InvHeight;
        public float TallThreshold;
        public UnityEngine.Color BaseColor;
        public EntityCommandBuffer.ParallelWriter Ecb;
        [ReadOnly] public NativeArray<float> Noise;

        public void Execute([ChunkIndexInQuery] int key, [EntityIndexInQuery] int entityIndex,
            Entity entity, ref ColorFlash flash, ref GlowColor color, FloorBehaviourAspect behaviour)
        {
            if (entityIndex >= Extents.x * Extents.y) return; // Only accept entities in bounds of the level

            // Get the coordinates of the floor tile
            var x = math.floor(InvHeight * entityIndex + 0.00001f);
            var y = entityIndex - Extents.y * x;

            Ecb.SetEnabled(key, entity, true);
            flash.BaseColor = color.Color = BaseColor;

            behaviour.Initialize((Noise[entityIndex] + 1) / 2 > TallThreshold,
                new(x - (Extents.x / 2 - 0.5f), y - (Extents.y / 2f - 0.5f)));
        }
    }
}