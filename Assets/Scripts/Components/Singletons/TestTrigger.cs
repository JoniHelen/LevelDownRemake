using Unity.Entities;

public struct TestTrigger : IComponentData
{
    public double Interval;
    public double GenerateTime;
    public double DestroyTime;
}
