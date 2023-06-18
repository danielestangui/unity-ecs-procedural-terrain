//#define DEBUG_TerrainGeneratorRenderSystem_Verbose

using System.Collections;
using System.Collections.Generic;
using TerrainGenerator.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator 
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(TerrainGenerationSystemGroup))]
    [UpdateAfter(typeof(DualCounturingSystem))]
    public partial class TerrainGeneratorRenderSystem : SystemBase
    {
        TerrainGeneratorRenderComponent renderComponent;

        private float3 targetPosition;

        protected override void OnCreate()
        {
#if DEBUG_TerrainGeneratorRenderSystem_Verbose
            Debug.Log($"[TerrainGeneratorRenderSystem]OnCreate");
#endif
            //SystemAPI.RequireForUpdate<TerrainGeneratorRenderComponent>();
        }

        protected override void OnUpdate()
        {
#if DEBUG_TerrainGeneratorRenderSystem_Verbose
            Debug.Log($"[TerrainGeneratorRenderSystem]OnCreate");
#endif

            // Restart Shapes Cicle
            DrawHelper.ClearOnDrawGizmoActions();

            // GetSingleton
            if (!SystemAPI.HasSingleton<TerrainGeneratorRenderComponent>()) 
            {
                return;
            }

            renderComponent = SystemAPI.GetSingleton<TerrainGeneratorRenderComponent>();

            targetPosition = OctreeLOD.GetTargetPosition();

            // Leafs Bounding Box
            if (renderComponent.showLeafBoundingBoxEnable) 
            {
                foreach (var leaf in SystemAPI.Query<OctreeLeafAspect>())
                {
                    DrawHelper.DrawCube(leaf.Position, leaf.Size, renderComponent.showLeafBoundingBoxThickness, renderComponent.showLeafBoundingBoxColor);
                }
            }

            // Grid Vertex
            if (renderComponent.showGridVertexEnable)
            {
                foreach (var chunk in SystemAPI.Query<ChunkAspect>()) 
                {
                    for (int i = 0; i < chunk.GridVertexArray.Length; i++)
                    {
                        float interpolator = (chunk.GridVertexArray[i].value + 1) * 0.5f;
                        Color color = Color.Lerp(renderComponent.showGridVertexAirColor, renderComponent.showGridVertexGroundColor, interpolator);

                        if (!renderComponent.showGridVertexGradientColor) 
                        {
                            color = (interpolator > 0.5f) ? renderComponent.showGridVertexAirColor : renderComponent.showGridVertexGroundColor;
                        }

                        DrawHelper.DrawSphere(chunk.GridVertexArray[i].position, renderComponent.showGridVertexRadius, color);
                    }
                }
            }

            // Cells
            // TODO: Hay funcionalidades quie se pueden mover a DualContouring.cs
            if (renderComponent.showCellEnable)
            {
                foreach (var chunk in SystemAPI.Query<ChunkAspect>())
                {
                    for (int i = 0; i < chunk.CellArray.Length; i++)
                    {
                        //If the corner has a vertice
                        if (renderComponent.showOnlyCellWithVertex)
                        {
                            int control = 0;

                            control |= (chunk.GridVertexArray[chunk.CellArray[i].corner0].value < 0.0f ? 0 : 1) << 0;
                            control |= (chunk.GridVertexArray[chunk.CellArray[i].corner1].value < 0.0f ? 0 : 1) << 1;
                            control |= (chunk.GridVertexArray[chunk.CellArray[i].corner2].value < 0.0f ? 0 : 1) << 2;
                            control |= (chunk.GridVertexArray[chunk.CellArray[i].corner3].value < 0.0f ? 0 : 1) << 3;
                            control |= (chunk.GridVertexArray[chunk.CellArray[i].corner4].value < 0.0f ? 0 : 1) << 4;
                            control |= (chunk.GridVertexArray[chunk.CellArray[i].corner5].value < 0.0f ? 0 : 1) << 5;
                            control |= (chunk.GridVertexArray[chunk.CellArray[i].corner6].value < 0.0f ? 0 : 1) << 6;
                            control |= (chunk.GridVertexArray[chunk.CellArray[i].corner7].value < 0.0f ? 0 : 1) << 7;

                            if (control == 0 || control == 255)
                            {
                                continue;
                            }
                        }

                        float3[] corners = {
                            chunk.GridVertexArray[chunk.CellArray[i].corner0].position,
                            chunk.GridVertexArray[chunk.CellArray[i].corner1].position,
                            chunk.GridVertexArray[chunk.CellArray[i].corner2].position,
                            chunk.GridVertexArray[chunk.CellArray[i].corner3].position,
                            chunk.GridVertexArray[chunk.CellArray[i].corner4].position,
                            chunk.GridVertexArray[chunk.CellArray[i].corner5].position,
                            chunk.GridVertexArray[chunk.CellArray[i].corner6].position,
                            chunk.GridVertexArray[chunk.CellArray[i].corner7].position
                        };

                        float side = Vector3.Distance(
                            chunk.GridVertexArray[chunk.CellArray[i].corner0].position, 
                            chunk.GridVertexArray[chunk.CellArray[i].corner1].position
                            );

                        float3 center = MeshMaths.GetCenterOfCube(corners);

                        DrawHelper.DrawCube(center, side, renderComponent.showCellThickness, renderComponent.showCellColor);
                    }
                }
            }

            if (renderComponent.showVertexEnable) 
            {
                foreach (var chunk in SystemAPI.Query<ChunkAspect>()) 
                {
                    for (int i = 0; i < chunk.verticesBuffer.Length; i++)
                    {
                        VerticeElement vertice = chunk.verticesBuffer[i].vertice;

                        DrawHelper.DrawSphere(vertice.position, renderComponent.showVertexRadius, renderComponent.showVertexColor);

                        if (renderComponent.showVertexEnableNormals) 
                        {
                            float3 to = vertice.position + vertice.normal * renderComponent.showVertexNormalLenght;
                            DrawHelper.DrawLine(vertice.position, to, renderComponent.showVertexNormalColor);
                        }
                    }
                }
            }

            if (renderComponent.showEdgesEnable) 
            {
                foreach (var chunk in SystemAPI.Query<ChunkAspect>())
                {
                    for (int i = 0; i < chunk.edgesBuffer.Length; i++)
                    {
                        IntersectingEdgesElement edge = chunk.edgesBuffer[i].edgeData;

                        GridVertexElement gridVertex1 = chunk.GridVertexArray[edge.vertexIndex0];
                        GridVertexElement gridVertex2 = chunk.GridVertexArray[edge.vertexIndex1];

                        bool isBorder = MeshMaths.VertexIsBorder(gridVertex1, chunk.Resolution) && MeshMaths.VertexIsBorder(gridVertex2, chunk.Resolution);

                        DrawHelper.DrawLine(gridVertex1.position, gridVertex2.position, (isBorder) ? renderComponent.showEdgesBorderColor : renderComponent.showEdgesInteriorColor);
                    }
                }
            }
        }
    }
}