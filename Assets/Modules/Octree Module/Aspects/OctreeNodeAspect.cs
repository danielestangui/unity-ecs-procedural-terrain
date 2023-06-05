using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace OctreeModule
{
    readonly partial struct OctreeNodeAspect : IAspect
    {
        public readonly Entity self;

        private readonly RefRO<LocalTransform> transform;
        private readonly RefRO<OctreeNodeComponent> node;
        private readonly RefRW<OctreeBranchComponent> branch;

        public float3 Position
        {
            get => transform.ValueRO.Position;
        }

        public float Size 
        {
            get => node.ValueRO.size;
        }

        public float LodDistance
        {
            get => node.ValueRO.lodDistance;
        }

        public int Depth 
        {
            get => node.ValueRO.depth;
        }

        public Entity Parent
        {
            get => node.ValueRO.parent;
        }

        public DynamicBuffer<ChildsNodesBuffer> Childs
        {
            get => branch.ValueRO.childsBuffer;
            set => branch.ValueRW.childsBuffer = value;
        }

        public bool IsRoot()
        {
            return node.ValueRO.parent == Entity.Null;
        }

        public bool HasChilds()
        {
            return branch.ValueRO.childsBuffer.Length > 0;
        }
    }
}