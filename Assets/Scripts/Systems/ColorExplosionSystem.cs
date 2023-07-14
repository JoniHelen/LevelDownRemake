using Unity.Entities;
using Unity.Burst;
using Unity.Physics;
using LevelDown.Components;
using LevelDown.Jobs;

namespace LevelDown.Systems
{
    [RequireMatchingQueriesForUpdate]
    public partial struct ColorExplosionSystem : ISystem
    {
        private ComponentLookup<ColorFlash> _flashLookup;
        private ComponentLookup<RandomValue> _randomLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _flashLookup = state.GetComponentLookup<ColorFlash>();
            _randomLookup = state.GetComponentLookup<RandomValue>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _flashLookup.Update(ref state);
            _randomLookup.Update(ref state);

            new ColorExplosionJob
            {
                CollisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
                FlashLookup = _flashLookup,
                RandomLookup = _randomLookup,
                Time = SystemAPI.Time.ElapsedTime
            }.Schedule();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        
        }
    }
}