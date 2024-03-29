//#define DEBUG_DualCounturingSystem__Verbose

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using TerrainGenerator.Utils;
using System.Collections.Generic;
using Unity.Jobs;

namespace TerrainGenerator
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(TerrainGenerationSystemGroup))]
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

                chunk.verticesBuffer.Clear();

                FunctionFillVerticeBuffer(chunk, ref edges);

                // Copia la lista para pasarla al bufer
                chunk.edgesBuffer.Clear();

                foreach (IntersectingEdgesElement element  in edges) 
                {
                    chunk.edgesBuffer.Add(new IntersectingEdgesBuffer
                    {
                        edgeData = element
                    });
                }

                chunk.triangleBuffer.Clear();
                GenerateTriangles(chunk);
            };
        }

        private void FunctionFillVerticeBuffer(ChunkAspect chunk,ref List<IntersectingEdgesElement> edges) 
        {
            for (int i = 0; i < chunk.CellArray.Length; i++)
            {
                VerticeElement vertice = DualContouring.CalculatePoint(i, chunk.verticesBuffer.Length, chunk.GridVertexArray, chunk.CellArray, chunk.Resolution, ref edges);

                if (vertice.index >= 0)
                {
                    VerticesBuffer element = new VerticesBuffer
                    {
                        vertice = vertice
                    };
                    
                    chunk.verticesBuffer.Add(element);
                }
            }
        }

        struct FillVerticeBuffer : IJobParallelFor 
        {
            ChunkAspect chunk;
            // Esto mal
            List<IntersectingEdgesElement> edges;

            public void Execute(int i) 
            {
                VerticeElement vertice = DualContouring.CalculatePoint(i, chunk.verticesBuffer.Length, chunk.GridVertexArray, chunk.CellArray, chunk.Resolution, ref edges);

                if (vertice.index >= 0)
                {
                    VerticesBuffer element = new VerticesBuffer
                    {
                        vertice = vertice
                    };

                    chunk.verticesBuffer.Add(element);
                }
            }
        }

        private void GenerateTriangles(ChunkAspect chunk) 
        {
            for (int i = 0; i < chunk.edgesBuffer.Length; i++)
            {
                List<int> tri = new List<int>();

                GridVertexElement gridVertex1 = chunk.GridVertexArray[chunk.edgesBuffer[i].edgeData.vertexIndex0];
                GridVertexElement gridVertex2 = chunk.GridVertexArray[chunk.edgesBuffer[i].edgeData.vertexIndex1];

                // Las aristas que estan incluidas en el bordes del chunk se trataran en otro momento para crear los tris 
                // Que unen los distintos chunks
                if (!(MeshMaths.VertexIsBorder(gridVertex1, chunk.Resolution) && 
                    MeshMaths.VertexIsBorder(gridVertex2, chunk.Resolution)))
                {
                    // Obtenemos todos los vertices que van a formar el tri
                    VerticeElement vertex00 = new VerticeElement();
                    chunk.GetVerticeFromCell(chunk.edgesBuffer[i].edgeData.sharedCells00, ref vertex00);

                    VerticeElement vertex01 = new VerticeElement();
                    chunk.GetVerticeFromCell(chunk.edgesBuffer[i].edgeData.sharedCells01, ref vertex01);

                    VerticeElement vertex10 = new VerticeElement();
                    chunk.GetVerticeFromCell(chunk.edgesBuffer[i].edgeData.sharedCells10, ref vertex10);

                    VerticeElement vertex11 = new VerticeElement();
                    chunk.GetVerticeFromCell(chunk.edgesBuffer[i].edgeData.sharedCells11, ref vertex11);

                    float3 normal = (vertex00.normal + vertex01.normal + vertex10.normal + vertex11.normal)/4;

                    // Creamos los tris segun el axis
                    switch (chunk.edgesBuffer[i].edgeData.axis) 
                    {
                        case 0:
                            // Axis X

                            if (normal.x > 0)
                            {
                                tri.Add(vertex00.index);
                                tri.Add(vertex01.index);
                                tri.Add(vertex11.index);

                                tri.Add(vertex00.index);
                                tri.Add(vertex11.index);
                                tri.Add(vertex10.index);
                            }
                            else 
                            {
                                tri.Add(vertex00.index);
                                tri.Add(vertex11.index);
                                tri.Add(vertex01.index);

                                tri.Add(vertex00.index);
                                tri.Add(vertex10.index);
                                tri.Add(vertex11.index);
                            }

                            break;
                        case 1:
                            // Axis Y

                            if (normal.y < 0)
                            {
                                tri.Add(vertex00.index);
                                tri.Add(vertex01.index);
                                tri.Add(vertex11.index);

                                tri.Add(vertex00.index);
                                tri.Add(vertex11.index);
                                tri.Add(vertex10.index);
                            }
                            else
                            {
                                tri.Add(vertex00.index);
                                tri.Add(vertex11.index);
                                tri.Add(vertex01.index);

                                tri.Add(vertex00.index);
                                tri.Add(vertex10.index);
                                tri.Add(vertex11.index);
                            }

                            break;
                        case 2:
                            // Axis Z

                            if (normal.z > 0)
                            {
                                tri.Add(vertex00.index);
                                tri.Add(vertex01.index);
                                tri.Add(vertex11.index);

                                tri.Add(vertex00.index);
                                tri.Add(vertex11.index);
                                tri.Add(vertex10.index);
                            }
                            else
                            {
                                tri.Add(vertex00.index);
                                tri.Add(vertex11.index);
                                tri.Add(vertex01.index);

                                tri.Add(vertex00.index);
                                tri.Add(vertex10.index);
                                tri.Add(vertex11.index);
                            }

                            break;
                    }

                    for (int j = 0; j < tri.Count; j++)
                    {
                        chunk.triangleBuffer.Add(new TrianglesBuffer { Value = tri[j] });
                    }
                }
            }
        }
    }
}