using System.Collections;
using System.Collections.Generic;
using TerrainGenerator;
using TerrainGenerator.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator 
{
    /// <summary>
    /// TODO: Optimize the way a branch is selected as prunable.
    /// It can't be done in parallel because I can't get with SysteAPI a reference to OctreeLeafAscpet from OctreeNodeAscpect inside IJobEntity
    /// </summary>
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(OctreeSystemGroup))]
    [UpdateAfter(typeof(GrowBranchesSystem))]
    public partial struct PruneBranchesSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

            float3 targetPosition = Camera.main.transform.position;
            int resolution = 0;

            // Mark as prunable
            foreach (var node in SystemAPI.Query<OctreeNodeAspect>())
            {
                bool isPrunable = true;

                foreach (var child in node.Childs)
                {
                    if (SystemAPI.HasComponent<OctreeLeafComponent>(child))
                    {
                        OctreeLeafAspect childLeaf = SystemAPI.GetAspect<OctreeLeafAspect>(child);

                        resolution = childLeaf.Resolution;

                        if (child != Entity.Null)
                        {
                            if (OctreeUtils.CheckActivationVolume(targetPosition, childLeaf.Position, childLeaf.Lenght))
                            {
                                isPrunable = false;
                            }
                        }
                    }
                    else 
                    {
                        isPrunable = false;
                        continue;
                    }
                }

                node.IsPrunable = isPrunable;
            }

            // Creates a new instance of the job, assigns the necessary data, and schedules the job in parallel.
            new PruneLeavesJob
            {
                Ecb = ecb,
                Resolution = resolution
            }.ScheduleParallel();
        }

        /// <summary>
        /// Creates a new EntityCommandBuffer.ParallelWriter
        /// https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/ecs-workflow-optimize-systems.html
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }

        /// <summary>
        /// Transform a Branch into a Leaf deleting its childs
        /// </summary>
        [BurstCompile]
        public partial struct PruneLeavesJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Ecb;
            public int Resolution;

            private void Execute([ChunkIndexInQuery] int chunkIndex, OctreeNodeAspect node)
            {
                if (node.IsPrunable)
                {
                    Ecb.AddComponent<OctreeLeafComponent>(chunkIndex, node.self);

                    //DualContoiring
                    var chunkComponent = new ChunkComponent
                    {
                        dirtyFlag = true,
                        resolution = Resolution,
                        lenght = node.Lenght
                    };

                    Ecb.AddComponent(chunkIndex,node.self, chunkComponent);
                    Ecb.AddBuffer<GridVertexElement>(chunkIndex,node.self);
                    Ecb.AddBuffer<CellElement>(chunkIndex, node.self);
                    Ecb.AddBuffer<VerticesBuffer>(chunkIndex, node.self);
                    Ecb.AddBuffer<IntersectingEdgesBuffer>(chunkIndex, node.self);
                    Ecb.AddBuffer<TrianglesBuffer>(chunkIndex, node.self);

                    foreach (Entity child in node.Childs)
                    {
                        Ecb.DestroyEntity(chunkIndex, child);
                    }
                }
            }
        }
    }
}

