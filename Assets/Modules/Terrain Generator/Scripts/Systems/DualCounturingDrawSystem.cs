#define DEBUG_DualContouring__DrawVertex
#define DEBUG_DualContouring__DrawVertexIndex
#define DEBUG_DualContouring__DrawNormals
#define DEBUG_DualContouring__DrawInteresectingEdges
#define DEBUG_DualContouring__DrawInteresectingEdgesIndex

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

            foreach (var vertex in SystemAPI.Query<DynamicBuffer<VerticesBuffer>>())
            {
                for (int i = 0; i < vertex.Length; i++)
                {
#if DEBUG_DualContouring__DrawVertex
                    Draw.DrawSphere(vertex[i].vertice.position, vertexRadius, vertexColor);
#endif
#if DEBUG_DualContouring__DrawVertex
                    float3 vertexIndexOffset = new float3(1, 1, 0) * vertexRadius;
                    Draw.DrawText(vertex[i].vertice.position + vertexIndexOffset, i.ToString());
#endif

#if DEBUG_DualContouring__DrawNormals
                    Draw.DrawLine(vertex[i].vertice.position, (vertex[i].vertice.position + vertex[i].vertice.normal * normalLenght), normalColor);
#endif
                    }
            };
#if DEBUG_DualContouring__DrawInteresectingEdges
            foreach (var chunk in SystemAPI.Query<ChunkAspect>())
            {
                for (int i = 0; i < chunk.edgesBuffer.Length; i++)
                {
                    GridVertex gridVertex1 = chunk.GridVertexArray[chunk.edgesBuffer[i].edgeData.vertexIndex0];
                    GridVertex gridVertex2 = chunk.GridVertexArray[chunk.edgesBuffer[i].edgeData.vertexIndex1];

                    bool isBorder = MeshMaths.VertexIsBorder(gridVertex1, chunk.Resolution) && MeshMaths.VertexIsBorder(gridVertex2, chunk.Resolution);

                    Draw.DrawLine(gridVertex1.position, gridVertex2.position, (isBorder) ? Color.magenta : Color.red);

                }
            };
#endif
        }
    }
}