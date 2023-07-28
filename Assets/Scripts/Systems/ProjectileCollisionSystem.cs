using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Burst;
using Unity.Transforms;
using LevelDown.Components;
using LevelDown.Components.Managed;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
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
                var ent = Entity.Null;
                if (_projectileLookup.HasComponent(collision.EntityA))
                    ent = collision.EntityA;
                else if (_projectileLookup.HasComponent(collision.EntityB))
                    ent = collision.EntityB;

                if (ent == Entity.Null) return;

                Ecb.SetEnabled(ent, false);
                _explosionLookup.SetComponentEnabled(ent, true);
                _explosionLookup.GetRefRW(ent).ValueRW.StartTime = SystemAPI.Time.ElapsedTime;
                SystemAPI.ManagedAPI.GetComponent<ParticleComponent>(ent)
                    .Play(_transformLookup[ent].Position.xy);
            }
        }
    }
}