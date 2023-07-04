using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Physics;

[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct LevelDestroyerJob : IJobEntity
{
    public double Time;
    public Entity destroyerSingleton;
    public EntityCommandBuffer.ParallelWriter ecb;
    [ReadOnly] public NativeList<DistanceHit> hits;
    [ReadOnly] public DynamicBuffer<EntityBuffer> buffer;

    public void Execute([ChunkIndexInQuery] int key, Entity entity, ref Shrinking shrink, ref ColorFlash flash, ref RandomValue random)
    {
        Entity hitEntity = Entity.Null;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].Entity == entity)
            {
                hitEntity = hits[i].Entity;
                break;
            }
        }

        if (hitEntity == Entity.Null || buffer.Reinterpret<Entity>().AsNativeArray().Contains(hitEntity)) return;

        ecb.AppendToBuffer(key, destroyerSingleton, new EntityBuffer { entity = hitEntity });
        ecb.SetComponentEnabled<Simulate>(key, hitEntity, true);
        ecb.SetComponentEnabled<Shrinking>(key, hitEntity, true);

        flash.StartTime = shrink.StartTime = Time;
        flash.FlashColor = UnityEngine.Color.HSVToRGB(random.Value.NextFloat(), 1, 1);
        flash.Finished = false;
    }
}