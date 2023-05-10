#define DEBUG_TerrainGenerator__DrawCorner
#define DEBUG_TerrainGenerator__DrawChunkBounds

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TerrainGenerator.Utils;

namespace TerrainGenerator 
{
    public class TerrainGeneratorSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(TerrainGeneratorSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct TerrainGeneratorSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, chunk) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ChunkComponent>>()) 
            {
                GenerateTerrainData(transform.ValueRO, ref chunk.ValueRW);
            };
        }

        private void GenerateTerrainData(LocalTransform transform, ref ChunkComponent chunk) 
        {
            int resolution = chunk.resolution;
            int resolution2 = resolution * resolution;

            // Vertices
            Vertex[] corner = chunk.vertices.ToArray();

            float3 centerOfset = new float3(-1, -1, -1) * chunk.size / 2;

            for (int i = 0; i < chunk.vertices.Length; i++)
            {
                //Corner info
                corner[i].position = transform.Position + ((float3)MeshMaths.IndexToPosition(i, resolution)/(resolution - 1)) * chunk.size + centerOfset;
                corner[i].value = MyNoise.PerlinNoise3D.get3DPerlinNoise(corner[i].position, chunk.frequency);
            }

            // Celdas
            Cell[] cells = chunk.cells.ToArray();

            int cellIndex = 0;

            for (int z = 0; z < resolution -1; z++) 
            {
                for (int y = 0; y < resolution - 1; y++)
                {
                    for (int x = 0; x < resolution - 1; x++)
                    {
                        int verticeIndex = MeshMaths.PositionToIndex(new int3(x,y,z), resolution);

                        cells[cellIndex].corner0 = verticeIndex;
                        cells[cellIndex].corner1 = verticeIndex + 1;
                        cells[cellIndex].corner2 = verticeIndex + resolution;
                        cells[cellIndex].corner3 = verticeIndex + 1 + resolution;
                        cells[cellIndex].corner4 = verticeIndex + resolution2;
                        cells[cellIndex].corner5 = verticeIndex + 1 + resolution2;
                        cells[cellIndex].corner6 = verticeIndex + resolution + resolution2;
                        cells[cellIndex].corner7 = verticeIndex + 1 + resolution + resolution2;

                        Debug.Log($"Cell {cellIndex}: " +
                            $"[{cells[cellIndex].corner0}, {cells[cellIndex].corner1}, " +
                            $"{cells[cellIndex].corner2}, {cells[cellIndex].corner3}, " +
                            $"{cells[cellIndex].corner4}, {cells[cellIndex].corner5}, " +
                            $"{cells[cellIndex].corner6}, {cells[cellIndex].corner7}]");

                        cellIndex++;

                        //DualContouring.CalculatePoint(corner, cells[cellIndex]);
                    }
                }
            }

            NativeArray<Vertex>.Copy(corner, chunk.vertices, corner.Length);
            NativeArray<Cell>.Copy(cells, chunk.cells, cells.Length);
        }
    }  
}

