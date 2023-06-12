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

            Entity singletonRenderEnitity = singletonRenderQuery.GetSingletonEntity();

            if (singletonRenderEnitity != Entity.Null) 
            {
                //UpdateTargtPosition(state.GetComponentDataFromEntity<TerrainGeneratorRenderComponent>(false));
                Debug.Log("Draw");
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
            Draw.DrawCube(position, size, Color.yellow);
        }
    }
}