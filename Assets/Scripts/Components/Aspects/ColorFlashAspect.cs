using UnityEngine;
using Unity.Entities;

namespace LevelDown.Components.Aspects
{
    /// <summary>
    /// Exposes the enabled bit of <see cref="ColorFlash"/>.
    /// </summary>
    public readonly partial struct ColorFlashAspect : IAspect
    {
        private readonly EnabledRefRW<ColorFlash> _enabled;
        private readonly RefRW<ColorFlash> _ref;

        public bool Enabled
        {
            get => _enabled.ValueRO;
            set => _enabled.ValueRW = value;
        }

        public Color FlashColor
        {
            get => _ref.ValueRO.FlashColor;
            set => _ref.ValueRW.FlashColor = value;
        }

        public Color BaseColor
        {
            get => _ref.ValueRO.BaseColor;
            set => _ref.ValueRW.BaseColor = value;
        }

        public float Duration
        {
            get => _ref.ValueRO.Duration;
            set => _ref.ValueRW.Duration = value;
        }

        public double StartTime
        {
            get => _ref.ValueRO.StartTime;
            set => _ref.ValueRW.StartTime = value;
        }
    }
}