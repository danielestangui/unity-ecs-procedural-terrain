using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator
{
    [AddComponentMenu("Terrain Generator/World Terrain")]
    public class WorldTerrainAuthoring : MonoBehaviour
    { 
        [Header("Octree")]
        [SerializeField]
        private int maxLodLevels;
        [SerializeField]
        private float size;
        [SerializeField]
        private float distanceToCameraPerEachLOD;

        public class Baker : Baker<WorldTerrainAuthoring>
        {
            public override void Bake(WorldTerrainAuthoring authoring)
            {
                OctreeNodeComponent octreeNodeComponent = new OctreeNodeComponent
                {
                    size = authoring.size,
                    level = authoring.maxLodLevels,
                    maxLevel = authoring.maxLodLevels,
                    activationDistance = authoring.maxLodLevels * authoring.distanceToCameraPerEachLOD
                };

                AddComponent(octreeNodeComponent);
                AddBuffer<OctreeNodeBufferElement>();
            }
        }
    }

    struct OctreeNodeComponent : IComponentData
    {
        public bool enable;
        public float size;
        public int level;
        public int maxLevel;
        public float activationDistance;
        public Entity parent;
    }

    [InternalBufferCapacity(8)]
    public struct OctreeNodeBufferElement : IBufferElementData
    {
        public Entity child;
    }
}