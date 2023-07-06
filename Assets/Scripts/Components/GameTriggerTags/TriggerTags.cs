using Unity.Entities;

namespace LevelDown.Components.Triggers
{
    public struct TriggerTagSingleton : IComponentData { }
    public struct GenerateLevelTriggerTag : IComponentData { }
    public struct DestroyLevelTriggerTag : IComponentData { }
}