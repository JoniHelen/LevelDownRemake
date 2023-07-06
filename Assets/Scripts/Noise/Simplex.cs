using Unity.Mathematics;

namespace LevelDown.Noise
{
	/// <summary>
	/// An implementation of fast 2D Simplex noise.
	/// </summary>
	public struct Simplex
	{
		public static float GetNoise(float x, float y, SmallXXHash hash, float scale)
		{
			x *= scale * (1f / math.sqrt(3f));
			y *= scale * (1f / math.sqrt(3f));

			float skew = (x + y) * ((math.sqrt(3f) - 1f) / 2f);
			float sx = x + skew, sz = y + skew;

			int x0 = (int)math.floor(sx), x1 = x0 + 1, z0 = (int)math.floor(sz), z1 = z0 + 1;

			bool xGz = sx - x0 > sz - z0;
			int xC = math.select(x0, x1, xGz), zC = math.select(z1, z0, xGz);

			SmallXXHash h0 = hash.Eat(x0), h1 = hash.Eat(x1), hC = SmallXXHash.Select(h0, h1, xGz);

			return Kernel(h0.Eat(z0), x0, z0, x, y) +
					Kernel(h1.Eat(z1), x1, z1, x, y) +
					Kernel(hC.Eat(zC), xC, zC, x, y);
		}

		private static float2 SquareVector(SmallXXHash hash)
		{
			float2 v;
			v.x = hash.Float01A * 2f - 1f;
			v.y = 0.5f - math.abs(v.x);
			v.x -= math.floor(v.x + 0.5f);
			return v;
		}

		private static float Circle(SmallXXHash hash, float x, float y)
		{
			float2 v = SquareVector(hash);
			return (v.x * x + v.y * y) * math.rsqrt(v.x * v.x + v.y * v.y);
		}

		private static float Kernel(SmallXXHash hash, float lx, float lz, float px, float py)
		{
			float unskew = (lx + lz) * ((3f - math.sqrt(3f)) / 6f);
			float x = px - lx + unskew, z = py - lz + unskew;
			float f = 0.5f - x * x - z * z;
			f = f * f * f * 8f;
			return math.max(0f, f) * (Circle(hash, x, z) * (5.832f / math.sqrt(2f)));
		}
	}
}