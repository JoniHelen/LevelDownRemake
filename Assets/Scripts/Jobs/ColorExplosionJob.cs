using Unity.Entities;
using Unity.Burst;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;
using LevelDown.Components;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    [BurstCompile, WithAll(typeof(Disabled)), WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
    public partial struct ColorExplosionJob : IJobEntity
    {
        public double Time;
        public CollisionWorld CollisionWorld;
        public ComponentLookup<ColorFlash> FlashLookup;
        public ComponentLookup<RandomValue> RandomLookup;

        public void Execute(ColorExplosionAspect explosion, ref LocalTransform local)
        {
            var elapsed = (float)(Time - explosion.StartTime);

            if (elapsed > explosion.Duration)
            {
                explosion.Enabled = false;
                explosion.Buffer.Clear();
            }
            else
            {
                var hits = new NativeList<DistanceHit>(Allocator.Temp);

                var radius = elapsed / explosion.Duration * explosion.TargetSize;
                CollisionWorld.OverlapSphere(local.Position, radius, ref hits, new CollisionFilter
                {
                    BelongsTo = 1u << 31,
                    CollidesWith = 1u << 2 | 1u << 3,
                    GroupIndex = 0
                });

                foreach (var hit in hits)
                {
                    if (explosion.Buffer.Reinterpret<Entity>().AsNativeArray().Contains(hit.Entity) || !FlashLookup.HasComponent(hit.Entity)) continue;

                    explosion.Buffer.Add(new EntityBufferData { Entity = hit.Entity });

                    var flash = FlashLookup.GetRefRW(hit.Entity);
                    flash.ValueRW.StartTime = Time;
                    flash.ValueRW.FlashBrightness = 5f;
                    flash.ValueRW.FlashColor = UnityEngine.Color.HSVToRGB(RandomLookup[hit.Entity].Value.NextFloat(), 1, 1);
                    FlashLookup.SetComponentEnabled(hit.Entity, true);
                }
            }
        }
    }
}