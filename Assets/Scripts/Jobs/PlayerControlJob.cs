using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using LevelDown.Components.Aspects;
using LevelDown.Input;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Updates the player's contols.
    /// </summary>
    [BurstCompile]
    public partial struct PlayerControlJob : IJobEntity
    {
        [WriteOnly] public NativeReference<float2> Aim;
        [WriteOnly] public NativeList<ProjectileDescriptor> ProjectileWriter;
        public float2 GunPos;

        public void Execute(PlayerControlAspect controls)
        {
            controls.Update();
            Aim.Value = controls.AimDirection;
            if (controls.FireButton.WasPressedThisFrame)
                ProjectileWriter.Add(new ProjectileDescriptor
                {
                    Position = GunPos,
                    Direction = controls.AimDirection
                });
        }
    }
}