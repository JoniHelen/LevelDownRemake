using Unity.Entities;
using Unity.Physics;

namespace LevelDown.Components.Singletons
{
    /// <summary>
    /// Stores floor tiles' physics collider data.
    /// </summary>
    public struct FloorPhysicsBlobs : IComponentData
    {
        public BlobAssetReference<Collider> Small;
        public BlobAssetReference<Collider> Tall;
    }
}