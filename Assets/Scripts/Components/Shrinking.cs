using Unity.Entities;

public struct Shrinking : IComponentData
{
    public double StartTime;
    public float Duration;
    public bool Finished;
}
