using Unity.Physics;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using LevelDown.Components;
using LevelDown.Input;
using Unity.Transforms;

namespace LevelDown.Jobs
{
    [BurstCompile]
    public struct ProjectileCollisionJob : ICollisionEventsJob
    {
        public ComponentLookup<Projectile> Projectiles;
        public ComponentLookup<LocalTransform> LocalTransforms;
        [WriteOnly] public NativeList<ExplosionDescriptor> Explosions;
        public EntityCommandBuffer Ecb;
        public double Time;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entity = Entity.Null;

            if (Projectiles.HasComponent(collisionEvent.EntityA))
                entity = collisionEvent.EntityA;
            else if (Projectiles.HasComponent(collisionEvent.EntityB))
                entity = collisionEvent.EntityB;

            if (entity == Entity.Null) return;

            Ecb.SetEnabled(entity, false);
            Explosions.Add(new ExplosionDescriptor
            {
                Duration = 0.1f,
                Size = 1.5f,
                Position = LocalTransforms[entity].Position.xy
            });
        }
    }
}