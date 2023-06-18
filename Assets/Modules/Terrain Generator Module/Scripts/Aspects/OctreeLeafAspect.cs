using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator 
{
    // a.k.a. Chunk
    public readonly partial struct OctreeLeafAspect : IAspect
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

        public BlobAssetReference<ResolutionArray> ResolutionBlob
        {
            get => node.ValueRO.resolution;
        }


        public BlobArray<int> ResolutionValues
        {
            get => node.ValueRO.resolution.Value.Values;
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