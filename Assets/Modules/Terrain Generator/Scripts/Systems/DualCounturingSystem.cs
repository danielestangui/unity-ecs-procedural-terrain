using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TerrainGenerator.Utils;
using Unity.Collections;
using System.Collections.Generic;

namespace TerrainGenerator
{
    [UpdateInGroup(typeof(TerrainGeneratorSystemGroup))]
    [UpdateAfter(typeof(TerrainGeneratorSystem))]
    public partial struct DualCounturingSystem : ISystem
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
                List<IntersectingEdgesElement> edges = new List<IntersectingEdgesElement>();

                FillVerticeBuffer(chunk, ref edges);

                string msg = "";
                for (int j = 0; j < chunk.verticesBuffer.Length; j++)
                {
                   msg += chunk.verticesBuffer[j].vertice.cell.index + ", ";
                }

                Debug.Log($"Vertrices: ({chunk.verticesBuffer.Length}) {msg}");

                // Copia la lista para pasarla al bufer
                chunk.edgesBuffer.Clear();

                foreach (IntersectingEdgesElement element  in edges) 
                {
                    chunk.edgesBuffer.Add(new IntersectingEdgesBuffer
                    {
                        edgeData = element
                    });
                }

                Debug.Log($"Edges: {chunk.edgesBuffer.Length}");

                // Crea los triangulos
                for (int i = 0; i < chunk.edgesBuffer.Length; i++)
                {
                    int[] tri = GenerateTriangles(chunk.edgesBuffer[i], chunk.verticesBuffer);

                    for (int j = 0; j < tri.Length; j++)
                    {
                        chunk.triangleBuffer.Add(new TrianglesBuffer { Value = tri[j] });
                    }
                }

                //Debug.Log($"Triangles: {chunk.triangleBuffer[23].Value}");
            };
        }

        private void FillVerticeBuffer(ChunkAspect chunk,ref List<IntersectingEdgesElement> edges) 
        {
            for (int i = 0; i < chunk.CellArray.Length; i++)
            {
                VerticeElement vertice = DualContouring.CalculatePoint(i, chunk.GridVertexArray, chunk.CellArray, chunk.Resolution, ref edges);

                if (!vertice.position.Equals(float3.zero))
                {
                    VerticesBuffer element = new VerticesBuffer
                    {
                        vertice = vertice
                    };
                    
                    chunk.verticesBuffer.Add(element);
                }
            }
        }

        private int[] GenerateTriangles(IntersectingEdgesBuffer edge, DynamicBuffer<VerticesBuffer> vertices) 
        {
            List<int> triangles = new List<int>();

            /*triangles.Add(edge.edgeData.sharedCells00.crossPointIndex);
            triangles.Add(edge.edgeData.sharedCells10.crossPointIndex);
            triangles.Add(edge.edgeData.sharedCells11.crossPointIndex);

            triangles.Add(edge.edgeData.sharedCells00.crossPointIndex);
            triangles.Add(edge.edgeData.sharedCells11.crossPointIndex);
            triangles.Add(edge.edgeData.sharedCells01.crossPointIndex);*/

            return triangles.ToArray();
        }
    }

   
}