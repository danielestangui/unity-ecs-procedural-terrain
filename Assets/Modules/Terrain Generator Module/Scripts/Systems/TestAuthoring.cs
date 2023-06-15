using System.Collections;
using System.Collections.Generic;
using TerrainGenerator;
using Unity.Entities;
using UnityEngine;

public class TestAuthoring : MonoBehaviour
{

    [SerializeField]
    private TerrainGeneratorData data;


    class Baker : Baker<TestAuthoring>
    {

        static ComponentTypeSet componentsToAdd = new(new ComponentType[]
            {
                  typeof(OctreeNodeComponent),
                //typeof(ChunkComponent),
                //typeof(OctreeLeafComponent)
            });


        public override void Bake(TestAuthoring authoring)
        {

            // Add Components
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, componentsToAdd);

            // Set Components
            OctreeNodeComponent octreeNodeComponent = new OctreeNodeComponent
            {
                parent = Entity.Null,
                size = authoring.data.size,
                depth = (authoring.data.depthResolution.Length - 1)
            };

            ChunkComponent chunkComponent = new ChunkComponent
            {
                resolution = authoring.data.depthResolution[octreeNodeComponent.depth],
                size = authoring.data.size,
            };

            OctreeLeafComponent octreeLeafComponent = new OctreeLeafComponent
            {
                value = 0
            };

            //Transform transform = GetComponent<Transform>();

            //SetComponent(entity, LocalTransform.FromPosition(transform.position));
            SetComponent(entity, octreeNodeComponent);
        }
    }


    public struct OctreeNodeComponent : IComponentData
    {
        public float size;
        public int depth;
        public Entity parent;
    }

    public struct OctreeBranchComponent : IComponentData
    {
        public Entity child0;
        public Entity child1;
        public Entity child2;
        public Entity child3;
        public Entity child4;
        public Entity child5;
        public Entity child6;
        public Entity child7;
    }

    public struct OctreeLeafComponent : IComponentData
    {
        public float value;
    }

}
