using Unity.Mathematics;
using LevelDown.Noise.Vectorized;

namespace LevelDown.Noise
{
	public readonly struct SmallXXHash
	{
		const uint primeA = 0b10011110001101110111100110110001;
		const uint primeB = 0b10000101111010111100101001110111;
		const uint primeC = 0b11000010101100101010111000111101;
		const uint primeD = 0b00100111110101001110101100101111;
		const uint primeE = 0b00010110010101100110011110110001;

		readonly uint accumulator;

		public uint ByteA => this & 255;

		public float Float01A => ByteA * (1f / 255f);

		public static implicit operator uint(SmallXXHash hash)
		{
			uint avalanche = hash.accumulator;
			avalanche ^= avalanche >> 15;
			avalanche *= primeB;
			avalanche ^= avalanche >> 13;
			avalanche *= primeC;
			avalanche ^= avalanche >> 16;
			return avalanche;
		}

		public static implicit operator SmallXXHash(uint accumulator) =>
			new(accumulator);

		public static implicit operator SmallXXHash4(SmallXXHash hash) =>
			new(hash.accumulator);

		public SmallXXHash(uint accumulator) { this.accumulator = accumulator; }

		public static SmallXXHash Seed(int seed) => (uint)seed + primeE;

		public SmallXXHash Eat(int data) =>
			RotateLeft(accumulator + (uint)data * primeC, 17) * primeD;

		public SmallXXHash Eat(byte data) =>
			RotateLeft(accumulator + data * primeE, 11) * primeA;

		public static SmallXXHash Select(SmallXXHash a, SmallXXHash b, bool c) =>
			math.select(a.accumulator, b.accumulator, c);

		private static uint RotateLeft(uint data, int steps) =>
			(data << steps) | (data >> 32 - steps);
	}
}