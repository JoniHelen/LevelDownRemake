using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using LevelDown.Components.Aspects;
using LevelDown.Components;
using LevelDown.Input;

namespace LevelDown.Jobs
{
    [BurstCompile, WithOptions(EntityQueryOptions.IgnoreComponentEnabledState, EntityQueryOptions.IncludeDisabledEntities)]
    public partial struct ExplosionInitializationJob : IJobEntity
    {
        public double Time;
        [ReadOnly] public NativeArray<ExplosionDescriptor> Descriptors;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute([EntityIndexInQuery] int entityIndex, [ChunkIndexInQuery] int key, Entity entity,
            ColorExplosionAspect explosion, ref LocalTransform local)
        {
            if (entityIndex >= Descriptors.Length) return;

            var descriptor = Descriptors[entityIndex];

            local.Position = new float3(descriptor.Position, -1);

            explosion.Enabled = true;
            explosion.StartTime = Time;
            explosion.TargetSize = descriptor.Size;
            explosion.Duration = descriptor.Duration;
            Ecb.SetEnabled(key, entity, true);
        }
    }
}