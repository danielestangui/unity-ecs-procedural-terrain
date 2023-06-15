//#define DEBUG_TerrainGeneratorREnderSystem_Verbose

using System.Collections;
using System.Collections.Generic;
using TerrainGenerator.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator 
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(TerrainGeneratorSystemGroup))]
    [UpdateAfter(typeof(DualContouring))]
    public partial struct TerrainGeneratorRenderSystem : ISystem
    {
        private EntityQuery singletonRenderQuery;

        private float3 targetPosition;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
#if DEBUG_TerrainGeneratorREnderSystem_Verbose
            Debug.Log($"[{this.ToString()}]OnCreate");
#endif
            state.RequireForUpdate<TerrainGeneratorRenderComponent>();

            singletonRenderQuery = state.GetEntityQuery(typeof(TerrainGeneratorRenderComponent));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
#if DEBUG_TerrainGeneratorREnderSystem_Verbose
            Debug.Log($"[{this.ToString()}]OnCreate");
#endif
            // GetSingleton
            Entity singletonRenderEnitity = singletonRenderQuery.GetSingletonEntity();
            TerrainGeneratorRenderComponent renderComponent = state.GetComponentLookup<TerrainGeneratorRenderComponent>(true)[singletonRenderEnitity];

            // Esta acción se tiene que cuando empieza el ciclo de SystenRender para que se vean bien las shapes
            DrawHelper.ClearOnDrawGizmoActions();

            if (singletonRenderEnitity == Entity.Null) 
            {
                return;
            }

            UpdateTargetPosition();

            foreach (var (transform, chunk) in SystemAPI.Query<RefRO<LocalToWorld>, RefRO<ChunkComponent>>())
            {
                if (renderComponent.showTerrainGeneratorBoundingBox) 
                {
                    DrawBounds(transform.ValueRO.Position, chunk.ValueRO.size);
                }
            }

        }

        private void UpdateTargetPosition()
        {
            Camera camera = Camera.main;
            targetPosition = camera.transform.position;
        }

        /// <summary>
        /// Draw a box with the boundaries of the chunk
        /// </summary>
        /// <param name="position"> Chunk pivot postion </param>
        /// <param name="size"> Size of the chunk </param>
        private void DrawBounds(float3 position, float size)
        {
            DrawHelper.DrawCube(position, size, Color.yellow);
        }
    }
}