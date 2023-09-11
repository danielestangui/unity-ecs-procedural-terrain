using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

namespace TerrainGenerator
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(OctreeSystemGroup), OrderFirst = true)]
    public partial struct GrowBranchesSystem : ISystem
    {
        public void OnCreate(ref SystemState state) {}

        public void OnDestroy(ref SystemState state) {}

        public void OnUpdate(ref SystemState state)
        {
            float3 targetPosition = Camera.main.transform.position;

            EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

            // Creates a new instance of the job, assigns the necessary data, and schedules the job in parallel.
            new GrowBranchesJob
            {
                Ecb = ecb,
                TargetPosition = targetPosition
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

        [BurstCompile]
        public partial struct GrowBranchesJob : IJobEntity 
        {
            public EntityCommandBuffer.ParallelWriter Ecb;
            public float3 TargetPosition;

            // IJobEntity generates a component data query based on the parameters of its `Execute` method.
            // This example queries for all OctreeLeafAspect components and uses `ref` to specify that the
            // operation requires read and write access. Unity processes `Execute` for each entity that
            // matches the component data query.

            private void Execute([ChunkIndexInQuery] int chunkIndex, OctreeLeafAspect leaf) 
            {
                if (leaf.Depth > 0)
                {
                    if (OctreeUtils.CheckActivationVolume(TargetPosition, leaf.Position, leaf.Lenght))
                    {
                        SplitLeaf(chunkIndex,leaf, Ecb, TargetPosition);
                    }
                }
            }

            /// <summary>
            /// Split one leaf into 8 leaves
            /// </summary>
            /// <param name="index"></param>
            /// <param name="octreeLeaf"></param>
            /// <param name="ecb"></param>
            private static void SplitLeaf(int index, OctreeLeafAspect octreeLeaf, EntityCommandBuffer.ParallelWriter ecb, float3 targetPosition)
            {
                ecb.RemoveComponent<OctreeLeafComponent>(index, octreeLeaf.self);
                ecb.RemoveComponent<ChunkComponent>(index, octreeLeaf.self);

                Entity[] childs = new Entity[8];

                int halfLenght = octreeLeaf.Lenght / 2;
                int quarterLenght = halfLenght / 2;

                // Create leaves
                for (int childIndex = 0; childIndex < Octree.childMap.Length; childIndex++)
                {
                    LocalTransform transform = new LocalTransform
                    {
                        Position = octreeLeaf.Position + Octree.childMap[childIndex] * quarterLenght
                    };

                    OctreeNodeComponent octreeNodeComponent = new OctreeNodeComponent
                    {
                        parent = octreeLeaf.self,
                        depth = octreeLeaf.Depth - 1,
                        lenght = halfLenght,
                        resolution = octreeLeaf.Resolution
                    };

                    OctreeLeafComponent octreeLeafComponent = new OctreeLeafComponent
                    {
                    };

                    Entity childEntity = ecb.CreateEntity(index);

                    ecb.AddComponent(index, childEntity, transform);
                    ecb.AddComponent(index, childEntity, octreeNodeComponent);
                    ecb.AddComponent(index, childEntity, octreeLeafComponent);

                    //DualContoiring
                    var chunkComponent = new ChunkComponent
                    {
                        //resolution = octreeNode.ResolutionValues[octreeNodeComponent.depth],
                        dirtyFlag = true,
                        resolution = octreeLeaf.Resolution,
                        lenght = halfLenght
                    };

                    ecb.AddComponent(index, childEntity, chunkComponent);
                    ecb.AddBuffer<GridVertexElement>(index, childEntity);
                    ecb.AddBuffer<CellElement>(index, childEntity);
                    ecb.AddBuffer<VerticesBuffer>(index, childEntity);
                    ecb.AddBuffer<IntersectingEdgesBuffer>(index, childEntity);
                    ecb.AddBuffer<TrianglesBuffer>(index, childEntity);


                    childs[childIndex] = childEntity;
                }

                ecb.AddComponent(index, octreeLeaf.self, new OctreeBranchComponent
                {
                    child0 = childs[0],
                    child1 = childs[1],
                    child2 = childs[2],
                    child3 = childs[3],
                    child4 = childs[4],
                    child5 = childs[5],
                    child6 = childs[6],
                    child7 = childs[7]
                });
            }
        }

    }
}