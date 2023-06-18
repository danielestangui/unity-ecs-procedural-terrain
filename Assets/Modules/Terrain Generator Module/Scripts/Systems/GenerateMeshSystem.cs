using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace TerrainGenerator 
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(TerrainGenerationSystemGroup))]
    [UpdateAfter(typeof(DualCounturingSystem))]
    public partial struct GenerateMeshSystem : ISystem 
    {
        private static bool enable = false;

        public void OnCreate(ref SystemState state) 
        {
            
        }

        public void OnUpdate(ref SystemState state) 
        {
            Debug.Log("[GenareteMesh] On Update");
            if (enable) 
            {
                // var query = SystemAPI.QueryBuilder().WithAll<ChunkAspect>().Build();
                //var query = SystemAPI.QueryBuilder().WithAspect<ChunkAspect>().WithAll<ChunkComponent>.Build();
                var query = new EntityQueryBuilder(Allocator.Temp).WithAll<ChunkComponent>().Build(state.EntityManager);
                var entities = query.ToEntityArray(Allocator.Temp);

                 foreach (var enitity in entities) 
                 {
                     var chunk = SystemAPI.GetAspect<ChunkAspect>(enitity);

                     var mesh = new Mesh();

                     Vector3[] vertices = new Vector3[chunk.verticesBuffer.Length];
                     Vector3[] normals = new Vector3[chunk.verticesBuffer.Length];

                     for (int verticeIndex = 0; verticeIndex < vertices.Length; verticeIndex++)
                     {
                         vertices[verticeIndex] = chunk.verticesBuffer[verticeIndex].vertice.position;
                         normals[verticeIndex] = chunk.verticesBuffer[verticeIndex].vertice.normal;
                     }

                     Debug.Log("[GenareteMesh] Vertices.Legnht: " + vertices.Length);

                     mesh.vertices = vertices;
                     mesh.normals = vertices;

                     var desc = new RenderMeshDescription(ShadowCastingMode.Off);
                     var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                     var meshArray = new RenderMeshArray(new[] { material }, new[] { mesh });
                     var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);

                     RenderMeshUtility.AddComponents(chunk.self, state.EntityManager, desc, meshArray, materialMeshInfo);

                 }
            }
        }
    }
}