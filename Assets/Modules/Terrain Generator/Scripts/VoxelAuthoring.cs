using UnityEngine;
using Unity.Entities;

namespace MarchingCube.Test
{
    [AddComponentMenu("MarchingCube/VoxelAuthoring")]
    public class VoxelAuthoring : MonoBehaviour
    {
        public uint resolution = 8;
        public float frequency = 8;

        [Range(0, 1)]
        public float threshold = 0.5f;

        public class Baker : Baker<VoxelAuthoring>
        {
            public override void Bake(VoxelAuthoring authoring)
            {
                var data = new Voxel
                {
                    resolution = authoring.resolution,
                    frequency = authoring.frequency,
                    threshold = authoring.threshold
                };
                AddComponent(data);
            }
        }
    }

    struct Voxel : IComponentData
    {
        public uint resolution;
        public float frequency;
        public float threshold;
    }
}