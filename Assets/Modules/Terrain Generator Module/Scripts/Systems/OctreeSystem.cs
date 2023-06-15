//#define DEBUG_OctreeSystem__Verbose

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    public partial class TerrainGenerationSystemGroup : ComponentSystemGroup
    {
    }

    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(TerrainGenerationSystemGroup), OrderFirst = true)]
    public partial class OctreeSystem : SystemBase
    {
        private EntityManager entityManager;

        private float3 targetPosition;

        protected override void OnCreate()
        {
#if DEBUG_OctreeSystem__Verbose
            Debug.Log($"[{this.ToString()}] OnCreate");
#endif
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            targetPosition = float3.zero;
        }

        protected override void OnUpdate()
        {
#if DEBUG_OctreeSystem__Verbose
            Debug.Log($"[{this.ToString()}] OnUpdate");
#endif
            targetPosition = OctreeLOD.GetTargetPosition();

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
                        OctreeLOD.SplitLeaf(leaf, ecb);
                    }
                }
            }).WithoutBurst().Run();
        }
    }
}