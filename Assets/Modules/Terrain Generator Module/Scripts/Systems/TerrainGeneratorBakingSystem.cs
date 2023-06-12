using System.Collections;
using System.Collections.Generic;
using TerrainGenerator.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [BurstCompile]
    public partial struct TerrainGeneratorBakingSystem : ISystem
    {
        private float3 targetPosition;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log($"[{this.ToString()}]OnCreate");


        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Debug.Log($"[{this.ToString()}]OnUpdate");
            UpdateTargetPosition();
        }

        private void UpdateTargetPosition() 
        {
            targetPosition = Camera.main.transform.position;
        }
    }
}