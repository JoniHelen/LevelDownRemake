using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Physics;
using Unity.Physics.Aspects;
using LevelDown.Components;

namespace LevelDown.Jobs
{
    /// <summary>
    /// "Destroys" the floor tiles that get hit during level destruction.
    /// </summary>
    [BurstCompile]
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
            // Find if the entity has already been "destroyed"
            unsafe
            {
                var data = Entities.ListData;
                for (var i = 0; i < data->Length; i++)
                    if (data->ElementAt(i) == entity) return;
            }

            // Find if the entity was hit this frame
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

            // Flash a color and start "destruction"
            Entities.AddNoResize(hitEntity);
            rigidBody.IsKinematic = false;
            flash.ValueRW.StartTime = shrink.ValueRW.StartTime = Time;
            flash.ValueRW.FlashColor = UnityEngine.Color.HSVToRGB(random.Value.NextFloat(), 1, 1);
            ShrinkingLookup.SetComponentEnabled(hitEntity, true);
            FlashLookup.SetComponentEnabled(hitEntity, true);
        }
    }
}