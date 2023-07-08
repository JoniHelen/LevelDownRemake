using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Physics;
using Unity.Physics.Aspects;
using LevelDown.Components;

namespace LevelDown.Jobs
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public partial struct LevelDestroyerJob : IJobEntity
    {
        public double Time;
        [NativeDisableParallelForRestriction]
        public ComponentLookup<Shrinking> ShrinkingLookup;
        [NativeDisableParallelForRestriction]
        public ComponentLookup<ColorFlash> FlashLookup;
        public NativeList<Entity>.ParallelWriter Entities;
        [ReadOnly] public NativeList<DistanceHit> Hits;

        public void Execute(Entity entity, ref RandomValue random, RigidBodyAspect rigidBody)
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

            var shrink = ShrinkingLookup.GetRefRW(hitEntity);
            var flash = FlashLookup.GetRefRW(hitEntity);

            Entities.AddNoResize(hitEntity);
            rigidBody.IsKinematic = false;
            flash.ValueRW.StartTime = shrink.ValueRW.StartTime = Time;
            flash.ValueRW.FlashColor = UnityEngine.Color.HSVToRGB(random.Value.NextFloat(), 1, 1);
            ShrinkingLookup.SetComponentEnabled(hitEntity, true);
            FlashLookup.SetComponentEnabled(hitEntity, true);
        }
    }
}