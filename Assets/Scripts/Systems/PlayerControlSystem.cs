using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using LevelDown.Jobs;
using LevelDown.Components.Singletons;
using LevelDown.Components.Tags;

namespace LevelDown.Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(PlayerInputSystem))]
    public partial struct PlayerControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) => state.RequireForUpdate<PlayerInputData>();

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var aimDir = new NativeReference<float2>(Allocator.TempJob);

            JobHandle control = new PlayerControlJob { Aim = aimDir,
                GunPos = SystemAPI.GetComponent<LocalToWorld>(SystemAPI.GetSingletonEntity<Gun>()).Position.xy,
                ProjectileWriter = SystemAPI.GetSingleton<ProjectileQueue>().Projectiles
                }.Schedule(state.Dependency);
            JobHandle rotation = new GunRotationJob { Input = aimDir }.Schedule(control);
            state.Dependency = aimDir.Dispose(rotation);
        }
    }
}