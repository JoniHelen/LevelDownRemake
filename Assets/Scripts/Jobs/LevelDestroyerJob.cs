using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Physics;

[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct LevelDestroyerJob : IJobEntity
{
    public double Time;
    public Entity DestroyerSingleton;
    public EntityCommandBuffer.ParallelWriter Ecb;
    [ReadOnly] public NativeList<DistanceHit> Hits;
    [ReadOnly] public DynamicBuffer<EntityBuffer> Buffer;

    public void Execute([ChunkIndexInQuery] int key, Entity entity, ref Shrinking shrink, ref ColorFlash flash, ref RandomValue random)
    {
        var hitEntity = Entity.Null;

        for (var i = 0; i < Hits.Length; i++)
        {
            if (Hits[i].Entity == entity)
            {
                hitEntity = Hits[i].Entity;
                break;
            }
        }

        if (hitEntity == Entity.Null || Buffer.Reinterpret<Entity>().AsNativeArray().Contains(hitEntity)) return;

        Ecb.AppendToBuffer(key, DestroyerSingleton, new EntityBuffer { entity = hitEntity });
        Ecb.SetComponentEnabled<Simulate>(key, hitEntity, true);
        Ecb.SetComponentEnabled<Shrinking>(key, hitEntity, true);

        flash.StartTime = shrink.StartTime = Time;
        flash.FlashColor = UnityEngine.Color.HSVToRGB(random.Value.NextFloat(), 1, 1);
        flash.Finished = false;
    }
}