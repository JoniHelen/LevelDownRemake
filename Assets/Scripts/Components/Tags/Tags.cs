using Unity.Entities;

namespace LevelDown.Components.Tags
{
    /// <summary>
    /// A tag component that indicates that a floor tile is in place and fully initialized.
    /// </summary>
    public struct Initialized : IComponentData, IEnableableComponent { }

    /// <summary>
    /// A tag component for the player's gun arm.
    /// </summary>
    public struct GunArm : IComponentData { }

    /// <summary>
    /// A tag component for the player's gun.
    /// </summary>
    public struct Gun : IComponentData { }
}