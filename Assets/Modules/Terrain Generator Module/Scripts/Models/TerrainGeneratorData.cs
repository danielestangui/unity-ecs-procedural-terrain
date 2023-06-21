using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGenerator 
{
    [CreateAssetMenu(menuName = "Terrain Generator/Terrain Generator Data")]
    public class TerrainGeneratorData : ScriptableObject
    {
        const float minSize = 1f;
        const float maxSize = 1000f;

        [Header("Octree settings")]
        [Range(minSize, maxSize)]
        public float size;

        public ushort[] depthResolution;
    }
}