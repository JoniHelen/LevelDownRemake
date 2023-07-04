using Unity.Entities;
using Unity.Burst;
using Unity.Physics;
using Unity.Mathematics;

[WithAll(typeof(Floor)), WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
[BurstCompile(FloatMode = FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
public partial struct PhysicsColliderGenerationJob : IJobEntity
{
    public void Execute(ref PhysicsCollider collider)
    {
        collider.Value = BoxCollider.Create(new BoxGeometry
        {
            BevelRadius = 0.05f,
            Center = 0,
            Size = 1,
            Orientation = quaternion.identity
        });
    }
}
