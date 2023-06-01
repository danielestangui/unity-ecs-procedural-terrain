using Unity.Entities;
using UnityEngine;

namespace TerrainGenerator
{
    [AddComponentMenu("Procedural Terrain/ Procedural Terrain Authoring")]
    public class ProceduralTerrainAuthoring : MonoBehaviour
    { 
        [SerializeField]
        private float size;
        [SerializeField]
        private int maxDepth;

        public class Baker : Baker<ProceduralTerrainAuthoring>
        {
            public override void Bake(ProceduralTerrainAuthoring authoring)
            {
                ProceduralTerrainComponent worldTerrainComponent = new ProceduralTerrainComponent
                {
                    size = authoring.size,
                    maxDepth = authoring.maxDepth
                };

                AddComponent(worldTerrainComponent);

                AddBuffer<OctreeNodeBufferElement>();
            }
        }
    }

    struct ProceduralTerrainComponent : IComponentData 
    {
        public float size;
        public float maxDepth;
    }

    [InternalBufferCapacity(8)]
    public struct OctreeNodeBufferElement : IBufferElementData
    {
        public OctreeNode octreeNode;
    }

    public struct OctreeNode
    {
        public int currentDepth;

    }


}