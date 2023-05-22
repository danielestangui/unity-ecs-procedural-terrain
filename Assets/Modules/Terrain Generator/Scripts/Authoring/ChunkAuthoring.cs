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

        [Header("Noise Settings")]
        [SerializeField]
        private float frequency;

        [SerializeField, Range(0, 1)]
        private float threshold;

        public class Baker : Baker<ChunkAuthoring>
        {
            public override void Bake(ChunkAuthoring authoring)
            {
                int vertexResolution = authoring.resolution * authoring.resolution * authoring.resolution;
                int cellResolution = (authoring.resolution - 1) * (authoring.resolution - 1) * (authoring.resolution - 1);


                var data = new ChunkComponent
                {
                    resolution = authoring.resolution,
                    size = authoring.size,
                    gridVertexNativeArray = new NativeArray<GridVertex>(vertexResolution, Allocator.Persistent),
                    cellNativeArray = new NativeArray<Cell>(cellResolution, Allocator.Persistent),
                    //frequency = authoring.threshold,
                    //threshold = authoring.frequency
                };
                AddComponent(data);

                AddBuffer<VerticesBuffer>();
                AddBuffer<IntersectingEdgesBuffer>();
                AddBuffer<TrianglesBuffer>();
            }
        }
    }

    struct ChunkComponent : IComponentData
    {
        public int resolution;
        public float size;
        public NativeArray<GridVertex> gridVertexNativeArray;
        public NativeArray<Cell> cellNativeArray;

        //public float frequency;
        //public float threshold;
    }

    public struct GridVertex
    {
        public float3 position;
        public float value;
    }

    public struct Cell
    {
        /*

                6---------7
               /|        /|
              / |       / |
             /  |      /  |
            2---------3   |
            |   4-----|---5
            |  /      |  /
            | /       | /
            |/        |/
            0---------1

        */
        public int corner0;
        public int corner1;
        public int corner2;
        public int corner3;
        public int corner4;
        public int corner5;
        public int corner6;
        public int corner7;

        public int crossPointIndex;
    }

    /// <summary>
    /// VerticesBuffer stores all the vertices that form the mesh
    /// </summary>
    public struct VerticesBuffer : IBufferElementData 
    {
        public VerticeElement vertice;
    }

    public struct VerticeElement 
    {
        public float3 position;
        public float3 normal;
    }

    /// <summary>
    /// TrianglesBuffer stores all the triangles that form the mesh
    /// </summary>
    public struct TrianglesBuffer : IBufferElementData 
    {
        public int Value;
    }

    /// <summary>
    /// IntersectingEdgesBuffer stores all intersecting edges. This buffer is used in poligonization fase.
    /// </summary>
    public struct IntersectingEdgesBuffer : IBufferElementData 
    {
        public IntersectingEdgesElement edgeData;
    }

    public struct IntersectingEdgesElement
    {
        public int vertexIndex0;
        public int vertexIndex1;

        // Shared cells between both vertex
        public Cell sharedCells00;
        public Cell sharedCells01;
        public Cell sharedCells10;
        public Cell sharedCells11;
    }
}