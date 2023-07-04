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
    private NativeArray<float4> NoiseArray;
    private float2 Dimensions;

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnCreate(ref SystemState state)
    {
        Dimensions = new(32, 18);
        NoiseArray = new((int)(Dimensions.x * Dimensions.y), Allocator.Persistent);
        state.RequireForUpdate<GenerateLevelTriggerTag>();
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnUpdate(ref SystemState state)
    {
        JobHandle noiseJob = new VectorizedNoiseGenerationJob {
            ResultNoise = NoiseArray,
            Extents = Dimensions,
            invHeight = 1 / Dimensions.y,
            Offset = (float)SystemAPI.Time.ElapsedTime,
            Scale = 4
        }.Schedule(NoiseArray.Length, 4, state.Dependency);

        state.Dependency = new LevelGenerationJob {
            Extents = Dimensions,
            invHeight = 1 / Dimensions.y,
            tallThreshold = 0.6f,
            Noise = NoiseArray.Reinterpret<float>(16),
            ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .ValueRW.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel(noiseJob);

        SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .ValueRW.CreateCommandBuffer(state.WorldUnmanaged).RemoveComponent<GenerateLevelTriggerTag>(
            SystemAPI.GetSingletonEntity<TriggerTagSingleton>());
    }

    [BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public void OnDestroy(ref SystemState state)
    {
        NoiseArray.Dispose();
    }
}