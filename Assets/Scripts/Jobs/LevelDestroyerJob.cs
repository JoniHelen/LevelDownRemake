using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Physics;
using Unity.Physics.Aspects;

[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct LevelDestroyerJob : IJobEntity
{
    public double Time;
    public NativeList<Entity>.ParallelWriter Entities;
    [DeallocateOnJobCompletion]
    [ReadOnly] public NativeArray<DistanceHit> Hits;

    public void Execute(Entity entity, ref Shrinking shrink, ref ColorFlash flash, ref RandomValue random, RigidBodyAspect rigidBody)
    {
        unsafe
        {
            var data = Entities.ListData;
            for (var i = 0; i < data->Length; i++)
                if (data->ElementAt(i) == entity) return;
        }
        
        var hitEntity = Entity.Null;

        for (var i = 0; i < Hits.Length; i++)
        {
            if (Hits[i].Entity == entity)
            {
                hitEntity = Hits[i].Entity;
                break;
            }
        }

        if (hitEntity == Entity.Null) return;

        Entities.AddNoResize(hitEntity);
        shrink.Finished = rigidBody.IsKinematic = flash.Finished = false;
        flash.StartTime = shrink.StartTime = Time;
        flash.FlashColor = UnityEngine.Color.HSVToRGB(random.Value.NextFloat(), 1, 1);
    }
}