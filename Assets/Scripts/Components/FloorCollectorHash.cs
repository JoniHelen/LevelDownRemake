using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace LevelDown.Components
{
    [BurstCompile]
    public struct FloorCollectorHash : ICollector<DistanceHit>
    {
        public NativeParallelHashSet<Entity> Comparison { get; set; }

        public NativeList<Entity> Hits { get; set; }

        public bool EarlyOutOnFirstHit => false;

        public float MaxFraction { get; set; }

        public int NumHits { get; private set; }

        public bool AddHit(DistanceHit hit)
        {
            var wasHit = !Comparison.Contains(hit.Entity);

            if (wasHit)
            {
                Hits.Add(hit.Entity);
                NumHits++;
            }

            return wasHit;
        }
    }

    [BurstCompile]
    public struct FloorCollectorArray : ICollector<DistanceHit>
    {
        public NativeArray<Entity> Comparison { get; set; }

        public NativeList<Entity> Hits { get; set; }

        public bool EarlyOutOnFirstHit => false;

        public float MaxFraction { get; set; }

        public int NumHits { get; private set; }

        public bool AddHit(DistanceHit hit)
        {
            var wasHit = !Comparison.Contains(hit.Entity);

            if (wasHit)
            {
                Hits.Add(hit.Entity);
                NumHits++;
            }

            return wasHit;
        }
    }
}