using UnityEngine;
using Unity.Entities;

namespace GizmosNameSpace
{
    /* To ensure that the MainThread systems only run in the MainThread scene,
     the MainThread systems require the existence of an Execute singleton to update. */
    [AddComponentMenu("Gizmos/MainThreadExecute")]
    public class ExecuteAuthoring : MonoBehaviour
    {
        public class Baker : Baker<ExecuteAuthoring>
        {
            public override void Bake(ExecuteAuthoring authoring)
            {
                AddComponent<Execute>();
            }
        }
    }

    public struct Execute : IComponentData
    {
    }
}