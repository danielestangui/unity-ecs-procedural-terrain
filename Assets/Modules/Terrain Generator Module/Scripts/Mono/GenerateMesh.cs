using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace TerrainGenerator
{
    [ExecuteAlways]
    public class GenerateMesh : MonoBehaviour
    {
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public Material material;

        private EntityManager entityManager;

        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void LateUpdate()
        {
            EntityQuery chunkEntityQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(ChunkComponent));
            NativeArray<Entity> entityNativeArray = chunkEntityQuery.ToEntityArray(Allocator.Temp);

            //Debug.Log($"[GenerateMesh]Numero de chunks: {entityNativeArray.Length}");

            for (int i = 0; i < entityNativeArray.Length; i++)
            {
                // Vertices
                DynamicBuffer<VerticesBuffer> verticeBuffer = entityManager.GetBuffer<VerticesBuffer>(entityNativeArray[i]);
                //Debug.Log($"[GenerateMesh]Vertex Buffer {i}: Tiene {verticeBuffer.Length} vertices.");

                Vector3[] vertices = new Vector3[verticeBuffer.Length];
                Vector3[] normals = new Vector3[verticeBuffer.Length];

                for (int verticeIndex = 0; verticeIndex < vertices.Length; verticeIndex++)
                {
                    vertices[verticeIndex] = verticeBuffer[verticeIndex].vertice.position;
                    normals[verticeIndex] = verticeBuffer[verticeIndex].vertice.normal;
                }

                // Triangles
                DynamicBuffer<TrianglesBuffer> triangleBuffer = entityManager.GetBuffer<TrianglesBuffer>(entityNativeArray[i]);
                //Debug.Log($"[GenerateMesh]Triangles Buffer {i}: Tiene {triangleBuffer.Length / 3} triangles.");
                int[] triangles = new int[triangleBuffer.Length];

                for (int triangleIndex = 0; triangleIndex < triangles.Length; triangleIndex++)
                { 
                    triangles[triangleIndex] = triangleBuffer[triangleIndex].Value;
                }

                // Mesh
                Mesh mesh = new Mesh();

                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.normals = normals;
                mesh.RecalculateNormals();

                meshFilter.sharedMesh = mesh;

                string meshTrianglesvalues = "";
                foreach (int triangleValue in mesh.triangles)
                {
                    meshTrianglesvalues += triangleValue + ", ";
                }

                //Debug.Log($"[GenerateMesh]Mesh Triangles ({mesh.triangles.Length}) --> {meshTrianglesvalues}");
            }

            entityNativeArray.Dispose();
        }
    }
}