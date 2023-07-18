using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Transforms;
using LevelDown.Components;
using LevelDown.Components.Managed;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(ManagedSystemGroup))]
    public partial struct ProjectileCollisionSystem : ISystem
    {
        private ComponentLookup<Projectile> _projectileLookup;
        private ComponentLookup<ColorExplosion> _explosionLookup;
        private ComponentLookup<LocalTransform> _transformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            _projectileLookup = state.GetComponentLookup<Projectile>();
            _explosionLookup = state.GetComponentLookup<ColorExplosion>();
            _transformLookup = state.GetComponentLookup<LocalTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _projectileLookup.Update(ref state);
            _explosionLookup.Update(ref state);
            _transformLookup.Update(ref state);

            var Ecb = SystemAPI.GetSingleton<EndSimulation>().CreateCommandBuffer(state.WorldUnmanaged);

            state.CompleteDependency();

            foreach (var collision in SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation().CollisionEvents)
            {
                if (_projectileLookup.HasComponent(collision.EntityA))
                {
                    Ecb.SetEnabled(collision.EntityA, false);
                    _explosionLookup.SetComponentEnabled(collision.EntityA, true);
                    _explosionLookup.GetRefRW(collision.EntityA).ValueRW.StartTime = SystemAPI.Time.ElapsedTime;
                    SystemAPI.ManagedAPI.GetComponent<ParticleComponent>(collision.EntityA).Play(_transformLookup.GetRefRO(collision.EntityA).ValueRO.Position.xy);
                }
                else if (_projectileLookup.HasComponent(collision.EntityB))
                {
                    Ecb.SetEnabled(collision.EntityB, false);
                    _explosionLookup.SetComponentEnabled(collision.EntityB, true);
                    _explosionLookup.GetRefRW(collision.EntityB).ValueRW.StartTime = SystemAPI.Time.ElapsedTime;
                    SystemAPI.ManagedAPI.GetComponent<ParticleComponent>(collision.EntityB).Play(_transformLookup.GetRefRO(collision.EntityB).ValueRO.Position.xy);
                }
            }
        }
    }
}