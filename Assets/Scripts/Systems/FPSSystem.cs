using Unity.Entities;
using Unity.Collections;
using LevelDown.Components.Managed;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(ManagedSystemGroup))]
    public partial struct FPSSystem : ISystem
    {
        private NativeQueue<float> _fpsQueue;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TextField>();
            _fpsQueue = new(Allocator.Persistent);
        }

        public void OnUpdate(ref SystemState state)
        {
            if (_fpsQueue.Count < 100)
                _fpsQueue.Enqueue(1 / SystemAPI.Time.DeltaTime);
            else
            {
                _fpsQueue.Dequeue();
                _fpsQueue.Enqueue(1 / SystemAPI.Time.DeltaTime);
            }

            SystemAPI.ManagedAPI.GetSingleton<TextField>().Value.text =
                AverageFPS().ToString("FPS: 0");
        }

        public void OnDestory(ref SystemState state)
        {
            _fpsQueue.Dispose();
        }

        private float AverageFPS()
        {
            float fps = 0;
            
            foreach (var f in _fpsQueue.ToArray(Allocator.Temp))
                fps += f;

            return fps / _fpsQueue.Count;
        }
    }
}