using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator 
{
    public static class Octree
    {
        public static readonly float3[] childMap =
       {
            new float3(-1,-1,-1),
            new float3(1,-1,-1),
            new float3(-1,1,-1),
            new float3(1,1,-1),
            new float3(-1,-1,1),
            new float3(1,-1,1),
            new float3(-1,1,1),
            new float3(1,1,1)
        };

        public static float3 GetPOVPostion() 
        { 
            return Camera.main.transform.position;
        }
    }
}
