using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TerrainGenerator.Utils;
using Unity.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;

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

                chunk.verticesBuffer.Clear();

                FillVerticeBuffer(chunk, ref edges);

                // Copia la lista para pasarla al bufer
                chunk.edgesBuffer.Clear();

                foreach (IntersectingEdgesElement element  in edges) 
                {
                    chunk.edgesBuffer.Add(new IntersectingEdgesBuffer
                    {
                        edgeData = element
                    });
                }

                Debug.Log($"[DualCounturingSystem]Edges: {chunk.edgesBuffer.Length}");

                chunk.triangleBuffer.Clear();
                GenerateTriangles(chunk);
               
                Debug.Log($"[DualCounturingSystem]Edges.Lenght: {chunk.triangleBuffer.Length}");
            };
        }

        private void FillVerticeBuffer(ChunkAspect chunk,ref List<IntersectingEdgesElement> edges) 
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

        private void GenerateTriangles(ChunkAspect chunk) 
        {
            for (int i = 0; i < chunk.edgesBuffer.Length; i++)
            {
                List<int> tri = new List<int>();

                GridVertex gridVertex1 = chunk.GridVertexArray[chunk.edgesBuffer[i].edgeData.vertexIndex0];
                GridVertex gridVertex2 = chunk.GridVertexArray[chunk.edgesBuffer[i].edgeData.vertexIndex1];

                /*Debug.Log($"IsBound: " +
                    $"{chunk.edgesBuffer[i].edgeData.vertexIndex0}, {MeshMaths.VertexIsBorder(chunk.GridVertexArray[chunk.edgesBuffer[i].edgeData.vertexIndex0], chunk.Resolution)}, " +
                    $"{chunk.edgesBuffer[i].edgeData.vertexIndex1}, {MeshMaths.VertexIsBorder(chunk.GridVertexArray[chunk.edgesBuffer[i].edgeData.vertexIndex1], chunk.Resolution)}");*/

                // Las aristas que estan incluidas en el bordes del chunk se trataran en otro momento para crear los tris 
                // Que unen los distintos chunks
                if (!(MeshMaths.VertexIsBorder(gridVertex1, chunk.Resolution) && 
                    MeshMaths.VertexIsBorder(gridVertex2, chunk.Resolution)))
                {
                    // Ordenar cells, damos por hecho que vienen ordeandas 
                    Debug.Log($"[DualCounturingSystem]Cells: " +
                        $"{chunk.edgesBuffer[i].edgeData.sharedCells00.index}, " +
                        $"{chunk.edgesBuffer[i].edgeData.sharedCells01.index}, " +
                        $"{chunk.edgesBuffer[i].edgeData.sharedCells10.index}, " +
                        $"{chunk.edgesBuffer[i].edgeData.sharedCells11.index}");

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
                    Debug.Log($"[DualCounturingSystem]Tris a evaluar normal: {normal}");

                    // Creamos los tris segun el axis
                    Debug.Log($"[DualCounturingSystem]Axis: {chunk.edgesBuffer[i].edgeData.axis}");
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

                    Debug.Log($"[DualCounturingSystem]chunk.triangleBuffer.Length = {chunk.triangleBuffer.Length}");
                }
            }
        }
    }
}