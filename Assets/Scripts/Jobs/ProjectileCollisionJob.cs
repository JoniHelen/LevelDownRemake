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
            if (Projectiles.HasComponent(collisionEvent.EntityA))
            {
                Ecb.SetEnabled(collisionEvent.EntityA, false);
                Explosions.Add(new ExplosionDescriptor
                {
                    Duration = 0.1f,
                    Size = 1.5f,
                    Position = LocalTransforms[collisionEvent.EntityA].Position.xy
                });
            }
            else if (Projectiles.HasComponent(collisionEvent.EntityB))
            {
                Ecb.SetEnabled(collisionEvent.EntityB, false);
                Explosions.Add(new ExplosionDescriptor
                {
                    Duration = 0.1f,
                    Size = 1.5f,
                    Position = LocalTransforms[collisionEvent.EntityB].Position.xy
                });
            }
        }
    }
}