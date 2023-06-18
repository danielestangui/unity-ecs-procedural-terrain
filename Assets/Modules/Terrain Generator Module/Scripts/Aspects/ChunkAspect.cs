using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

//[assembly: RegisterGenericComponentType(typeof(TerrainGenerator.ChunkAspect))]

namespace TerrainGenerator
{
    readonly partial struct ChunkAspect : IAspect
    {
        public readonly Entity self;

        private readonly RefRO<LocalTransform> transform;
        private readonly RefRW<ChunkComponent> chunk;

        public readonly DynamicBuffer<GridVertexElement> gridVertexBuffer;
        public readonly DynamicBuffer<CellElement> cellBuffer;
        public readonly DynamicBuffer<VerticesBuffer> verticesBuffer;
        public readonly DynamicBuffer<IntersectingEdgesBuffer> edgesBuffer;
        public readonly DynamicBuffer<TrianglesBuffer> triangleBuffer;

        #region Propierties

        public float3 Position
        {
            get => transform.ValueRO.Position;
        }

        public int Resolution 
        {
            get => chunk.ValueRO.resolution;
        }

        public float Size 
        {
            get => chunk.ValueRO.size;
        }

        public int Depth
        {
            get => chunk.ValueRO.depth;
        }

        public void GetVerticeFromCell(CellElement cell, ref VerticeElement vertex) 
        {
            for (int i = 0; i < verticesBuffer.Length; i++)
            {
                if (verticesBuffer[i].vertice.cell.index == cell.index)
                    vertex = verticesBuffer[i].vertice;
            }
        }

        public GridVertexElement[] GridVertexArray 
        {
            get  
            {
                return gridVertexBuffer.ToNativeArray(Allocator.Temp).ToArray();
            }
            set 
            {
                gridVertexBuffer.Clear();
                foreach (GridVertexElement element in value) 
                {
                    gridVertexBuffer.Add(element);
                }
            } 
        }

        public CellElement[] CellArray 
        {
            get
            {
                return cellBuffer.ToNativeArray(Allocator.Temp).ToArray();
            }
            set
            {
                cellBuffer.Clear();
                foreach (CellElement element in value)
                {
                    cellBuffer.Add(element);
                }
            }
        }
        #endregion
    }
}