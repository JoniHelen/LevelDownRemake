using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Physics;
using Unity.Physics.Aspects;
using LevelDown.Components;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    /// <summary>
    /// "Destroys" the floor tiles that get hit during level destruction.
    /// </summary>
    [BurstCompile, WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct LevelDestroyerJob : IJobEntity
    {
        public double Time;
        public NativeList<Entity>.ParallelWriter Entities;
        [ReadOnly] public NativeList<DistanceHit> Hits;

        public void Execute(Entity entity, ref RandomValue random, RigidBodyAspect rigidBody,
            ColorFlashAspect flash, ShrinkingAspect shrink)
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

            // Flash a color and start "destruction"
            Entities.AddNoResize(hitEntity);
            rigidBody.IsKinematic = false;
            flash.StartTime = shrink.StartTime = Time;
            flash.Enabled = shrink.Enabled = true;
            flash.FlashColor = UnityEngine.Color.HSVToRGB(random.Value.NextFloat(), 1, 1);
        }
    }
}