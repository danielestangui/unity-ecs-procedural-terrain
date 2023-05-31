using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator 
{
    readonly partial struct OctreeNodeAspect : IAspect
    {
        public readonly Entity self;

        private readonly RefRO<LocalTransform> transform;
        private readonly RefRW<OctreeNodeComponent> octreeNode;

        public readonly DynamicBuffer<OctreeNodeBufferElement> childBuffer;

        public float3 Position
        {
            get => transform.ValueRO.Position;
        }

        public bool Enable
        {
            get => octreeNode.ValueRO.enable;
            set => octreeNode.ValueRW.enable = value;
        }

        public int Level 
        {
            get => octreeNode.ValueRO.level;
        }

        public int MaxLevel 
        {
            get => octreeNode.ValueRO.maxLevel;
        }

        public float ActivationDistance 
        {
            get => octreeNode.ValueRO.activationDistance;
        }

        public bool isRoot() 
        {
            return octreeNode.ValueRO.level == octreeNode.ValueRO.maxLevel;
        }
    }
}