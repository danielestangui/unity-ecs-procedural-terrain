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
        private float3 targetPosition;

        protected override void OnCreate()
        {
            targetPosition = float3.zero;
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
                int currentLOD = OctreeUtils.GetCurrentLOD(targetPosition, node.Position, node.Size);

                if (node.Depth <= currentLOD)
                {
                    ecb.AddComponent<OctreeLeafComponent>(node.self);

                    foreach (Entity child in node.Childs)
                    {
                        ecb.DestroyEntity(child);
                    }

                    ecb.RemoveComponent<OctreeBranchComponent>(node.self);
                }

            }).WithoutBurst().Run();
        }
    }
}

