using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator
{
    public readonly partial struct OctreeNodeAspect : IAspect
    {
        public readonly Entity self;

        private readonly RefRO<LocalTransform> transform;
        private readonly RefRW<OctreeNodeComponent> node;
        private readonly RefRW<OctreeBranchComponent> branch;

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

/*        public BlobArray<int> Resolution 
        {
            get => node.ValueRO.resolution.Value.Values;
        }*/

        public Entity Parent
        {
            get => node.ValueRO.parent;
        }

        public Entity[] Childs
        {
            get 
            {
                Entity[] childs =
                {
                    branch.ValueRO.child0,
                    branch.ValueRO.child1,
                    branch.ValueRO.child2,
                    branch.ValueRO.child3,
                    branch.ValueRO.child4,
                    branch.ValueRO.child5,
                    branch.ValueRO.child6,
                    branch.ValueRO.child7
                };

                return childs;
            }
            set 
            {
                branch.ValueRW.child0 = value[0];
                branch.ValueRW.child1 = value[1];
                branch.ValueRW.child2 = value[2];
                branch.ValueRW.child3 = value[3];
                branch.ValueRW.child4 = value[4];
                branch.ValueRW.child5 = value[5];
                branch.ValueRW.child6 = value[6];
                branch.ValueRW.child7 = value[7];

            }
        }

        public bool IsRoot()
        {
            return node.ValueRO.parent == Entity.Null;
        }

        public bool IsPrunable 
        {
            get => node.ValueRO.isPronable;
            set => node.ValueRW.isPronable = value;
        }
    }
}