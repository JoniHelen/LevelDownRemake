using Unity.Entities;
using Unity.Burst;

/// <summary>
/// This system updates and resets floor entities when switching levels
/// </summary>
[RequireMatchingQueriesForUpdate]
public partial struct FloorShrinkingSystem : ISystem
{
    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state) => new FloorResetJob {
        Time = SystemAPI.Time.ElapsedTime,
        PhysicsBlobs = SystemAPI.GetSingleton<FloorPhysicsBlobs>(),
        Ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
    }.ScheduleParallel();
}