#define DEBUG_TerrainGenerator__GridVertex
//#define DEBUG_TerrainGenerator__GridVertexIndex
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
        private const float gizmoSphereRadiusFactor = 0.05f;

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

#if DEBUG_TerrainGenerator__DrawChunkBounds
                DrawBounds(chunk.Position, chunk.Size);
#endif

#if DEBUG_TerrainGenerator__GridVertex
                float gizmoSphereRadius = chunk.Size / chunk.Resolution * gizmoSphereRadiusFactor;
                DrawCorners(chunk.GridVertexArray, gizmoSphereRadius);
#endif

            };
        }

        /// <summary>
        /// Draw a box with the boundaries of the chunk
        /// </summary>
        /// <param name="position"> Chunk pivot postion </param>
        /// <param name="size"> Size of the chunk </param>
        private void DrawBounds(float3 position, float size)
        {
            Draw.DrawCube(position, size, Color.yellow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridVertexArray"></param>
        /// <param name="gizmoSphereRadius"></param>
        private void DrawCorners(GridVertex[] gridVertexArray, float gizmoSphereRadius)
        {

            for (int i = 0; i < gridVertexArray.Length; i++)
            {
                Draw.DrawSphere(gridVertexArray[i].position, gizmoSphereRadius, gridVertexArray[i].value < 0 ? Color.white : Color.black);
#if DEBUG_TerrainGenerator__GridVertexIndex
                float3 gridVertexIndexOffset = new float3(1, 1, 0) * gizmoSphereRadius;
                Draw.DrawText(gridVertexArray[i].position + gridVertexIndexOffset, i.ToString());
# endif
            }
        }

    }
}