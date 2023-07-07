using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using LevelDown.Noise;
using LevelDown.Noise.Vectorized;
using LevelDown.Components.Singletons;

namespace LevelDown.Jobs
{
    [WithAll(typeof(PlayerInputData))]
    //[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public partial struct NoiseOffsetJob : IJobEntity
    {
        public NativeReference<float2> Offset;
        public float2 Extents;
        public float Threshold;
        public float Scale;

        public void Execute(ref LocalTransform local)
        {
            float2 offset = 0;
            float2 playerPosition = local.Position.xy + new float2(Extents.x / 2 - 0.5f, Extents.y / 2 - 0.5f);
            var texelSize = 1f / Extents;

            for (var i = 0; i < 100; i++)
            {
                if (i == 99) UnityEngine.Debug.Log($"OFFSET FAILED WITH OFFSET: {offset}, POSITION: {playerPosition}, TEXELSIZE: {texelSize}");

                if (PlayerOverlapsLevel(playerPosition, offset, texelSize))
                    offset += texelSize;
                else
                    break;
            }

            Offset.Value = offset;
        }

        private bool PlayerOverlapsLevel(float2 position, float2 offset, float2 texelSize)
        {
            var playerTexel = math.floor(position) * texelSize + offset;

            if (Simplex.GetNoise(playerTexel, Scale) >= Threshold) return true;

            var u1 = new float4
            {
                x = playerTexel.x - texelSize.x,
                y = playerTexel.x - texelSize.x,
                z = playerTexel.x,
                w = playerTexel.x + texelSize.x
            };

            var u2 = new float4
            {
                x = playerTexel.x + texelSize.x,
                y = playerTexel.x + texelSize.x,
                z = playerTexel.x,
                w = playerTexel.x - texelSize.x
            };

            var v1 = new float4
            {
                x = playerTexel.y,
                y = playerTexel.y + texelSize.y,
                z = playerTexel.y + texelSize.y,
                w = playerTexel.y + texelSize.y
            };

            var v2 = new float4
            {
                x = playerTexel.y,
                y = playerTexel.y - texelSize.y,
                z = playerTexel.y - texelSize.y,
                w = playerTexel.y - texelSize.y
            };

            NativeArray<float4> values = new(2, Allocator.Temp);
            values[0] = Simplex4.GetNoise4(new float4x2(u1, v1), Scale);
            values[1] = Simplex4.GetNoise4(new float4x2(u2, v2), Scale);

            foreach (var value in values.Reinterpret<float>(sizeof(float) * 4))
                if (value >= Threshold)
                {
                    values.Dispose();
                    return true;
                }

            values.Dispose();
            return false;
        }
    }
}