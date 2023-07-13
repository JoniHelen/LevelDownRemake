using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics.Aspects;
using Unity.Transforms;
using Unity.Mathematics;
using LevelDown.Components;
using LevelDown.Input;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Stops initialized floor tiles at the proper point in space.
    /// </summary>
    [BurstCompile, WithOptions(EntityQueryOptions.IncludeDisabledEntities), WithAll(typeof(Projectile))]
    public partial struct ProjectileInitializationJob : IJobEntity
    {
        [ReadOnly] public NativeArray<ProjectileDescriptor> Descriptors;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute([EntityIndexInQuery] int entityIndex, [ChunkIndexInQuery] int key, RigidBodyAspect rigidBody)
        {
            if (entityIndex >= Descriptors.Length) return;

            var descriptor = Descriptors[entityIndex];

            rigidBody.Position = new float3(descriptor.Position, -1);
            rigidBody.LinearVelocity = new float3(descriptor.Direction, 0) * 10;

            Ecb.SetEnabled(key, rigidBody.Entity, true);
        }
    }
}