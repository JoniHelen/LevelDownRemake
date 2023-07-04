using Unity.Entities;
using Unity.Burst;

/// <summary>
/// This system is responsible for updating the flashing floor tiles
/// </summary>
[RequireMatchingQueriesForUpdate]
public partial struct ColorFlashSystem : ISystem
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state) 
        => new GlowUpdateJob { Time = SystemAPI.Time.ElapsedTime }.Schedule();
}