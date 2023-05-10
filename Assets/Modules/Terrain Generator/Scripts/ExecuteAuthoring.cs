using UnityEngine;
using Unity.Entities;

namespace MarchingCube.Test
{
    [AddComponentMenu("MarchingCube/MainThreadExecute")]
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