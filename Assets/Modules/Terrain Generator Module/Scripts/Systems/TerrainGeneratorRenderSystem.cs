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
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct TerrainGeneratorRenderSystem : ISystem
    {
        private EntityQuery singletonRenderQuery;
        private EntityManager entityManager;
        private float3 targetPosition;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log($"[{this.ToString()}]OnCreate");

            state.RequireForUpdate<TerrainGeneratorRenderComponent>();
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            singletonRenderQuery = state.GetEntityQuery(typeof(TerrainGeneratorRenderComponent));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Debug.Log($"[{this.ToString()}]OnUpdate");
            
            // Esta acción se tiene que cuando empieza el ciclo de SystenRender para que se vean bien las shapes
            DrawHelper.ClearOnDrawGizmoActions();

            Entity singletonRenderEnitity = singletonRenderQuery.GetSingletonEntity();

            if (singletonRenderEnitity == Entity.Null) 
            {
                return;
            }

            foreach (var (transform, chunk) in SystemAPI.Query<RefRO<LocalToWorld>, RefRO<ChunkComponent>>())
            {
                DrawBounds(transform.ValueRO.Position, chunk.ValueRO.size);
            }
        }

        private void UpdateTargtPosition(TerrainGeneratorRenderComponent renderComponent) 
        {
            
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