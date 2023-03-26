using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Utils.GizmosECS
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public class GizmoSystem : ComponentSystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            //UtilsServerLocator.Instance.GetService<GizmoECS>().OnDrawGizmos(DrawGizmosSphere);
        }

        public override void Update()
        {
        }

        private void DrawGizmosSphere()
        {
            //Gizmos.DrawSphere(Vector3.zero, 0.5f);
        }
    }
}