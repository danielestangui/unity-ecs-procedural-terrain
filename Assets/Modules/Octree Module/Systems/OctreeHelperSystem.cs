using System.Collections;
using System.Collections.Generic;
using TerrainGenerator.Utils;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace OctreeModule
{
    [UpdateInGroup(typeof(OctreeSystemGroup))]
    [UpdateAfter(typeof(OctreeSystem))]
    [BurstCompile]
    public partial struct OctreeHelperSystem : ISystem
    {
        private Color nodeColor;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            nodeColor = Color.yellow;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int leafCount = 0;
            foreach (var leaf in SystemAPI.Query<OctreeLeafAspect>())
            {
                leafCount++;
                Draw.DrawCube(leaf.Position,leaf.Size, nodeColor);
            }

            int branchNode = 0;
            foreach (var leaf in SystemAPI.Query<OctreeNodeAspect>())
            {
                branchNode++;
                Draw.DrawCube(leaf.Position, leaf.Size, nodeColor);
            }

            Debug.Log($"Leaf Count:{leafCount}");
            Debug.Log($"Branch Count:{branchNode}");
        }
    }
}