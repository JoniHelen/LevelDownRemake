using Unity.Entities;
using Unity.Physics;

public struct FloorPhysicsBlobs : IComponentData
{
    public BlobAssetReference<Collider> Small;
    public BlobAssetReference<Collider> Tall;
}
