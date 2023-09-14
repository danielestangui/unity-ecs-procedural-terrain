using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace TerrainGenerator 
{
    public struct OctreeNodeComponent : IComponentData
    {
        public int childIndex;
        public int depth;
        public Entity parent;
        public int resolution;
        public bool isPronable;
        public int lenght;
    }

    public struct OctreeBranchComponent : IComponentData
    {
        public Entity child0;
        public Entity child1;
        public Entity child2;
        public Entity child3;
        public Entity child4;
        public Entity child5;
        public Entity child6;
        public Entity child7;
    }

    public struct OctreeLeafComponent : IComponentData
    {
    }

    public struct ResolutionArray
    {
        public BlobArray<int> Values;
    }
}
