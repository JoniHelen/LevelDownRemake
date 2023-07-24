using Unity.Entities;
using Unity.Burst;
using Unity.Physics;
using Unity.Collections;
using LevelDown.Components;
using LevelDown.Components.Tags;
using System.Linq;

namespace LevelDown.Jobs
{
    [BurstCompile]
    public partial struct ColorFlashTriggersJob : ITriggerEventsJob
    {
        public BufferLookup<EntityBufferData> BufferLookup;
        public ComponentLookup<ColorExplosion> ExplosionLookup;
        public ComponentLookup<LevelDestroyer> LevelDestroyerLookup;
        public NativeList<Entity> FlashingEntities;

        public void Execute(TriggerEvent triggerEvent)
        {
            CheckLookup(triggerEvent, ExplosionLookup);
            CheckLookup(triggerEvent, LevelDestroyerLookup);
        }

        private void CheckLookup<C>(TriggerEvent triggerEvent, ComponentLookup<C> lookup) where C : unmanaged, IComponentData
        {
            if (!lookup.HasComponent(triggerEvent.EntityB)) return;

            var triggerer = triggerEvent.EntityB;
            var floor = triggerEvent.EntityA;
            var buffer = BufferLookup[triggerer];

            for (var i = 0; i < buffer.Length; i++)
                if (buffer[i] == floor) return;

            buffer.Add(floor);

            if (!FlashingEntities.Contains(floor))
                FlashingEntities.Add(floor);
        }
    }
}