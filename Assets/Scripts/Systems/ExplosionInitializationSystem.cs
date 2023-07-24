using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using LevelDown.Jobs;
using LevelDown.Components;
using LevelDown.Components.Singletons;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(ProjectileSystemGroup)), UpdateAfter(typeof(ProjectileInitializationSystem))]
    public partial struct ExplosionInitializationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectileQueue>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<ColorExplosion, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)
                .Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var explosionPrefab = SystemAPI.QueryBuilder().WithAll<ColorExplosion, Prefab>()
                .WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities | EntityQueryOptions.IgnoreComponentEnabledState)
                .Build().GetSingletonEntity();

            var explosionCount = SystemAPI.QueryBuilder().WithAll<ColorExplosion, Disabled>()
                .WithOptions(EntityQueryOptions.IncludeDisabledEntities | EntityQueryOptions.IgnoreComponentEnabledState).Build()
                .CalculateEntityCount();

            var singleton = SystemAPI.GetSingleton<ProjectileQueue>();

            var requiredExplosions = singleton.Explosions.ToArray(Allocator.TempJob);

            singleton.Explosions.Clear();

            if (explosionCount < requiredExplosions.Length)
                state.EntityManager.Instantiate(explosionPrefab, requiredExplosions.Length - explosionCount, Allocator.Temp);

            JobHandle expolsionInit = new ExplosionInitializationJob
            {
                Descriptors = requiredExplosions,
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                Time = SystemAPI.Time.ElapsedTime
            }.ScheduleParallel(state.Dependency);

            state.Dependency = requiredExplosions.Dispose(expolsionInit);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}