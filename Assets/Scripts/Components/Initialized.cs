using Unity.Entities;

namespace LevelDown.Components
{
    /// <summary>
    /// A tag component that indicates that a floor tile is in place and fully initialized.
    /// </summary>
    public struct Initialized : IComponentData, IEnableableComponent
    {

    }
}