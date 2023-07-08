using UnityEngine;
using Unity.Entities;

namespace LevelDown.Components
{
    public struct ColorFlash : IComponentData, IEnableableComponent
    {
        public Color FlashColor;
        public Color BaseColor;
        public float Duration;
        public double StartTime;
    }
}