using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TerrainGenerator 
{
    [CreateAssetMenu(menuName = "Terrain Generator/Terrain Generator Render Data")]
    public class TerrainGeneratorRenderData : ScriptableObject
    {
        public ShowLeafBoundingBox showLeafBoundingBox;
        public ShowGridVertex showGridVertex;
        public ShowCell showCell;
        public ShowVertex showVertex;
        public ShowEdge showEdge;
    }

    [System.Serializable]
    public struct ShowLeafBoundingBox 
    {
        public bool enable;
        [Range(0.1f, 1f)]
        public float thickness;
        public Color color;
    }

    [System.Serializable]
    public struct ShowGridVertex
    {
        public bool enable;
        [Range(0.1f, 1f)]
        public float radius;
        public bool gradientColor;
        public Color airColor;
        public Color groundColor;
    }

    [System.Serializable]
    public struct ShowCell
    {
        public bool enable;
        public bool onlyCellWithVertex;
        [Range(0.1f, 1f)]
        public float thickness;
        public Color color;
    }

    [System.Serializable]
    public struct ShowVertex
    {
        public bool enable;
        [Range(0.1f, 1f)]
        public float radius;
        public Color color;
        public bool enableNormals;
        [Range(0.1f, 1f)]
        public float normalLenght;
        public Color normalColor;
    }

    [System.Serializable]
    public struct ShowEdge
    {
        public bool enable;
        [Range(0.1f, 1f)]
        public float thickness;
        public Color interiorColor;
        public Color borderColor;
    }
}