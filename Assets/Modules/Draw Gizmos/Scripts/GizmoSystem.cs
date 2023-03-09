using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Utils;
using Utils.GizmosECS;

namespace GizmosNameSpace 
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public struct GizmoSystem : ISystem
    {

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute>();
            UtilsServerLocator.Instance.GetService<GizmoECS>().OnDrawGizmos(DrawGizmos);
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

        private void DrawGizmos()
        {
            Gizmos.DrawSphere(Vector3.zero, 0.5f);
        }

    }
}