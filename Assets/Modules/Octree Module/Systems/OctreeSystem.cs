using TerrainGenerator.Utils;
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
            Camera camera = Camera.main;
   
            if (camera != null)
            {
                targetPosition = camera.transform.position;
            }

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            UpdateOctreeLeaves(ecb);

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
                    if (OctreeUtils.CheckActivationVolume(targetPosition, leaf.Position, leaf.Size)) 
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

            Entity[] childs = new Entity[8];

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
                };

                OctreeLeafComponent octreeLeafComponent = new OctreeLeafComponent
                {
                    value = 1f
                };

                Entity childEntity = ecb.CreateEntity();

                ecb.AddComponent(childEntity, transform);
                ecb.AddComponent(childEntity, octreeNodeComponent);
                ecb.AddComponent(childEntity, octreeLeafComponent);

                childs[childIndex] = childEntity;
            }

            ecb.AddComponent(octreeNode.self, new OctreeBranchComponent 
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