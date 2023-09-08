using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator 
{
    struct ChunkComponent : IComponentData
    {
        public int resolution;
        public float lenght;
    }

    public struct GridVertexElement : IBufferElementData
    {
        public int index;
        public float3 position;
        public float value;
    }


    public struct CellElement : IBufferElementData
    {
        /*

                6---------7
               /|        /|
              / |       / |
             /  |      /  |
            2---------3   |
            |   4-----|---5
            |  /      |  /
            | /       | /
            |/        |/
            0---------1

        */

        public int index;
        public int corner0;
        public int corner1;
        public int corner2;
        public int corner3;
        public int corner4;
        public int corner5;
        public int corner6;
        public int corner7;

        public int cornersCount;
    }

    /// <summary>
    /// VerticesBuffer stores all the vertices that form the mesh
    /// </summary>
    public struct VerticesBuffer : IBufferElementData
    {
        public VerticeElement vertice;
    }

    public struct VerticeElement
    {
        public int index;
        public float3 position;
        public float3 normal;
        public CellElement cell;
    }

    /// <summary>
    /// TrianglesBuffer stores all the triangles that form the mesh
    /// </summary>
    public struct TrianglesBuffer : IBufferElementData
    {
        public int Value;
    }

    /// <summary>
    /// IntersectingEdgesBuffer stores all intersecting edges. This buffer is used in poligonization fase.
    /// </summary>
    public struct IntersectingEdgesBuffer : IBufferElementData
    {
        public IntersectingEdgesElement edgeData;
    }

    public struct IntersectingEdgesElement
    {
        public int index;
        public int vertexIndex0;
        public int vertexIndex1;

        public int axis;

        // Shared cells between both vertex
        public CellElement sharedCells00;
        public CellElement sharedCells01;
        public CellElement sharedCells10;
        public CellElement sharedCells11;
    }
}
