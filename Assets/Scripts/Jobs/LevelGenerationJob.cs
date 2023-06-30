using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public partial struct LevelGenerationJob : IJobParallelForBatch
{
    public NativeSlice<PostTransformMatrix> FreeEntities;

    [BurstCompile]
    public void Execute(int startIndex, int count)
    {
        int x = startIndex / 18;

        for (int i = startIndex; i < count; i++)
        {
            int y = i - startIndex;

            PostTransformMatrix matrix = FreeEntities[i];
            matrix.Value = math.mul(float4x4.Translate(new float3(x - 15.5f, y - 8.5f, 0)), matrix.Value);
            FreeEntities[i] = matrix;
        }
    }
}
