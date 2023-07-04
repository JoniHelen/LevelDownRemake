using Unity.Entities;

public struct Shrinking : IComponentData, IEnableableComponent
{
    public double StartTime;
    public float Duration;
}
