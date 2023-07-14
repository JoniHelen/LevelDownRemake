using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using LevelDown.Jobs;
using LevelDown.Components;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    public partial struct ProjectileCollisionSystem : ISystem
    {
        private ComponentLookup<Projectile> _projectileLookup;
        private ComponentLookup<ColorExplosion> _explosionLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            _projectileLookup = state.GetComponentLookup<Projectile>();
            _explosionLookup = state.GetComponentLookup<ColorExplosion>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _projectileLookup.Update(ref state);
            _explosionLookup.Update(ref state);

            state.Dependency = new ProjectileCollisionJob
            {
                Time = SystemAPI.Time.ElapsedTime,
                Explosions = _explosionLookup,
                Projectiles = _projectileLookup,
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged)
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }
    }
}