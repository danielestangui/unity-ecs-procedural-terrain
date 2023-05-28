using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using TerrainGenerator.Utils;
using Unity.Collections;

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
            foreach (var chunk in SystemAPI.Query<ChunkAspect>()) 
            {
                chunk.verticesBuffer.Clear();
                chunk.triangleBuffer.Clear();

                chunk.GridVertexArray = GenerateGridVertexData(chunk.Position, chunk.Resolution, chunk.Size);
                chunk.CellArray = GenerateCellData(chunk.Resolution);
            };
        }

        /// <summary>
        /// Generate all chunk's grid vertex data
        /// </summary>
        /// <param name="position"> Chunk pivot postion </param>
        /// <param name="resolution"> Number of grid vertex per chunk side </param>
        /// <param name="size"> Size of the chunk </param>
        /// <returns></returns>
        private GridVertex[] GenerateGridVertexData(float3 position, int resolution, float size) 
        {
            GridVertex[] gridVertexArray = new GridVertex[resolution * resolution * resolution];

            float3 centerOfset = new float3(-1, -1, -1) * size / 2;

            for (int i = 0; i < gridVertexArray.Length; i++)
            {
                gridVertexArray[i].index = i;
                gridVertexArray[i].position = position + ((float3)MeshMaths.IndexToPosition(i, resolution) / (resolution - 1)) * size + centerOfset;
                gridVertexArray[i].value = MyNoise.PerlinNoise3D.DensityFunction(gridVertexArray[i].position);
            }

            return gridVertexArray;
        }

        /// <summary>
        /// Generate all chunk's cells data
        /// </summary>
        /// <param name="resolution"> Number of grid vertex per chunk side </param>
        /// <returns></returns>
        private Cell[] GenerateCellData(int resolution) 
        {
            int cellResolution = (resolution - 1);
            int cellArrayResolution = cellResolution * cellResolution * cellResolution;
            int resolution2 = resolution * resolution;

            Cell[] cellArray = new Cell[cellArrayResolution];

            int cellIndex = 0;

            for (int z = 0; z < resolution - 1; z++)
            {
                for (int y = 0; y < resolution - 1; y++)
                {
                    for (int x = 0; x < resolution - 1; x++)
                    {
                        int verticeIndex = MeshMaths.PositionToIndex(new int3(x, y, z), resolution);

                        cellArray[cellIndex].index = cellIndex;
                        cellArray[cellIndex].corner0 = verticeIndex;
                        cellArray[cellIndex].corner1 = verticeIndex + 1;
                        cellArray[cellIndex].corner2 = verticeIndex + resolution;
                        cellArray[cellIndex].corner3 = verticeIndex + 1 + resolution;
                        cellArray[cellIndex].corner4 = verticeIndex + resolution2;
                        cellArray[cellIndex].corner5 = verticeIndex + 1 + resolution2;
                        cellArray[cellIndex].corner6 = verticeIndex + resolution + resolution2;
                        cellArray[cellIndex].corner7 = verticeIndex + 1 + resolution + resolution2;

                        cellIndex++;
                    }
                }
            }

            return cellArray;
        }   
    }  
}