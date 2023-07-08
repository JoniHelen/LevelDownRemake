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
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public partial struct NoiseOffsetJob : IJobEntity
    {
        public NativeReference<float2> Offset;
        public float2 Extents;
        public float Threshold;
        public float Scale;

        public void Execute(ref LocalTransform local)
        {
            float2 offset = 0;
            var playerPosition = local.Position.xy + new float2(Extents.x / 2 - 0.5f, Extents.y / 2 - 0.5f);
            var texelSize = 1f / Extents;
            var normalizedTexelSize = texelSize.x > texelSize.y ? texelSize.x : texelSize.y;

            for (var i = 0; i < 100; i++)
            {
                if (PlayerOverlapsLevel(playerPosition, offset, normalizedTexelSize))
                    offset += normalizedTexelSize;
                else
                    break;
            }

            Offset.Value = offset;
        }

        private bool PlayerOverlapsLevel(float2 position, float2 offset, float normalizedTexelSize)
        {
            var playerTexel = math.floor(position) * normalizedTexelSize + offset;

            if ((Simplex.GetNoise(playerTexel, Scale) + 1) / 2 >= Threshold) return true;

            var u1 = new float4
            {
                x = playerTexel.x - normalizedTexelSize,
                y = playerTexel.x - normalizedTexelSize,
                z = playerTexel.x,
                w = playerTexel.x + normalizedTexelSize
            };

            var u2 = new float4
            {
                x = playerTexel.x + normalizedTexelSize,
                y = playerTexel.x + normalizedTexelSize,
                z = playerTexel.x,
                w = playerTexel.x - normalizedTexelSize
            };

            var v1 = new float4
            {
                x = playerTexel.y,
                y = playerTexel.y + normalizedTexelSize,
                z = playerTexel.y + normalizedTexelSize,
                w = playerTexel.y + normalizedTexelSize
            };

            var v2 = new float4
            {
                x = playerTexel.y,
                y = playerTexel.y - normalizedTexelSize,
                z = playerTexel.y - normalizedTexelSize,
                w = playerTexel.y - normalizedTexelSize
            };

            var values1 = (Simplex4.GetNoise4(new float4x2(u1, v1), Scale) + 1) / 2;
            var values2 = (Simplex4.GetNoise4(new float4x2(u2, v2), Scale) + 1) / 2;

            for (var i = 0; i < 4; i++)
                if (values1[i] >= Threshold || values2[i] >= Threshold) return true;

            return false;
        }
    }
}