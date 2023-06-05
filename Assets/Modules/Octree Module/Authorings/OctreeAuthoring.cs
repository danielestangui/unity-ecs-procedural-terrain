using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[assembly: RegisterGenericComponentType(typeof(DynamicBuffer<OctreeModule.ChildsNodesBuffer>))]

namespace OctreeModule
{
    [AddComponentMenu("Octree/Octree Authoring")]
    public class OctreeAuthoring : MonoBehaviour
    { 
        [SerializeField]
        private float size;
        
        [SerializeField]
        private int maxDepth;

        [SerializeField]
        private float lodDistance;

        public class Baker : Baker<OctreeAuthoring>
        {
            public override void Bake(OctreeAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                OctreeNodeComponent octreeNodeComponent = new OctreeNodeComponent
                {
                    parent = Entity.Null,
                    lodDistance = authoring.lodDistance,
                    size = authoring.size,
                    depth = authoring.maxDepth
                };

                OctreeLeafComponent octreeLeafComponent = new OctreeLeafComponent
                {
                    value = 0f
                };

                AddComponent(entity, octreeNodeComponent);
                AddComponent(entity, octreeLeafComponent);
            }
        }
    }

    public struct OctreeNodeComponent : IComponentData
    {
        public float lodDistance;
        public float size;
        public int depth;
        public Entity parent;
    }

    public struct OctreeBranchComponent : IComponentData
    {
        public DynamicBuffer<ChildsNodesBuffer> childsBuffer;
    }
   
    public struct OctreeLeafComponent : IComponentData 
    {
        public float value;
    }

    public struct ChildsNodesBuffer : IBufferElementData
    {
        public Entity entity;
    }
}