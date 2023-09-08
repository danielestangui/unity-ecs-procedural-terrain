using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator 
{
    [AddComponentMenu("Terrain Generator/Chunk Authoring")]
    public class ChunkAuthoring : MonoBehaviour
    {
        [SerializeField, Range(2, 10)]
        private int resolution;

        [SerializeField, Range(1, 10)]
        private float size;

        class Baker : Baker<ChunkAuthoring>
        {
            public override void Bake(ChunkAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                ChunkComponent chunkComponent = new ChunkComponent
                {
                    resolution = authoring.resolution,
                    lenght = authoring.size,
                };

                AddComponent(entity, chunkComponent);

                AddBuffer<GridVertexElement>(entity);
                AddBuffer<CellElement>(entity);
                AddBuffer<VerticesBuffer>(entity);
                AddBuffer<IntersectingEdgesBuffer>(entity);
                AddBuffer<TrianglesBuffer>(entity);

            }
        }
    }
}