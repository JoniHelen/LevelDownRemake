using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using LevelDown.Components.Triggers;
using LevelDown.Jobs;
using LevelDown.Noise;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    /// <summary>
    /// This system is responsible for generating new levels
    /// </summary>
    public partial struct LevelGeneratorSystem : ISystem
    {
        private NativeArray<float4> _noiseArray;
        private float2 _dimensions;
        private Random _random;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _random = new Random(SmallXXHash.Seed((int)(SystemAPI.Time.ElapsedTime * 1000)));
            _dimensions = new(32, 18);
            _noiseArray = new((int)(_dimensions.x * _dimensions.y), Allocator.Persistent);
            state.RequireForUpdate<GenerateLevelTriggerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NativeReference<float2> Offset = new(_random.NextFloat(1000), Allocator.TempJob);

            // Find offset
            JobHandle offsetJob = new NoiseOffsetJob
            {
                Extents = _dimensions,
                Threshold = 0.6f,
                Scale = 4,
                Offset = Offset
            }.Schedule(state.Dependency);

            // Generate noise
            JobHandle noiseJob = new VectorizedNoiseGenerationJob
            {
                ResultNoise = _noiseArray,
                Extents = _dimensions,
                InvHeight = 1 / _dimensions.y,
                Offset = Offset,
                Scale = 4
            }.Schedule(_noiseArray.Length, 4, offsetJob);

            JobHandle dispose = Offset.Dispose(noiseJob);

            // Initialize entities
            state.Dependency = new LevelGenerationJob
            {
                Extents = _dimensions,
                InvHeight = 1 / _dimensions.y,
                TallThreshold = 0.6f,
                BaseColor = UnityEngine.Color.HSVToRGB(_random.NextFloat(), _random.NextFloat(0.5f, 1), 1),
                Noise = _noiseArray.Reinterpret<float>(sizeof(float) * 4),
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(dispose);

            // Finish execution
            SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .RemoveComponent<GenerateLevelTriggerTag>(
                SystemAPI.GetSingletonEntity<TriggerTagSingleton>());
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) => _noiseArray.Dispose();
    }
}