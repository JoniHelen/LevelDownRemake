using Unity.Entities;
using Unity.Collections;
using LevelDown.Input;

namespace LevelDown.Components.Singletons
{
    /// <summary>
    /// Used for initializing projectiles during runtime.
    /// </summary>
    public struct ProjectileQueue : IComponentData
    {
        public NativeQueue<ProjectileDescriptor> Projectiles;
    }
}