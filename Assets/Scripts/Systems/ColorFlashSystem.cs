using Unity.Entities;
using Unity.Burst;
using LevelDown.Jobs;
using LevelDown.Components;

namespace LevelDown.Systems
{
    /// <summary>
    /// This system is responsible for updating the flashing floor tiles.
    /// </summary>
    [RequireMatchingQueriesForUpdate]
    public partial struct ColorFlashSystem : ISystem
    {
        private ComponentLookup<ColorFlash> _flashLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _flashLookup = state.GetComponentLookup<ColorFlash>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _flashLookup.Update(ref state);

            new GlowUpdateJob { 
                Time = SystemAPI.Time.ElapsedTime,
                FlashLookup = _flashLookup
            }.ScheduleParallel();
        }
    }
}