using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Mathematics;
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
        private static bool enable = true;
       
        public void OnCreate(ref SystemState state) 
        {
        }

        public void OnUpdate(ref SystemState state) 
        {
            if (enable) 
            {
                var query = new EntityQueryBuilder(Allocator.Temp).WithAll<ChunkComponent>().Build(state.EntityManager);
                var entities = query.ToEntityArray(Allocator.Temp);

                foreach (var entity in entities) 
                {
                    if (!state.EntityManager.HasComponent<RenderMeshArray>(entity))
                    {
                        CreateMesh(SystemAPI.GetAspect<ChunkAspect>(entity), ref state);
                    }
                    else 
                    {
                        var renderMesh = new Mesh();

                        Vector3[] vertices = new Vector3[chunk.verticesBuffer.Length];
                        Vector3[] normals = new Vector3[chunk.verticesBuffer.Length];
                        int[] triangles = new int[chunk.triangleBuffer.Length];

                        for (int i = 0; i < triangles.Length; i++)
                        {
                            triangles[i] = chunk.triangleBuffer[i].Value;
                        }

                        for (int verticeIndex = 0; verticeIndex < vertices.Length; verticeIndex++)
                        {
                            vertices[verticeIndex] = chunk.verticesBuffer[verticeIndex].vertice.position;
                            normals[verticeIndex] = chunk.verticesBuffer[verticeIndex].vertice.normal;
                        }

                        renderMesh.vertices = vertices;
                        renderMesh.triangles = triangles;
                        renderMesh.normals = normals;

                        var filterSettings = RenderFilterSettings.Default;
                        //filterSettings.RenderingLayerMask = 1;
                        filterSettings.ShadowCastingMode = ShadowCastingMode.Off;
                        filterSettings.ReceiveShadows = false;

                        var renderMeshDescription = new RenderMeshDescription
                        {
                            FilterSettings = filterSettings,
                            LightProbeUsage = LightProbeUsage.Off,
                        };


                        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                        var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
                        var meshArray = new RenderMeshArray(new[] { material }, new[] { renderMesh });


                        //state.EntityManager.SetComponentData(entity,)
                        state.EntityManager.SetComponentData(entity, new RenderBounds { Value = renderMesh.bounds.ToAABB() });
                    }
                    
                }
            }
        }

        private void CreateMesh(ChunkAspect chunk,ref SystemState state) 
        {
            var renderMesh = new Mesh();

            Vector3[] vertices = new Vector3[chunk.verticesBuffer.Length];
            Vector3[] normals = new Vector3[chunk.verticesBuffer.Length];
            int[] triangles = new int[chunk.triangleBuffer.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = chunk.triangleBuffer[i].Value;
            }

            for (int verticeIndex = 0; verticeIndex < vertices.Length; verticeIndex++)
            {
                vertices[verticeIndex] = chunk.verticesBuffer[verticeIndex].vertice.position;
                normals[verticeIndex] = chunk.verticesBuffer[verticeIndex].vertice.normal;
            }

            renderMesh.vertices = vertices;
            renderMesh.triangles = triangles;
            renderMesh.normals = normals;

            var filterSettings = RenderFilterSettings.Default;
            //filterSettings.RenderingLayerMask = 1;
            filterSettings.ShadowCastingMode = ShadowCastingMode.Off;
            filterSettings.ReceiveShadows = false;

            var renderMeshDescription = new RenderMeshDescription
            {
                FilterSettings = filterSettings,
                LightProbeUsage = LightProbeUsage.Off,
            };


            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
            var meshArray = new RenderMeshArray(new[] { material }, new[] { renderMesh });

            RenderMeshUtility.AddComponents(chunk.self, state.EntityManager, renderMeshDescription, meshArray, materialMeshInfo);
            state.EntityManager.SetComponentData(chunk.self, new RenderBounds { Value = renderMesh.bounds.ToAABB() });
        }
    }
}