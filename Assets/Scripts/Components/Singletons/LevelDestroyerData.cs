using Unity.Entities;

public struct LevelDestroyerData : IComponentData
{
    public double StartTime;
    public float Duration;
    public float TargetRadius;
    public bool Finished;
}
