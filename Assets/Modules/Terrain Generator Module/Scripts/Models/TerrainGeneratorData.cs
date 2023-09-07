using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGenerator 
{
    [CreateAssetMenu(menuName = "Terrain Generator/Terrain Generator Data")]
    public class TerrainGeneratorData : ScriptableObject
    {
        /// <summary>
        /// Chunk lenght values are multiples of 2
        /// </summary>
        private enum ChunkLenghtValidValues
        {
            _8 = 8, 
            _16 = 16, 
            _32 = 32
        }

        [Header("Octree Settings")]
        [TextArea]
        public string labelOctreeLenght;
        

        [SerializeField]
        private ChunkLenghtValidValues _minChunkLenght = ChunkLenghtValidValues._8;

        /// <summary>
        /// How deep can the octree create new branches
        /// </summary>
        [Range(1, 15)]
        public ushort depthResolution;

        [Header("Chunk Settings")]
        [Range(2,6)]
        public int chunkResolution;

        /// <summary>
        /// The smallest chunk has this lenght on each dimension in metters
        /// </summary>
        public int minChunkLenght 
        {
            get => (int)_minChunkLenght;
        }

        /// <summary>
        /// How big can be the mesh inside the octree in one dimension. 
        /// Higher values reduce CPU performace.
        /// </summary>
        public int octreeLenght 
        {
            get => (minChunkLenght * (1 << (depthResolution - 1)));
        }

        private ChunkLenghtValidValues _minChunkLenghtLastValue;
        private ushort _depthResolutionLastValue;

        private void OnValidate()
        {
            if (Application.isEditor) 
            {
                if ((depthResolution != _depthResolutionLastValue) || (_minChunkLenght != _minChunkLenghtLastValue)) 
                {
                    labelOctreeLenght = $"Octree side is {octreeLenght} meters.";

                    _depthResolutionLastValue = depthResolution;
                    _minChunkLenghtLastValue = _minChunkLenght;
                }
            }
        }
    }
}