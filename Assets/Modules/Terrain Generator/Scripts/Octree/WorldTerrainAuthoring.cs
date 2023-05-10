using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator.Octree 
{
    [AddComponentMenu("Terrain Generator/World Terrain")]
    public class WorldTerrainAuthoring : MonoBehaviour
    { 
        [Header("Octree")]
        [SerializeField]
        private int maxLodLevels;
        [SerializeField]
        private float size;

        public class Baker : Baker<WorldTerrainAuthoring>
        {
            public override void Bake(WorldTerrainAuthoring authoring)
            {
                OctreeComponent octreeComponent = new OctreeComponent
                {
                    maxLodLevels = authoring.maxLodLevels
                };

                OctreeNodeComponent octreeNodeComponent = new OctreeNodeComponent
                {
                    size = authoring.size,
                    level = 0
                };

                AddComponent(octreeComponent);
                AddComponent(octreeNodeComponent);
            }
        }
    }

    struct OctreeComponent : IComponentData
    {
        public int maxLodLevels;
    }

    struct OctreeNodeComponent : IComponentData
    {
        public float size;
        public int level;
    }
}