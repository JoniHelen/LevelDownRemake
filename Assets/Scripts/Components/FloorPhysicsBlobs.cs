using Unity.Entities;
using Unity.Physics;

namespace LevelDown.Components
{
    /// <summary>
    /// Stores floor tiles' physics collider data.
    /// </summary>
    public struct FloorPhysicsBlobs : ISharedComponentData
    {
        public BlobAssetReference<Collider> Small;
        public BlobAssetReference<Collider> Tall;
    }
}