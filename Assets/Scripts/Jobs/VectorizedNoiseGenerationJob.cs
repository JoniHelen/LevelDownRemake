using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using LevelDown.Noise.Vectorized;

namespace LevelDown.Jobs
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public struct VectorizedNoiseGenerationJob : IJobParallelFor
    {
        [WriteOnly] public NativeArray<float4> ResultNoise;
        [ReadOnly] public NativeReference<float2> Offset;
        public float2 Extents;
        public float InvHeight, Scale;

        public void Execute(int index)
        {
            int realIndex = index * 4;

            float4 i = new(realIndex, realIndex + 1, realIndex + 2, realIndex + 3);
            float4 u = math.floor(InvHeight * i + 0.00001f);
            float4 v = i - Extents.y * u;

            u /= Extents.x > Extents.y ? Extents.y : Extents.x;
            v /= Extents.x > Extents.y ? Extents.y : Extents.x;

            u += Offset.Value.x;
            v += Offset.Value.y;

            ResultNoise[index] = Simplex4.GetNoise4(new float4x2(u, v), Scale);
        }
    }
}