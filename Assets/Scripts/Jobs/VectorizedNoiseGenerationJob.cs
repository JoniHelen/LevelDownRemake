using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public struct VectorizedNoiseGenerationJob : IJobParallelFor
{
    [WriteOnly]
    public NativeArray<float4> ResultNoise;

    public float2 Extents, Offset;
    public float invHeight, Scale;

    public void Execute(int index)
    {
        int realIndex = index * 4;

        float4 i = new(realIndex, realIndex + 1, realIndex + 2, realIndex + 3);
        float4 u = math.floor(invHeight * i + 0.00001f);
        float4 v = i - Extents.y * u;

        u /= Extents.x > Extents.y ? Extents.y : Extents.x;
        v /= Extents.x > Extents.y ? Extents.y : Extents.x;

        u += Offset.x;
        v += Offset.y;

        ResultNoise[index] = Simplex4.GetNoise4(new float4x2(u, v), Scale);
    }
}