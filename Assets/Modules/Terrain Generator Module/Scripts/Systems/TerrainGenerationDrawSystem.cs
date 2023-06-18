//#define DEBUG_TerrainGenerator__DrawChunkBounds
//#define DEBUG_TerrainGenerator__GridVertex
//#define DEBUG_TerrainGenerator__GridVertexIndex
//#define DEBUG_TerrainGenerator__DrawCell
//#define DEBUG_TerrainGenerator__DrawCellWithVertice
//#define DEBUG_TerrainGenerator__DrawCellIndex

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TerrainGenerator.Utils;

namespace TerrainGenerator
{
    //[UpdateInGroup(typeof(TerrainGeneratorSystemGroup))]
    //[UpdateAfter(typeof(TerrainGeneratorSystem))]
    [BurstCompile]
    public partial struct TerrainGeneratorDrawSystem : ISystem
    {
        private const float gizmoSphereRadiusFactor = 0.05f;

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

#if DEBUG_TerrainGenerator__DrawChunkBounds
                DrawBounds(chunk.Position, chunk.Size);
#endif

#if DEBUG_TerrainGenerator__GridVertex
                float gizmoSphereRadius = chunk.Size / chunk.Resolution * gizmoSphereRadiusFactor;
                DrawCorners(chunk.GridVertexArray, gizmoSphereRadius);
#endif

                bool drawWithVertiece = false;

#if DEBUG_TerrainGenerator__DrawCellWithVertice
                drawWithVertiece = true;
#endif

#if DEBUG_TerrainGenerator__DrawCell
                DrawCells(chunk.GridVertexArray, chunk.CellArray, drawWithVertiece);
#endif

            };
        }

        /// <summary>
        /// Draw a box with the boundaries of the chunk
        /// </summary>
        /// <param name="position"> Chunk pivot postion </param>
        /// <param name="size"> Size of the chunk </param>
        private void DrawBounds(float3 position, float size)
        {
            //DrawHelper.DrawCube(position, size, Color.yellow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridVertexArray"></param>
        /// <param name="gizmoSphereRadius"></param>
        private void DrawCorners(GridVertexElement[] gridVertexArray, float gizmoSphereRadius)
        {

            for (int i = 0; i < gridVertexArray.Length; i++)
            {
                DrawHelper.DrawSphere(gridVertexArray[i].position, gizmoSphereRadius, gridVertexArray[i].value < 0 ? Color.white : Color.black);
#if DEBUG_TerrainGenerator__GridVertexIndex
                float3 gridVertexIndexOffset = new float3(1, 1, 0) * gizmoSphereRadius;
                Draw.DrawText(gridVertexArray[i].position + gridVertexIndexOffset, i.ToString());
# endif
            }
        }

        private void DrawCells(GridVertexElement[] gridVertex, CellElement[] cells, bool cellWithVertice = false)
        {
            for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
            {
                //If the corner has a vertice
                if (cellWithVertice) 
                {
                    int control = 0;

                    control |= (gridVertex[cells[cellIndex].corner0].value < 0.0f ? 0 : 1) << 0;
                    control |= (gridVertex[cells[cellIndex].corner1].value < 0.0f ? 0 : 1) << 0;
                    control |= (gridVertex[cells[cellIndex].corner2].value < 0.0f ? 0 : 1) << 0;
                    control |= (gridVertex[cells[cellIndex].corner3].value < 0.0f ? 0 : 1) << 0;
                    control |= (gridVertex[cells[cellIndex].corner4].value < 0.0f ? 0 : 1) << 0;
                    control |= (gridVertex[cells[cellIndex].corner5].value < 0.0f ? 0 : 1) << 0;
                    control |= (gridVertex[cells[cellIndex].corner6].value < 0.0f ? 0 : 1) << 0;
                    control |= (gridVertex[cells[cellIndex].corner7].value < 0.0f ? 0 : 1) << 0;


                    if (control == 0 || control == 255)
                    {
                        continue;
                    }
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

                //DrawHelper.DrawCube(center, side, Color.green);

#if DEBUG_TerrainGenerator__DrawCellIndex
                Draw.DrawText(center, $"[{cellIndex}]");
#endif
            }
        }
    }
}