using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Transforms;
using LevelDown.Components;
using LevelDown.Components.Singletons;
using LevelDown.Components.Managed;
using LevelDown.Input;
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
            var queue = SystemAPI.GetSingleton<ProjectileQueue>();

            state.CompleteDependency();

            foreach (var collision in SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation().CollisionEvents)
            {
                if (_projectileLookup.HasComponent(collision.EntityA))
                {
                    Ecb.SetEnabled(collision.EntityA, false);
                    queue.Explosions.Add(new ExplosionDescriptor
                    {
                        Duration = 0.1f,
                        Size = 3f,
                        Position = _transformLookup[collision.EntityA].Position.xy
                    });
                    SystemAPI.ManagedAPI.GetComponent<ParticleComponent>(collision.EntityA)
                        .Play(_transformLookup[collision.EntityA].Position.xy);
                }
                else if (_projectileLookup.HasComponent(collision.EntityB))
                {
                    Ecb.SetEnabled(collision.EntityB, false);
                    queue.Explosions.Add(new ExplosionDescriptor
                    {
                        Duration = 0.1f,
                        Size = 3f,
                        Position = _transformLookup[collision.EntityB].Position.xy
                    });
                    SystemAPI.ManagedAPI.GetComponent<ParticleComponent>(collision.EntityB)
                        .Play(_transformLookup[collision.EntityB].Position.xy);
                }
            }
        }
    }
}