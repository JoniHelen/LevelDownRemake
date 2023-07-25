using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using LevelDown.Components;
using LevelDown.Input;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Initializes projectiles.
    /// </summary>
    [BurstCompile, WithOptions(EntityQueryOptions.IncludeDisabledEntities),
        WithAll(typeof(Projectile)), WithDisabled(typeof(ColorExplosion))]
    public partial struct ProjectileInitializationJob : IJobEntity
    {
        [ReadOnly] public NativeArray<ProjectileDescriptor> Descriptors;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute([EntityIndexInQuery] int entityIndex, [ChunkIndexInQuery] int key, Entity entity,
            ref PhysicsVelocity velocity, ref LocalTransform local)
        {
            if (entityIndex >= Descriptors.Length) return;

            var descriptor = Descriptors[entityIndex];

            local.Position = new float3(descriptor.Position, -1);
            velocity.Linear = new float3(descriptor.Direction, 0) * 20;

            Ecb.SetEnabled(key, entity, true);
        }
    }
}