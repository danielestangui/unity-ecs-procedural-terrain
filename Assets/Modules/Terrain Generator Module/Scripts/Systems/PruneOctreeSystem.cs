//#define DEBUG_PruneOctreeSystem__Verbose

using System.Collections;
using System.Collections.Generic;
using TerrainGenerator;
using TerrainGenerator.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator 
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(TerrainGenerationSystemGroup))]
    [UpdateAfter(typeof(OctreeSystem))]
    public partial class PruneOctreeSystem : SystemBase
    {
        private EntityManager entityManager;
        private float3 targetPosition;

        protected override void OnCreate()
        {
#if DEBUG_PruneOctreeSystem__Verbose
            Debug.Log($"[{this.ToString()}] OnCreate");
#endif

            targetPosition = float3.zero;
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
#if DEBUG_PruneOctreeSystem__Verbose
            Debug.Log($"[{this.ToString()}] OnUpdate");
#endif
           targetPosition = OctreeLOD.GetTargetPosition();

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
                        OctreeLeafAspect childLeaf = SystemAPI.GetAspect<OctreeLeafAspect>(child);

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

                    //DualContoiring
                    var chunkComponent = new ChunkComponent
                    {
                        resolution = node.Resolution[node.Depth],
                        size = node.Size,
                    };

                    ecb.AddComponent(node.self, chunkComponent);
                    ecb.AddBuffer<GridVertexElement>(node.self);
                    ecb.AddBuffer<CellElement>(node.self);
                    ecb.AddBuffer<VerticesBuffer>(node.self);
                    ecb.AddBuffer<IntersectingEdgesBuffer>(node.self);
                    ecb.AddBuffer<TrianglesBuffer>(node.self);

                    foreach (Entity child in node.Childs)
                    {

                        ecb.DestroyEntity(child);
                    }
                }
            }).WithoutBurst().Run();
        }
    }
}

