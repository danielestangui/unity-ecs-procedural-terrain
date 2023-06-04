using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

namespace TerrainGenerator
{
    readonly partial struct ChunkAspect : IAspect
    {
        public readonly Entity self;

        private readonly RefRO<LocalTransform> transform;
        private readonly RefRW<ChunkComponent> chunk;

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

        public void GetVerticeFromCell(Cell cell, ref VerticeElement vertex) 
        {
            for (int i = 0; i < verticesBuffer.Length; i++)
            {
                if (verticesBuffer[i].vertice.cell.index == cell.index)
                    vertex = verticesBuffer[i].vertice;
            }
        }

        public GridVertex[] GridVertexArray 
        {
            get => chunk.ValueRO.gridVertexNativeArray.ToArray();
            set => NativeArray<GridVertex>.Copy(value, chunk.ValueRW.gridVertexNativeArray, value.Length);
        }

        public Cell[] CellArray 
        {
            get => chunk.ValueRO.cellNativeArray.ToArray();
            set => NativeArray<Cell>.Copy(value, chunk.ValueRW.cellNativeArray, value.Length);
        }
        #endregion
    }
}