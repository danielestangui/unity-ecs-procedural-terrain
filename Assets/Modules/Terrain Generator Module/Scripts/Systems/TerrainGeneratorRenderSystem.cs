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

        private float3 targetPosition;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log($"[{this.ToString()}]OnCreate");

            state.RequireForUpdate<TerrainGeneratorRenderComponent>();

            singletonRenderQuery = state.GetEntityQuery(typeof(TerrainGeneratorRenderComponent));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Debug.Log($"[{this.ToString()}]OnUpdate");
            // GetSingleton
            Entity singletonRenderEnitity = singletonRenderQuery.GetSingletonEntity();
            TerrainGeneratorRenderComponent renderComponent = state.GetComponentLookup<TerrainGeneratorRenderComponent>(true)[singletonRenderEnitity];

            if (!UpdateTargtPosition(renderComponent)) 
            {
                return;
            }

            // Esta acción se tiene que cuando empieza el ciclo de SystenRender para que se vean bien las shapes
            DrawHelper.ClearOnDrawGizmoActions();

            if (singletonRenderEnitity == Entity.Null) 
            {
                return;
            }

            foreach (var (transform, chunk) in SystemAPI.Query<RefRO<LocalToWorld>, RefRO<ChunkComponent>>())
            {
                if (renderComponent.showTerrainGeneratorBoundingBox) 
                {
                    DrawBounds(transform.ValueRO.Position, chunk.ValueRO.size);
                }
            }
        }

        private bool UpdateTargtPosition(TerrainGeneratorRenderComponent renderComponent) 
        {
            string msg = "";
            Camera camera;

            switch (renderComponent.LODPointOfView) 
            {
                case LODPointOfView.MainCamera:
                    msg += "CamaraMain";
                    camera = Camera.main;
                    break;
                default:
                    msg += "CurrentCamera";
                    camera = Camera.current;
                    break;
            }

            if (camera) 
            {
                targetPosition = camera.transform.position;
            }

            return camera != null;
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