using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using TerrainGenerator.Utils;

namespace TerrainGenerator.Octree
{
    [BurstCompile]
    public partial struct OctreeNodeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, octreeNode) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<OctreeNodeComponent>>())
            {
                //Draw.DrawCube(transform.ValueRO.Position, octreeNode.ValueRO.size, octreeNode.ValueRO.level);
            };
        }
    }
}