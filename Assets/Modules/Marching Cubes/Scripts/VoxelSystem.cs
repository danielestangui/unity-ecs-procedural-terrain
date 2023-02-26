using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace MarchingCube.Test 
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct VoxelSystem : ISystem 
    {
        public void OnCreate(ref SystemState state) 
        {
            state.RequireForUpdate<Execute>();
        }

        public void OnDestroy(ref SystemState state) 
        { 
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) 
        {
            Create(ref state);
        }

        private void Create(ref SystemState state) 
        {
            foreach (var (tranform, voxel) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Voxel>>())
            {
                Debug.Log($"{tranform.ValueRO.Position} positon, {voxel.ValueRO.resolution} resolution");

                Debug.Log("OnCreate");

            }
        }
    }
}