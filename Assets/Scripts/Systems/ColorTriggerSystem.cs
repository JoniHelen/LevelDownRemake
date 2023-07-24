using Unity.Entities;
using Unity.Burst;
using Unity.Physics;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Jobs;
using LevelDown.Jobs;
using LevelDown.Components;
using LevelDown.Components.Tags;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct ColorTriggerSystem : ISystem
    {
        private ComponentLookup<RandomValue> _randomLookup;
        private ComponentLookup<ColorFlash> _flashLookup;
        private ComponentLookup<ColorExplosion> _explosionLookup;
        private ComponentLookup<LevelDestroyer> _levelDestroyerLookup;
        private BufferLookup<EntityBufferData> _bufferLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _randomLookup = state.GetComponentLookup<RandomValue>();
            _flashLookup = state.GetComponentLookup<ColorFlash>();
            _explosionLookup = state.GetComponentLookup<ColorExplosion>();
            _levelDestroyerLookup = state.GetComponentLookup<LevelDestroyer>();
            _bufferLookup = state.GetBufferLookup<EntityBufferData>();

            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.CompleteDependency();

            _randomLookup.Update(ref state);
            _flashLookup.Update(ref state);
            _explosionLookup.Update(ref state);
            _levelDestroyerLookup.Update(ref state);
            _bufferLookup.Update(ref state);

            var flashes = new NativeList<Entity>(200, Allocator.TempJob);

            JobHandle triggers = new ColorFlashTriggersJob
            {
                BufferLookup = _bufferLookup,
                ExplosionLookup = _explosionLookup,
                LevelDestroyerLookup = _levelDestroyerLookup,
                FlashingEntities = flashes
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

            triggers.Complete();

            JobHandle flashJob = new ColorFlashJob
            {
                Entities = flashes,
                FlashLookup = _flashLookup,
                RandomLookup = _randomLookup,
                Time = SystemAPI.Time.ElapsedTime
            }.Schedule(flashes.Length, 8, triggers);

            state.Dependency = flashes.Dispose(flashJob);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}