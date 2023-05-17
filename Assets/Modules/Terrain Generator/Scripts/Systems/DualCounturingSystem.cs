using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TerrainGenerator.Utils;
using Unity.Collections;

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
            foreach (var (buffer, chunk) in SystemAPI.Query<DynamicBuffer<VerticesBuffer>, RefRW<ChunkComponent>>())
            {
                Cell[] cells = chunk.ValueRO.cells.ToArray();
                Vertex[] corner = chunk.ValueRO.vertices.ToArray();

                for (int i = 0; i < cells.Length; i++)
                {
                    Vector3 position = DualContouring.CalculatePoint(i,corner, cells[i]);

                    cells[i].isCrossPoint = position != Vector3.zero;

                    if (position != Vector3.zero) 
                    {
                        cells[i].crossPoint = position;

                        VerticesBuffer element = new VerticesBuffer { Value = position };
                        buffer.Add(element);
                    }
                }

                NativeArray<Cell>.Copy(cells, chunk.ValueRW.cells, cells.Length);
            };
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
            foreach (var (transform, chunk) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<ChunkComponent>>())
            {

                Cell[] cells = chunk.ValueRO.cells.ToArray();

                for (int i = 0; i < cells.Length; i++)
                {

                    if (chunk.ValueRO.cells[i].isCrossPoint) 
                    {
                        float3 position = chunk.ValueRO.cells[i].crossPoint;
                        Draw.DrawSphere(position, 0.1f, Color.red);
                    }    
                }
            };
        }
    }
}