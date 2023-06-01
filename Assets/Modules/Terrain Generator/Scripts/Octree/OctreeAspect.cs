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

        public readonly DynamicBuffer<OctreeNodeBufferElement> childBuffer;

        public float3 Position
        {
            get => transform.ValueRO.Position;
        }
    }
}