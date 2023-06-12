using System.Collections;
using System.Collections.Generic;
using TerrainGenerator.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator
{
    [UpdateInGroup(typeof(OctreeSystemGroup))]
    [UpdateAfter(typeof(PruneOctreeSystem))]
    [BurstCompile]
    public partial struct OctreeHelperSystem : ISystem
    {
        private float3 targetPosition;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            targetPosition = float3.zero;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Camera camera = Camera.main;

            if (camera != null)
            {
                targetPosition = camera.transform.position;
            }
/*
            float lodDistance = 5;
            int maxDepth = 2;

            for (int i = 0; i <= maxDepth; i++)
            {
                Draw.DrawCircleSphere(targetPosition, (i + 1) * lodDistance, OctreeUtils.GetColor(i));
            }*/

            int leafCount = 0;
            foreach (var leaf in SystemAPI.Query<OctreeLeafAspect>())
            {
                Draw.DrawCube(leaf.Position,leaf.Size, OctreeUtils.GetColor(0));
                leafCount++;
            }

            int branchNode = 0;
            foreach (var branch in SystemAPI.Query<OctreeNodeAspect>())
            {
                branchNode++;
                //Draw.DrawCube(branch.Position, branch.Size, OctreeUtils.GetColor(branch.Depth));
            }

            //Debug.Log($"Leaf Count:{leafCount}");
            //Debug.Log($"Branch Count:{branchNode}");
        }
    }
}