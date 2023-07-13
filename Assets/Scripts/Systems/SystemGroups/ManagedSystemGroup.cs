using Unity.Entities;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class ManagedSystemGroup : ComponentSystemGroup
    {

    }
}