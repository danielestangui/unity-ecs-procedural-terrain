using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace OctreeModule
{
    public partial class OctreeSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(OctreeSystemGroup), OrderFirst = true)]
    public partial class OctreeSystem : SystemBase
    {
        private EntityManager entityManager;

        private float3 targetPosition;

        public static readonly float3[] childMap =
        {
            new float3(-1,-1,-1),
            new float3(1,-1,-1),
            new float3(-1,1,-1),
            new float3(1,1,-1),
            new float3(-1,-1,1),
            new float3(1,-1,1),
            new float3(-1,1,1),
            new float3(1,1,1)
        };

        protected override void OnCreate()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            targetPosition = float3.zero;
        }

        protected override void OnUpdate()
        {
            Camera camera = Camera.current;
   
            if (camera != null)
            {
                targetPosition = camera.transform.position;
            }

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            UpdateOctreeLeaves(ecb);
            PruneOctreeLeaves(ecb);

            Dependency.Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void UpdateOctreeLeaves(EntityCommandBuffer ecb)
        {
            Entities.ForEach((OctreeLeafAspect leaf) =>
            {
                if (leaf.Depth > 0)
                {
                    float distance = math.distance(targetPosition, leaf.Position);
                    int LOD = (int) math.trunc(distance / leaf.LodDistance);

                    if (LOD < leaf.Depth)
                    {
                        SplitLeaf(leaf, ecb);
                    }
                }
            }).WithoutBurst().Run();
        }

    /// <summary>
    /// Conver a leaf into a branch
    /// </summary>
    /// <param name="octreeNode"></param>
    private void SplitLeaf(OctreeLeafAspect octreeNode, EntityCommandBuffer ecb) 
        {
            ecb.RemoveComponent<OctreeLeafComponent>(octreeNode.self);

            DynamicBuffer<ChildsNodesBuffer> childBuffer = ecb.AddBuffer<ChildsNodesBuffer>(octreeNode.self);
            childBuffer.EnsureCapacity(childMap.Length);

            float halfSize = octreeNode.Size * 0.5f;
            float quarterSize = halfSize * 0.5f;

            // Create leaves
            for (int childIndex = 0; childIndex < childMap.Length; childIndex++)
            {
                LocalTransform transform = new LocalTransform
                {
                    Position = octreeNode.Position + childMap[childIndex] * quarterSize
                };

                OctreeNodeComponent octreeNodeComponent = new OctreeNodeComponent
                {
                    parent = octreeNode.self,
                    depth = octreeNode.Depth - 1,
                    size = halfSize,
                    lodDistance = octreeNode.LodDistance - 1
                };

                OctreeLeafComponent octreeLeafComponent = new OctreeLeafComponent
                {
                    value = 1f
                };

                Entity childEntity = ecb.CreateEntity();

                ecb.AddComponent(childEntity, transform);
                ecb.AddComponent(childEntity, octreeNodeComponent);
                ecb.AddComponent(childEntity, octreeLeafComponent);

                childBuffer.Add(new ChildsNodesBuffer
                {
                    entity = childEntity
                });
            }

            ecb.AddComponent(octreeNode.self, new OctreeBranchComponent 
            {
                childsBuffer = childBuffer
            });
        }

        private void PruneOctreeLeaves(EntityCommandBuffer ecb) 
        {
            Entities.ForEach((OctreeNodeAspect node) =>
            {
                float distance = math.distance(targetPosition, node.Position);
                int LOD = (int)math.trunc(distance / node.LodDistance);

                if (LOD > node.Depth)
                {
                    ecb.AddComponent<OctreeLeafComponent>(node.self);

                    foreach (ChildsNodesBuffer child in node.Childs) 
                    {
                        ecb.DestroyEntity(child.entity);
                    }

                    ecb.RemoveComponent<OctreeBranchComponent>(node.self);
                }

            }).WithoutBurst().Run();
        }
    }
}