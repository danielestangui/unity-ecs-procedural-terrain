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
                    vertices = new NativeArray<Vertex>(vertexResolution, Allocator.Persistent),
                    cells = new NativeArray<Cell>(cellResolution, Allocator.Persistent),
                    //frequency = authoring.threshold,
                    //threshold = authoring.frequency
                };
                AddComponent(data);
            }
        }
    }

    struct ChunkComponent : IComponentData
    {
        public int resolution;
        public float size;
        public NativeArray<Vertex> vertices;
        public NativeArray<Cell> cells;

        //public float frequency;
        //public float threshold;
    }

    public struct Vertex
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

        public bool isCrossPoint;
        public float3 crossPoint;
    }
}