#define DEBUG_TerrainGenerator__DrawCorners
#define DEBUG_TerrainGenerator__DrawChunkBounds

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TerrainGenerator.Utils;

namespace TerrainGenerator
{
    [UpdateInGroup(typeof(TerrainGeneratorSystemGroup))]
    [UpdateAfter(typeof(TerrainGeneratorSystem))]
    [BurstCompile]
    public partial struct TerrainGeneratorDrawSystem : ISystem
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
#if DEBUG_TerrainGenerator__DrawChunkBounds
                DrawBounds(transform.ValueRO.Position, chunk.ValueRO.size);
#endif
#if DEBUG_TerrainGenerator__DrawCorners
                DrawCorners(transform.ValueRO, chunk.ValueRO);
#endif

            };
        }

        /// <summary>
        /// Dibujas los limites del Chunk
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        private void DrawBounds(float3 position, float size)
        {
            Draw.DrawCube(position, size, Color.yellow);
        }

        private void DrawCorners(LocalTransform transform, ChunkComponent chunk)
        {
            // Draw Corners
            Vertex[] corner = chunk.vertices.ToArray();
            int resolution = chunk.resolution;

            float gizmoSphereRadius = chunk.size / resolution * 0.1f;

            for (int i = 0; i < chunk.vertices.Length; i++)
            {
                Draw.DrawSphere(corner[i].position, gizmoSphereRadius, corner[i].value > chunk.threshold ? Color.white : Color.black);

                float3 textOffset = new float3(1, 1, 0) * gizmoSphereRadius;
                Draw.DrawText(corner[i].position + textOffset, i.ToString());
            }
        }

    }

}