using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

/// <summary>
/// This system is responsible for generating new levels
/// </summary>
public partial struct LevelGeneratorSystem : ISystem
{
    private NativeArray<float4> _noiseArray;
    private float2 _dimensions;

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnCreate(ref SystemState state)
    {
        _dimensions = new(32, 18);
        _noiseArray = new((int)(_dimensions.x * _dimensions.y), Allocator.Persistent);
        state.RequireForUpdate<GenerateLevelTriggerTag>();
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state)
    {
        JobHandle noiseJob = new VectorizedNoiseGenerationJob {
            ResultNoise = _noiseArray,
            Extents = _dimensions,
            InvHeight = 1 / _dimensions.y,
            Offset = (float)SystemAPI.Time.ElapsedTime,
            Scale = 4
        }.Schedule(_noiseArray.Length, 4, state.Dependency);

        state.Dependency = new LevelGenerationJob {
            Extents = _dimensions,
            InvHeight = 1 / _dimensions.y,
            TallThreshold = 0.6f,
            PhysicsBlobs = SystemAPI.GetSingleton<FloorPhysicsBlobs>(),
            Noise = _noiseArray.Reinterpret<float>(16),
            Ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(noiseJob);

        SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).RemoveComponent<GenerateLevelTriggerTag>(
            SystemAPI.GetSingletonEntity<TriggerTagSingleton>());
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnDestroy(ref SystemState state) => _noiseArray.Dispose();
}