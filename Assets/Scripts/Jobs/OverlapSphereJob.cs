using Unity.Physics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using LevelDown.Components.Singletons;

namespace LevelDown.Jobs
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public unsafe struct OverlapSphereJob : IJob
    {
        public float TimeSinceStart;
        public NativeArray<DistanceHit> Hits;
        public CollisionWorld CollisionWorld;
        public LevelDestroyerData DestroyerData;
        public void Execute()
        {
            NativeList<DistanceHit> distanceHits = new(Allocator.Temp);
            _ = CollisionWorld.OverlapSphere(0, math.lerp(0, DestroyerData.TargetRadius, TimeSinceStart / DestroyerData.Duration), ref distanceHits, CollisionFilter.Default);
            NativeArray<DistanceHit>.Copy(distanceHits.AsArray(), Hits, distanceHits.Length);
            distanceHits.Dispose();
        }
    }
}