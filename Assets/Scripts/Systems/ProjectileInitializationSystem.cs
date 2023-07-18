using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using LevelDown.Components;
using LevelDown.Components.Singletons;
using LevelDown.Jobs;
using EndSimulation = 
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    public partial struct ProjectileInitializationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectileQueue>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<Projectile, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)
                .Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var prefab = SystemAPI.QueryBuilder().WithAll<Projectile, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)
                .Build().GetSingletonEntity();

            var entityCount = SystemAPI.QueryBuilder().WithAll<Projectile, Disabled>()
                .WithDisabled<ColorExplosion>().WithOptions(EntityQueryOptions.IncludeDisabledEntities).Build()
                .CalculateEntityCount();

            state.CompleteDependency();
            var queue = SystemAPI.GetSingleton<ProjectileQueue>().Projectiles;

            var requiredProjectiles = queue.ToArray(Allocator.TempJob);

            if (requiredProjectiles.Length == 0)
            {
                requiredProjectiles.Dispose();
                return;
            }

            queue.Clear();

            if (entityCount < requiredProjectiles.Length)
            {
                state.EntityManager.Instantiate(prefab, requiredProjectiles.Length - entityCount, Allocator.Temp);
            }

            JobHandle init = new ProjectileInitializationJob
            {
                Descriptors = requiredProjectiles,
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(state.Dependency);

            state.Dependency = requiredProjectiles.Dispose(init);
        }
    }
}