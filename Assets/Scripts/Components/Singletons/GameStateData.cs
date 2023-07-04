using Unity.Entities;

public struct GameStateData : IComponentData
{
    public bool RequireNewLevel;
    public bool RequireLevelDestruction;
    public double DestroyTime;
}
