using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TerrainGenerator.Utils;

namespace TerrainGenerator
{
    [BurstCompile]
    public partial struct WorldTerrainSystem : ISystem
    {
        EntityManager entityManager;
        NativeArray<Entity> terrainEntityArray;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            UpdateTerrainEntitiesArray();

            Debug.Log($"<color=cyan>[{this.GetType().ToString()}] Terrain number: {terrainEntityArray.Length}</color>");

            for (int index = 0; index < terrainEntityArray.Length; index++)
            {
                foreach (var node in entityManager.GetBuffer<OctreeNodeBufferElement>(terrainEntityArray[index]))
                {
                    OctreeNode octreeNode = new OctreeNode
                    {
                        currentDepth = 0
                    };

                    entityManager.GetBuffer<OctreeNodeBufferElement>(terrainEntityArray[index]).Add(new OctreeNodeBufferElement
                    {
                        octreeNode = octreeNode
                    });
                }
            }

        }

        private void UpdateTerrainEntitiesArray() 
        {
            EntityQuery query = entityManager.CreateEntityQuery(typeof(ProceduralTerrainComponent));
            terrainEntityArray = query.ToEntityArray(Allocator.Temp);
        }
    }
}