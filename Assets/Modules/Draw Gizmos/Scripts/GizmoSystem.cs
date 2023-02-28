using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace GizmosNameSpace 
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public struct GizmoSystem : ISystem
    {

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute>();
            MyGizmo.OnDrawGizmos(DrawGizmos);
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
            Debug.Log("Pinta esfera");
            Gizmos.DrawSphere(Vector3.zero, 0.5f);
        }

    }
}