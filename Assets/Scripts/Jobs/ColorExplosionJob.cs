using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using LevelDown.Components;
using LevelDown.Components.Aspects;
using Unity.Collections;
using Unity.Physics;

namespace LevelDown.Jobs
{
    [BurstCompile, WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
    public partial struct ColorExplosionJob : IJobEntity
    {
        public double Time;
        public EntityCommandBuffer.ParallelWriter Ecb;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<ColorFlash> FlashLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<RandomValue> RandomLookup;

        [ReadOnly] public CollisionWorld QueryWorld;

        public void Execute([ChunkIndexInQuery] int key, Entity entity,
            ColorExplosionAspect explosion, ref LocalTransform local)
        {
            var elapsed = (float)(Time - explosion.StartTime);

            if (elapsed > explosion.Duration)
            {
                Ecb.SetEnabled(key, entity, false);
                explosion.Enabled = false;
                explosion.Buffer.Clear();
            }
            else
            {
                var hits = new NativeList<Entity>(20, Allocator.Temp);
                var radius = elapsed / explosion.Duration * explosion.TargetSize;
                var collector = new FloorCollectorArray
                {
                    Comparison = explosion.Buffer.Reinterpret<Entity>().AsNativeArray(),
                    Hits = hits,
                    MaxFraction = radius
                };

                QueryWorld.OverlapSphereCustom(local.Position, radius, ref collector, new CollisionFilter
                {
                    BelongsTo = 1u << 31,
                    CollidesWith = (1u << 2) | (1u << 3),
                    GroupIndex = 0
                });

                for (int i = 0, n = hits.Length; i < n; i++)
                {
                    var hitEntity = hits[i];

                    explosion.Buffer.Add(hitEntity);

                    var flash = FlashLookup.GetRefRW(hitEntity);
                    flash.ValueRW.StartTime = Time;
                    flash.ValueRW.FlashBrightness = 10f;
                    flash.ValueRW.FlashColor = UnityEngine.Color.HSVToRGB(RandomLookup.GetRefRW(hitEntity).ValueRW.Value.NextFloat(), 1, 1);
                    FlashLookup.SetComponentEnabled(hitEntity, true);
                }
            }
        }
    }
}