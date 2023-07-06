using Unity.Mathematics;

namespace LevelDown.Noise.Vectorized
{
	/// <summary>
	/// A vectorized implementation of fast 2D Simplex noise.
	/// </summary>
	public struct Simplex4
	{
		public static float4 GetNoise4(float4x2 positions, SmallXXHash4 hash, float scale)
		{
			positions *= scale * (1f / math.sqrt(3f));

			float4 skew = (positions.c0 + positions.c1) * ((math.sqrt(3f) - 1f) / 2f);
			float4 sx = positions.c0 + skew, sy = positions.c1 + skew;

			int4 x0 = (int4)math.floor(sx), x1 = x0 + 1, y0 = (int4)math.floor(sy), y1 = y0 + 1;

			bool4 xGz = sx - x0 > sy - y0;
			int4 xC = math.select(x0, x1, xGz), yC = math.select(y1, y0, xGz);

			SmallXXHash4 h0 = hash.Eat(x0), h1 = hash.Eat(x1), hC = SmallXXHash4.Select(h0, h1, xGz);

			return Kernel(h0.Eat(y0), x0, y0, positions) +
				Kernel(h1.Eat(y1), x1, y1, positions) +
				Kernel(hC.Eat(yC), xC, yC, positions);
		}

		public static float4 GetNoise4(float4x2 positions, float scale)
		{
			SmallXXHash4 hash = SmallXXHash4.Seed(756284);
			return GetNoise4(positions, hash, scale);
		}

		private static float4x2 SquareVectors(SmallXXHash4 hash)
		{
			float4x2 v;
			v.c0 = hash.Floats01A * 2f - 1f;
			v.c1 = 0.5f - math.abs(v.c0);
			v.c0 -= math.floor(v.c0 + 0.5f);
			return v;
		}

		private static float4 Circle(SmallXXHash4 hash, float4 x, float4 y)
		{
			float4x2 v = SquareVectors(hash);
			return (v.c0 * x + v.c1 * y) * math.rsqrt(v.c0 * v.c0 + v.c1 * v.c1);
		}

		private static float4 Kernel(SmallXXHash4 hash, float4 lx, float4 lz, float4x2 positions)
		{
			float4 unskew = (lx + lz) * ((3f - math.sqrt(3f)) / 6f);
			float4 x = positions.c0 - lx + unskew, y = positions.c1 - lz + unskew;
			float4 f = 0.5f - x * x - y * y;
			f = f * f * f * 8f;
			return math.max(0f, f) * (Circle(hash, x, y) * (5.832f / math.sqrt(2f)));
		}
	}
}