using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator 
{
    public static class OctreeLOD
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

        public static float3 GetTargetPosition() 
        {
            return Camera.main.transform.position;
        }

        public static void SplitLeaf(OctreeLeafAspect octreeNode, EntityCommandBuffer ecb)
        {
            ecb.RemoveComponent<OctreeLeafComponent>(octreeNode.self);

            Entity[] childs = new Entity[8];

            float halfSize = octreeNode.Size * 0.5f;
            float quarterSize = halfSize * 0.5f;

            // Create leaves
            for (int childIndex = 0; childIndex < childMap.Length; childIndex++)
            {
                LocalTransform transform = new LocalTransform
                {
                    Position = octreeNode.Position + childMap[childIndex] * quarterSize
                };

                OctreeNodeComponent octreeNodeComponent = new OctreeNodeComponent
                {
                    parent = octreeNode.self,
                    depth = octreeNode.Depth - 1,
                    size = halfSize,
                };

                OctreeLeafComponent octreeLeafComponent = new OctreeLeafComponent
                {
                    value = 1f
                };

                Entity childEntity = ecb.CreateEntity();

                ecb.AddComponent(childEntity, transform);
                ecb.AddComponent(childEntity, octreeNodeComponent);
                ecb.AddComponent(childEntity, octreeLeafComponent);

                //DualContoiring
               /* var chunkComponent = new ChunkComponent
                {
                    resolution = OctreeUtils.depthResolution[octreeNodeComponent.depth],
                    size = halfSize,
                };
*/
                //ecb.AddComponent(childEntity, chunkComponent);
                ecb.AddBuffer<GridVertexElement>(childEntity);
                ecb.AddBuffer<CellElement>(childEntity);
                ecb.AddBuffer<VerticesBuffer>(childEntity);
                ecb.AddBuffer<IntersectingEdgesBuffer>(childEntity);
                ecb.AddBuffer<TrianglesBuffer>(childEntity);

                childs[childIndex] = childEntity;
            }

            ecb.AddComponent(octreeNode.self, new OctreeBranchComponent
            {
                child0 = childs[0],
                child1 = childs[1],
                child2 = childs[2],
                child3 = childs[3],
                child4 = childs[4],
                child5 = childs[5],
                child6 = childs[6],
                child7 = childs[7]
            });
        }
    }
}
