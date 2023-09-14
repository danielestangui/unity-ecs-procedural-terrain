using System.Collections;
using System.Collections.Generic;
using TerrainGenerator.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator 
{
    public static class OctreeUtils
    {

        public static float activationRadious = 3f;

        public static int[] depthResolution;

        public static Color[] depthColorArray = 
            {
            Color.black,
            Color.blue,
            Color.gray,
            Color.green,
            Color.white,
            Color.red,
            Color.cyan,
            Color.yellow,
            Color.magenta
        };

        public static Color GetColor(int index)
        {
            return depthColorArray[index % depthColorArray.Length];
        }

        public static bool CheckActivationVolume(float3 targetPosition, float3 voxelPosition, float voxelSideLenght)
        {
            return MeshMaths.CheckSphereCubeCollision(targetPosition, activationRadious, voxelPosition, voxelSideLenght);
        }

    }
}