using Unity.Entities;

namespace LevelDown.Components.Triggers
{
    // These tag components are used to trigger certain systems that need to run only in certain situations.
    public struct TriggerTagSingleton : IComponentData { }
    public struct GenerateLevelTriggerTag : IComponentData { }
    public struct DestroyLevelTriggerTag : IComponentData { }
}