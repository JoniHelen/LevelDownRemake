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
    [UpdateInGroup(typeof(ProjectileSystemGroup))]
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
            var projectilePrefab = SystemAPI.QueryBuilder().WithAll<Projectile, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)
                .Build().GetSingletonEntity();

            var projectileCount = SystemAPI.QueryBuilder().WithAll<Projectile, Disabled>()
                .WithDisabled<ColorExplosion>().WithOptions(EntityQueryOptions.IncludeDisabledEntities).Build()
                .CalculateEntityCount();

            state.CompleteDependency();
            var singleton = SystemAPI.GetSingleton<ProjectileQueue>();

            var requiredProjectiles = singleton.Projectiles.ToArray(Allocator.TempJob);

            singleton.Projectiles.Clear();

            if (projectileCount < requiredProjectiles.Length)
                state.EntityManager.Instantiate(projectilePrefab, requiredProjectiles.Length - projectileCount, Allocator.Temp);

            JobHandle projectileInit = new ProjectileInitializationJob
            {
                Descriptors = requiredProjectiles,
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(state.Dependency);

            state.Dependency = requiredProjectiles.Dispose(projectileInit);
        }
    }
}