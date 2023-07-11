using System;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

namespace LevelDown.Systems
{
    /// <summary>
    /// Stores data about a level in a bitmap.
    /// </summary>
    public struct LevelBitMap : INativeDisposable, IDisposable
    {
        public NativeArray<uint> Data;

        public LevelBitMap(params uint[] data)
        {
            Data = new(18, Allocator.Persistent);

            if (data != null)
                NativeArray<uint>.Copy(data, Data, data.Length);
        }

        public uint this[int i]
        {
            get => Data[i];
            set => Data[i] = value;
        }

        public bool this[int x, int y]
        {
            get => IsSet(x, y);
            set => SetBit(x, y, value);
        }

        public void SetBit(int2 pos, bool value)
            => SetBit(pos.x, pos.y, value);

        public void SetBit(int x, int y, bool value)
            => Data[y] = value ? (Data[y] | (1u << x)) : (Data[y] & ~(1u << x));

        public bool IsSet(int2 pos) => IsSet(pos.x, pos.y);

        public bool IsSet(int x, int y) => (Data[y] & (1u << x)) != 0u;

        public void Dispose() => Data.Dispose();

        public JobHandle Dispose(JobHandle handle) => Data.Dispose(handle);
    }
}