using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using LevelDown.Components.Aspects;

namespace LevelDown.Jobs
{
    [BurstCompile, UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ColorExplosionJob : IJobEntity
    {
        public double Time;
        public EntityCommandBuffer.ParallelWriter Ecb;

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
                local.Scale = elapsed / explosion.Duration * explosion.TargetSize;
            }
        }
    }
}