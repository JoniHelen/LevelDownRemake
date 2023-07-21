using Unity.Entities;
using LevelDown.Input;

namespace LevelDown.Components
{
    public struct Projectile : IComponentData
    {
        public ProjectileData Data;
        public float Damage;
        public float Speed;
        public float CritChance;
        public float GlitchChance;
    }
}