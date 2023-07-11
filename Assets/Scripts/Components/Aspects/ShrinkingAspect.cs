using Unity.Entities;

namespace LevelDown.Components.Aspects
{
    /// <summary>
    /// Exposes the enabled bit of <see cref="Shrinking"/>.
    /// </summary>
    public readonly partial struct ShrinkingAspect : IAspect
    {
        private readonly EnabledRefRW<Shrinking> _enabled;
        private readonly RefRW<Shrinking> _ref;

        public bool Enabled
        {
            get => _enabled.ValueRO;
            set => _enabled.ValueRW = value;
        }

        public double StartTime
        {
            get => _ref.ValueRO.StartTime;
            set => _ref.ValueRW.StartTime = value;
        }

        public float Duration
        {
            get => _ref.ValueRO.Duration;
            set => _ref.ValueRW.Duration = value;
        }
    }
}