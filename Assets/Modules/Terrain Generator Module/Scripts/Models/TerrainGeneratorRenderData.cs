using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TerrainGenerator 
{
    [CreateAssetMenu(menuName = "Terrain Generator/Terrain Generator Render Data")]
    public class TerrainGeneratorRenderData : ScriptableObject
    {
        public bool showTerrainGeneratorBoundingBox;
    }
}