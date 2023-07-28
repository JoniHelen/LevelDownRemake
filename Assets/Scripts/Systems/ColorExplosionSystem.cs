using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using LevelDown.Components;
using EndSimulation =
    Unity.Entities.EndSimulationEntityCommandBufferSystem.Singleton;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct ColorExplosionSystem : ISystem
    {
        private ComponentLookup<ColorFlash> _flashLookup;
        private ComponentLookup<RandomValue> _randomValueLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _flashLookup = state.GetComponentLookup<ColorFlash>();
            _randomValueLookup = state.GetComponentLookup<RandomValue>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _flashLookup.Update(ref state);
            _randomValueLookup.Update(ref state);

            new ColorExplosionJob
            {
                Ecb = SystemAPI.GetSingleton<EndSimulation>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                FlashLookup = _flashLookup,
                RandomLookup = _randomValueLookup,
                Time = SystemAPI.Time.ElapsedTime,
                QueryWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld
            }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}