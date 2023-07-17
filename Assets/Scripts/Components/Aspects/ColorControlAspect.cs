using UnityEngine;
using Unity.Entities;

namespace LevelDown.Components.Aspects
{
    /// <summary>
    /// This aspect contains all components needed to control the floor tiles' colors.
    /// </summary>
    public readonly partial struct ColorControlAspect : IAspect
    {
        private readonly RefRW<GlowColor> _color;
        private readonly RefRW<GlowBrightness> _brightness;
        private readonly EnabledRefRW<ColorFlash> _enabled;
        private readonly RefRW<ColorFlash> _flash;
        private readonly RefRW<Floor> _floor;

        public Color Color
        {
            get => _color.ValueRO.Color;
            set => _color.ValueRW.Color = value;
        }

        public bool Enabled
        {
            get => _enabled.ValueRO;
            set => _enabled.ValueRW = value;
        }

        public float FlashBrightness
        {
            get => _flash.ValueRO.FlashBrightness;
            set => _flash.ValueRW.FlashBrightness = value;
        }

        public Color FlashColor
        {
            get => _flash.ValueRO.FlashColor;
            set => _flash.ValueRW.FlashColor = value;
        }
        public Color BaseColor
        {
            get => _flash.ValueRO.BaseColor;
            set => _flash.ValueRW.BaseColor = value;
        }

        public float Duration
        {
            get => _flash.ValueRO.Duration;
            set => _flash.ValueRW.Duration = value;
        }

        public float Brightness
        {
            get => _brightness.ValueRO.Value;
            set => _brightness.ValueRW.Value = value;
        }

        public double StartTime
        {
            get => _flash.ValueRO.StartTime;
            set => _flash.ValueRW.StartTime = value;
        }

        public bool Tall
        {
            get => _floor.ValueRO.Tall;
            set => _floor.ValueRW.Tall = value;
        }
    }
}