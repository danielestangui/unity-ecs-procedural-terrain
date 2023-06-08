using System.Collections;
using System.Collections.Generic;
using TerrainGenerator.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace OctreeModule 
{
    public static class OctreeUtils
    {
        private static LODData[] LODDataArray = 
        { 
            new LODData(0, 3f, Color.yellow)
            //new LODData(1, 7f, Color.cyan),
            //new LODData(2, 14f, Color.magenta)
        }; 

        public static Color GetColor(int index)
        {
            return LODDataArray[index].color;
        }

        public static int GetCurrentLOD(float3 targetPosition, float3 voxelPosition, float voxelSideLenght)
        {
            int currentLOD = int.MaxValue;

            for (int i = 0; i < LODDataArray.Length; i++)
            {
                if (MeshMaths.CheckSphereCubeCollision(targetPosition, LODDataArray[i].untilDistance,voxelPosition,voxelSideLenght)) 
                {
                    if (i < currentLOD) 
                    {
                        currentLOD = i;
                    }
                }
            }

            return currentLOD;
        }

    }

    public struct LODData
    {
        public int index;
        public float untilDistance;
        public Color color;

        public LODData(int index, float untilDistance, Color color) 
        {
            this.index = index;
            this.untilDistance = untilDistance;
            this.color = Color.red;
        }
    }
}