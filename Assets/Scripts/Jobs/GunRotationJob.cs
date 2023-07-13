using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using LevelDown.Components.Tags;

namespace LevelDown.Jobs
{
    /// <summary>
    /// Updates the player's gun.
    /// </summary>
    [BurstCompile, WithAll(typeof(GunArm))]
    public partial struct GunRotationJob : IJobEntity
    {
        [ReadOnly] public NativeReference<float2> Input;

        public void Execute(ref LocalTransform transform)
        {
            transform.Rotation = quaternion.LookRotation(new float3(0, 0, 1), new float3(Input.Value, 0));
        }
    }
}