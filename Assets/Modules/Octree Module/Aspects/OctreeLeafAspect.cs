using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator 
{
    readonly partial struct OctreeLeafAspect : IAspect
    {
        public readonly Entity self;

        private readonly RefRO<LocalTransform> transform;
        private readonly RefRO<OctreeNodeComponent> node;
        private readonly RefRO<OctreeLeafComponent> leaf;

        public float3 Position
        {
            get => transform.ValueRO.Position;
        }

        public float Size
        {
            get => node.ValueRO.size;
        }

        public int Depth
        {
            get => node.ValueRO.depth;
        }
        public Entity Parent
        {
            get => node.ValueRO.parent;
        }


        public bool IsRoot()
        {
            return node.ValueRO.parent == Entity.Null;
        }
    }
}