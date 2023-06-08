using System.Collections;
using System.Collections.Generic;
using TerrainGenerator.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace OctreeModule 
{
    [UpdateInGroup(typeof(OctreeSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(OctreeSystem))]
    public partial class PruneOctreeSystem : SystemBase
    {
        private EntityManager entityManager;
        private float3 targetPosition;

        protected override void OnCreate()
        {
            targetPosition = float3.zero;
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
            Camera camera = Camera.main;

            if (camera != null)
            {
                targetPosition = camera.transform.position;
            }

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            PruneOctreeLeaves(ecb);

            Dependency.Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void PruneOctreeLeaves(EntityCommandBuffer ecb)
        {
            Entities.ForEach((OctreeNodeAspect node) =>
            {
                bool prune = true;

                foreach (Entity child in node.Childs)
                {         
                    if (entityManager.HasComponent<OctreeLeafComponent>(child))
                    {
                        OctreeLeafAspect childLeaf = entityManager.GetAspect<OctreeLeafAspect>(child);

                        if (child != Entity.Null)
                        {
                            if (OctreeUtils.CheckActivationVolume(targetPosition, childLeaf.Position, childLeaf.Size))
                            {
                                prune = false;
                            }
                        }
                    }
                    else
                    {
                        prune = false;
                        continue;
                    }
                }

                if (prune) 
                {
                    ecb.AddComponent<OctreeLeafComponent>(node.self);
                    
                    foreach (Entity child in node.Childs)
                    {

                        ecb.DestroyEntity(child);
                    }
                }
            }).WithoutBurst().Run();
        }
    }
}

