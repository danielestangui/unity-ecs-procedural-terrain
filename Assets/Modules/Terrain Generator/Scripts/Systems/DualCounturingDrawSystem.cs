#define DEBUG_TerrainGenerator__DrawCell
#define DEBUG_TerrainGenerator__DrawCellWithVertice
#define DEBUG_TerrainGenerator__DrawCellIndex
#define DEBUG_DualContouring__DrawVertex
//#define DEBUG_DualContouring__DrawNormals



using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using TerrainGenerator.Utils;
using Unity.Mathematics;

namespace TerrainGenerator 
{
    [UpdateInGroup(typeof(TerrainGeneratorSystemGroup))]
    [UpdateAfter(typeof(DualCounturingSystem))]
    public partial struct DualCounturingDrawSystem : ISystem
    {
        private const float vertexRadius = 0.1f;
        private const float normalLenght = 0.2f;


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
            Color vertexColor = Color.red;
            Color normalColor = Color.blue;

            foreach (var chunk in SystemAPI.Query<ChunkAspect>())
            {
                bool drawWithVertiece = false;

#if DEBUG_TerrainGenerator__DrawCellWithVertice
                drawWithVertiece = true;
#endif

#if DEBUG_TerrainGenerator__DrawCell
                DrawCells(chunk.GridVertexArray, chunk.CellArray, drawWithVertiece);
#endif
            };

            foreach (var VerticeBuffer in SystemAPI.Query<DynamicBuffer<VerticesBuffer>>())
            {
                for (int i = 0; i < VerticeBuffer.Length; i++)
                {
#if DEBUG_DualContouring__DrawVertex
                    Draw.DrawSphere(VerticeBuffer[i].vertice.position, vertexRadius, vertexColor);
#endif

#if DEBUG_DualContouring__DrawNormals
                    Draw.DrawLine(buffer[i].vertice.position, (buffer[i].vertice.position + buffer[i].vertice.normal * normalLenght), normalColor);
#endif
                    }
            };
        }

        private void DrawCells(GridVertex[] gridVertex, Cell[] cells, bool cellWithVertice = false)
        {
            for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
            {
                if (cellWithVertice && cells[cellIndex].crossPointIndex < 0) 
                {
                    break;
                }

                float3[] corners = {
                    gridVertex[cells[cellIndex].corner0].position,
                    gridVertex[cells[cellIndex].corner1].position,
                    gridVertex[cells[cellIndex].corner2].position,
                    gridVertex[cells[cellIndex].corner3].position,
                    gridVertex[cells[cellIndex].corner4].position,
                    gridVertex[cells[cellIndex].corner5].position,
                    gridVertex[cells[cellIndex].corner6].position,
                    gridVertex[cells[cellIndex].corner7].position
                };

                float side = Vector3.Distance(gridVertex[cells[cellIndex].corner0].position, gridVertex[cells[cellIndex].corner1].position);
                float3 center = MeshMaths.GetCenterOfCube(corners);

                Draw.DrawCube(center, side, Color.green);

#if DEBUG_TerrainGenerator__DrawCellIndex
                Draw.DrawText(center, $"[{cellIndex}]");
#endif
            }
        }
    }
}