using Unity.Entities;
using Unity.Physics;

namespace LevelDown.Components.Singletons
{
    public struct FloorPhysicsBlobs : IComponentData
    {
        public BlobAssetReference<Collider> Small;
        public BlobAssetReference<Collider> Tall;
    }
}