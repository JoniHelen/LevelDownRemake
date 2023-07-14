using Unity.Physics;
using Unity.Burst;
using Unity.Entities;
using LevelDown.Components;

namespace LevelDown.Jobs
{
    [BurstCompile]
    public struct ProjectileCollisionJob : ICollisionEventsJob
    {
        public ComponentLookup<Projectile> Projectiles;
        public ComponentLookup<ColorExplosion> Explosions;
        public EntityCommandBuffer Ecb;
        public double Time;

        public void Execute(CollisionEvent collisionEvent)
        {
            if (Projectiles.HasComponent(collisionEvent.EntityA))
            {
                Ecb.SetEnabled(collisionEvent.EntityA, false);
                Explosions.SetComponentEnabled(collisionEvent.EntityA, true);
                Explosions.GetRefRW(collisionEvent.EntityA).ValueRW.StartTime = Time;
            }
            else if (Projectiles.HasComponent(collisionEvent.EntityB))
            {
                Ecb.SetEnabled(collisionEvent.EntityB, false);
                Explosions.SetComponentEnabled(collisionEvent.EntityB, true);
                Explosions.GetRefRW(collisionEvent.EntityB).ValueRW.StartTime = Time;
            }
        }
    }
}