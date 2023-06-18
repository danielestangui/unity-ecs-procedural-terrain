using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using TerrainGenerator;
using Unity.Transforms;

namespace TerrainGenerator
{
    [AddComponentMenu("Terrain Generator/Terrain Generator Authoring")]
    public class TerrainGeneratorAuthoring : MonoBehaviour
    {
        [SerializeField]
        private TerrainGeneratorData data;

        class Baker : Baker<TerrainGeneratorAuthoring>
        {
            static ComponentTypeSet componentsToAdd = new(new ComponentType[]
            {
                typeof(LocalTransform),
                typeof(LocalToWorld),
                typeof(OctreeNodeComponent),
                typeof(ChunkComponent),
                typeof(OctreeLeafComponent)
            });

            public override void Bake(TerrainGeneratorAuthoring authoring)
            {
                // Baking dependencies
                DependsOn(authoring.data);

                if (authoring.data == null)
                    return;
                
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                // Add Components
                AddComponent(entity, componentsToAdd);

                // Blob asset
                BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
                ref ResolutionArray resolutionRoot = ref blobBuilder.ConstructRoot<ResolutionArray>();

                BlobBuilderArray<int> blobArray = blobBuilder.Allocate(ref resolutionRoot.Values, authoring.data.depthResolution.Length);

                for (int i = 0; i < authoring.data.depthResolution.Length; i++)
                {
                    blobArray[i] = authoring.data.depthResolution[i];
                }

                BlobAssetReference<ResolutionArray> blobAsset = blobBuilder.CreateBlobAssetReference<ResolutionArray>(Allocator.Persistent);

                // Set Components
                OctreeNodeComponent octreeNodeComponent = new OctreeNodeComponent
                {
                    parent = Entity.Null,
                    size = authoring.data.size,
                    depth = (authoring.data.depthResolution.Length - 1),
                    resolution = blobAsset
                };

                ChunkComponent chunkComponent = new ChunkComponent
                {
                    resolution = authoring.data.depthResolution[octreeNodeComponent.depth],
                    size = authoring.data.size,
                };

                OctreeLeafComponent octreeLeafComponent = new OctreeLeafComponent
                {
                    value = 0
                };

                Transform transform = GetComponent<Transform>();

                SetComponent(entity, LocalTransform.FromPosition(transform.position));
                SetComponent(entity, octreeNodeComponent);
                SetComponent(entity, chunkComponent);
                SetComponent(entity, octreeLeafComponent);

                // Add Buffers
                AddBuffer<GridVertexElement>(entity);
                AddBuffer<CellElement>(entity);
                AddBuffer<VerticesBuffer>(entity);
                AddBuffer<IntersectingEdgesBuffer>(entity);
                AddBuffer<TrianglesBuffer>(entity);

                // Blobbuilder Dispose
                blobBuilder.Dispose();
            }
        }
    }

    public struct OctreeNodeComponent : IComponentData
    {
        public float size;
        public int depth;
        public Entity parent;
        public BlobAssetReference<ResolutionArray> resolution;
    }

    public struct OctreeBranchComponent : IComponentData
    {
        public Entity child0;
        public Entity child1;
        public Entity child2;
        public Entity child3;
        public Entity child4;
        public Entity child5;
        public Entity child6;
        public Entity child7;
    }
   
    public struct OctreeLeafComponent : IComponentData 
    {
        public float value;
    }

    public struct ResolutionArray 
    {
        public BlobArray<int> Values;
    }
}