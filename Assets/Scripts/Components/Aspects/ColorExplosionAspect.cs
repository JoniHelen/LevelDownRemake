using Unity.Entities;

namespace LevelDown.Components.Aspects
{
    public readonly partial struct ColorExplosionAspect : IAspect
    {
        private readonly RefRW<ColorExplosion> _explosion;
        private readonly EnabledRefRW<ColorExplosion> _enabled;
        private readonly DynamicBuffer<EntityBufferData> _buffer;

        public bool Enabled
        {
            get => _enabled.ValueRO;
            set => _enabled.ValueRW = value;
        }

        public DynamicBuffer<EntityBufferData> Buffer
        {
            get => _buffer;
        }

        public double StartTime
        {
            get => _explosion.ValueRO.StartTime;
            set => _explosion.ValueRW.StartTime = value;
        }

        public float Duration
        {
            get => _explosion.ValueRO.Duration;
            set => _explosion.ValueRW.Duration = value;
        }

        public float TargetSize
        {
            get => _explosion.ValueRO.TargetSize;
            set => _explosion.ValueRW.TargetSize = value;
        }
    }
}