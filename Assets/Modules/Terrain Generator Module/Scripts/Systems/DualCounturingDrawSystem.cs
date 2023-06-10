//#define DEBUG_DualContouring__DrawVertex
//#define DEBUG_DualContouring__DrawVertex_Index
//#define DEBUG_DualContouring__DrawNormals
//#define DEBUG_DualContouring__DrawInteresectingEdges
//#define DEBUG_DualContouring__DrawInteresectingEdges_Index
//#define DEBUG_DualContouring__DrawInteresectingEdges_Axis

using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using TerrainGenerator.Utils;
using Unity.Mathematics;
using UnityEditor.Searcher;

namespace TerrainGenerator 
{
    [UpdateInGroup(typeof(TerrainGeneratorSystemGroup))]
    [UpdateAfter(typeof(DualCounturingSystem))]
    public partial struct DualCounturingDrawSystem : ISystem
    {
        private const float vertexRadius = 0.1f;
        private const float normalLenght = 0.2f;

        private Color vertexColor;
        private Color normalColor;
        private Color intersectingEdgeColor;
        private Color intersectingEdgeIsBorderColor;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            vertexColor = Color.red;
            normalColor = Color.blue;
            intersectingEdgeColor = Color.cyan;
            intersectingEdgeIsBorderColor = Color.magenta;
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
#if DEBUG_DualContouring__DrawVertex
                DrawVertices(chunk);
#endif

#if DEBUG_DualContouring__DrawInteresectingEdges
                DrawIntersectiongEdges(chunk);
#endif
            };

        }

        private void DrawVertices(ChunkAspect chunk) 
        {
            for (int i = 0; i < chunk.verticesBuffer.Length; i++)
            {
                VerticeElement vertice = chunk.verticesBuffer[i].vertice;

                Draw.DrawSphere(vertice.position, vertexRadius, vertexColor);

#if DEBUG_DualContouring__DrawVertex_Index
                float3 vertexIndexOffset = new float3(1, 1, 0) * vertexRadius;
                Draw.DrawText(vertice.position + vertexIndexOffset, i.ToString());
#endif

#if DEBUG_DualContouring__DrawNormals
                Draw.DrawLine(vertice.position, (vertice.position + vertice.normal * normalLenght), normalColor);
#endif
            }
        }

        private void DrawIntersectiongEdges(ChunkAspect chunk) 
        {
                for (int i = 0; i < chunk.edgesBuffer.Length; i++)
            {
                IntersectingEdgesElement edge = chunk.edgesBuffer[i].edgeData;

                GridVertexElement gridVertex1 = chunk.GridVertexArray[edge.vertexIndex0];
                GridVertexElement gridVertex2 = chunk.GridVertexArray[edge.vertexIndex1];

                bool isBorder = MeshMaths.VertexIsBorder(gridVertex1, chunk.Resolution) && MeshMaths.VertexIsBorder(gridVertex2, chunk.Resolution);

                Draw.DrawLine(gridVertex1.position, gridVertex2.position, (isBorder) ? intersectingEdgeIsBorderColor : intersectingEdgeColor);

                string edgeInfoMsg = "";

                float3 intersectingEdgesOffset = new float3(0, 1, 0) * vertexRadius;

                if (edge.axis == 1)
                {
                    intersectingEdgesOffset = new float3(1, 0, 0) * vertexRadius;
                }

                float3 position = (gridVertex1.position + gridVertex2.position) * 0.5f;

#if DEBUG_DualContouring__DrawInteresectingEdges_Index

                edgeInfoMsg += edge.index.ToString();
#endif

#if DEBUG_DualContouring__DrawInteresectingEdges_Axis
                if (!string.IsNullOrEmpty(edgeInfoMsg))
                    edgeInfoMsg += ":";

                switch (edge.axis)
                {
                    case 0:
                        edgeInfoMsg += "X";
                        break;
                    case 1:
                        edgeInfoMsg += "Y";
                        break;
                    case 2:
                        edgeInfoMsg += "Z";
                        break;
                    default:
                        edgeInfoMsg += "?";
                        break;
                }
#endif
                if (!string.IsNullOrEmpty(edgeInfoMsg))
                    Draw.DrawText(position + intersectingEdgesOffset, edgeInfoMsg);
            }
        }
    }
}