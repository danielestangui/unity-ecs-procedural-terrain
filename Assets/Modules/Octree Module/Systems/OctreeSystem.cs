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

            UpdateOctreeLeaves();
            PruneOctreeLeaves();
            UpdateOctreeBranches();
        }

        private void UpdateOctreeLeaves()
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entities.ForEach((OctreeLeafAspect leaf) =>
            {
                if (leaf.Depth > 0)
                {
                    float distance = math.distance(targetPosition, leaf.Position);
                    float lodDistance = leaf.LodDistance * leaf.Depth;

                    if (distance <= lodDistance)
                    {
                        SplitLeaf(leaf, ecb);
                    }
                }
            }).WithoutBurst().Run();

            Dependency.Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();
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
                    child = childEntity
                });
            }
        }

        private void PruneOctreeLeaves() 
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entities.ForEach((OctreeLeafAspect leaf) =>
            {
                OctreeNodeAspect parentNode = entityManager.GetAspect<OctreeNodeAspect>(leaf.Parent);

                if (!parentNode.IsRoot())
                {
                    float distance = math.distance(targetPosition, parentNode.Position);
                    float parentLodDistance = parentNode.LodDistance * parentNode.Depth;

                    if (distance > parentLodDistance)
                    {
                        CutLeaf(leaf, ecb);
                    }
                }
            }).WithoutBurst().Run();

            Dependency.Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void CutLeaf(OctreeLeafAspect leaf, EntityCommandBuffer ecb) 
        {
            DynamicBuffer<ChildsNodesBuffer> parentBuffer = entityManager.GetBuffer<ChildsNodesBuffer>(leaf.Parent);

            int index = 0;
            int target = 0;
            foreach (ChildsNodesBuffer child in parentBuffer) 
            {
                if (child.child == leaf.self) 
                {
                    target = index;
                }
                index++;

            }

            parentBuffer.RemoveAt(target);

            ecb.DestroyEntity(leaf.self);
        }

        private void UpdateOctreeBranches() 
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entities.ForEach((OctreeNodeAspect leaf) =>
            {
               
            }).WithoutBurst().Run();

            Dependency.Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}