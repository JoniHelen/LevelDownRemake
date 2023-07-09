using UnityEngine;
using Unity.Entities;

namespace LevelDown.Components
{
    /// <summary>
    /// Stores data regarding the floor tiles' color flashes
    /// </summary>
    public struct ColorFlash : IComponentData, IEnableableComponent
    {
        public Color FlashColor;
        public Color BaseColor;
        public float Duration;
        public double StartTime;
    }
}