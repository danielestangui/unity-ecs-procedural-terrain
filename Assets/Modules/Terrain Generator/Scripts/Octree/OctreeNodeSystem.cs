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
    public partial struct OctreeNodeSystem : ISystem
    {
        EntityManager entityManager;

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
            Camera cam = Camera.current;

            if (cam == null) 
            {
                Debug.Log("No hay niguna camara activa en la escena");
                return;
            }

            foreach (var octreeNode in SystemAPI.Query<OctreeNodeAspect>())
            {
                // Acuta sobre los octree activos
                // Actua sobre el root siempre 
                if (octreeNode.Enable || octreeNode.isRoot()) 
                {
                    float distance = Vector3.Distance(octreeNode.Position, cam.transform.position);

                    if (distance < octreeNode.ActivationDistance) 
                    {
                        Entity child = entityManager.CreateEntity();
                        OctreeNodeComponent octreeComponent = new OctreeNodeComponent
                        {
                            level = (octreeNode.Level - 1),
                            maxLevel = octreeNode.MaxLevel,
                            activationDistance = 0.5f
                        };
                        
                        entityManager.AddComponentData(child, octreeComponent);
                    }

                    Debug.Log($"Node level {octreeNode.Level}");
                }
            };
        }
    }
}