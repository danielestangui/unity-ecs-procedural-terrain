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

                // Set Components
                OctreeNodeComponent octreeNodeComponent = new OctreeNodeComponent
                {
                    parent = Entity.Null,
                    lenght = authoring.data.octreeLenght * (1 << (authoring.data.depthResolution - 1)),
                    depth = authoring.data.depthResolution,
                    resolution = authoring.data.chunkResolution
                };

                ChunkComponent chunkComponent = new ChunkComponent
                {
                    lenght = authoring.data.octreeLenght * (1 << (authoring.data.depthResolution - 1)),
                    resolution = authoring.data.chunkResolution
                };

                OctreeLeafComponent octreeLeafComponent = new OctreeLeafComponent
                {
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
            }
        }
    }
}