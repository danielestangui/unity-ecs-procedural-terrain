using UnityEngine;
using Unity.Entities;

namespace MarchingCube.Test
{
    [AddComponentMenu("MarchingCube/VoxelAuthoring")]
    public class VoxelAuthoring : MonoBehaviour
    {
        public uint resolution = 8;

        public class Baker : Baker<VoxelAuthoring>
        {
            public override void Bake(VoxelAuthoring authoring)
            {
                var data = new Voxel
                {
                    resolution = authoring.resolution
                };
                AddComponent(data);
            }
        }
    }

    struct Voxel : IComponentData
    {
        public uint resolution;
    }
}