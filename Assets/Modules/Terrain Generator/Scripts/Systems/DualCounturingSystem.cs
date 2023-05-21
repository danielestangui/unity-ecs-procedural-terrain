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
            foreach (var (verticesBuffer, edgesBuffer, triangleBuffer, chunk) in 
                SystemAPI.Query<DynamicBuffer<VerticesBuffer>, DynamicBuffer<IntersectingEdgesBuffer>, DynamicBuffer<TrianglesBuffer>, RefRW<ChunkComponent>>())
            {
                Cell[] cells = chunk.ValueRO.cells.ToArray();
                Vertex[] corner = chunk.ValueRO.vertices.ToArray();
                List<IntersectingEdgesElement> edges = new List<IntersectingEdgesElement>();

                for (int i = 0; i < cells.Length; i++)
                {
                    VerticeElement vertice = DualContouring.CalculatePoint(i,corner, cells, chunk.ValueRO.resolution, ref edges, cells[i]);

                    cells[i].isCrossPoint = !vertice.position.Equals(float3.zero);

                    if (!vertice.position.Equals(float3.zero)) 
                    {

                        VerticesBuffer element = new VerticesBuffer 
                        { 
                            vertice = vertice 
                        };

                        cells[i].crossPointIndex = verticesBuffer.Length;
                        verticesBuffer.Add(element);
                    }

                }

                Debug.Log($"Edges: {edges.ToArray().Length}");

                foreach (IntersectingEdgesElement element  in edges) 
                {
                    edgesBuffer.Add(new IntersectingEdgesBuffer
                    {
                        edgeData = element
                    });
                }

                for (int i = 0; i < edgesBuffer.Length; i++)
                {
                    int[] tri = GenerateTriangles(edgesBuffer[i], verticesBuffer);

                    for (int j = 0; j < tri.Length; j++)
                    {
                        triangleBuffer.Add(new TrianglesBuffer { Value = tri[j] });
                    }
                }

                edgesBuffer.Clear();

                NativeArray<Cell>.Copy(cells, chunk.ValueRW.cells, cells.Length);
            };
        }

        private int[] GenerateTriangles(IntersectingEdgesBuffer edge, DynamicBuffer<VerticesBuffer> vertices) 
        {
            List<int> triangles = new List<int>();

            triangles.Add(edge.edgeData.sharedCells00.crossPointIndex);
            triangles.Add(edge.edgeData.sharedCells10.crossPointIndex);
            triangles.Add(edge.edgeData.sharedCells11.crossPointIndex);

            triangles.Add(edge.edgeData.sharedCells00.crossPointIndex);
            triangles.Add(edge.edgeData.sharedCells11.crossPointIndex);
            triangles.Add(edge.edgeData.sharedCells01.crossPointIndex);

            return triangles.ToArray();
        }
    }

    [UpdateInGroup(typeof(TerrainGeneratorSystemGroup))]
    [UpdateAfter(typeof(DualCounturingSystem))]
    public partial struct DualCounturingDrawSystem : ISystem
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
            foreach (var buffer in SystemAPI.Query<DynamicBuffer<VerticesBuffer>>())
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    Draw.DrawSphere(buffer[i].vertice.position, 0.1f, Color.red);
                    Draw.DrawLine(buffer[i].vertice.position, (buffer[i].vertice.position + buffer[i].vertice.normal * 0.2f), Color.blue);
                }
            };
        }
    }
}