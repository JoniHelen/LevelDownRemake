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
            var projectilePrefab = SystemAPI.QueryBuilder().WithAll<Projectile, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)
                .Build().GetSingletonEntity();

            var explosionPrefab = SystemAPI.QueryBuilder().WithAll<ColorExplosion, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities | EntityQueryOptions.IgnoreComponentEnabledState)
                .Build().GetSingletonEntity();

            var projectileCount = SystemAPI.QueryBuilder().WithAll<Projectile, Disabled>()
                .WithOptions(EntityQueryOptions.IncludeDisabledEntities).Build()
                .CalculateEntityCount();

            var explosionCount = SystemAPI.QueryBuilder().WithAll<ColorExplosion, Disabled>()
                .WithOptions(EntityQueryOptions.IncludeDisabledEntities | EntityQueryOptions.IgnoreComponentEnabledState).Build()
                .CalculateEntityCount();

            state.CompleteDependency();
            var singleton = SystemAPI.GetSingleton<ProjectileQueue>();

            var requiredProjectiles = singleton.Projectiles.ToArray(Allocator.TempJob);
            var requiredExplosions = singleton.Explosions.ToArray(Allocator.TempJob);

            if (requiredProjectiles.Length == 0)
            {
                requiredProjectiles.Dispose();
                return;
            }

            singleton.Projectiles.Clear();

            if (projectileCount < requiredProjectiles.Length)
                state.EntityManager.Instantiate(projectilePrefab, requiredProjectiles.Length - projectileCount, Allocator.Temp);

            if (explosionCount < requiredProjectiles.Length)
                state.EntityManager.Instantiate(explosionPrefab, requiredProjectiles.Length - explosionCount, Allocator.Temp);

            JobHandle expolsionInit = new ExplosionInitializationJob
            {
                Descriptors = requiredExplosions,
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                Time = SystemAPI.Time.ElapsedTime
            }.Schedule(state.Dependency);

            JobHandle projectileInit = new ProjectileInitializationJob
            {
                Descriptors = requiredProjectiles,
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(expolsionInit);

            state.Dependency = JobHandle.CombineDependencies(
                requiredProjectiles.Dispose(projectileInit),
                requiredExplosions.Dispose(expolsionInit));
        }
    }
}