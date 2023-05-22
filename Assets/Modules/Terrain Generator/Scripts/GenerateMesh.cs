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

            Debug.Log($"Numero de chunks: {entityNativeArray.Length}");

            for (int i = 0; i < entityNativeArray.Length; i++)
            {
                // Vertices
                DynamicBuffer<VerticesBuffer> verticeBuffer = entityManager.GetBuffer<VerticesBuffer>(entityNativeArray[i]);
                Debug.Log($"Buffer {i}: Tiene {verticeBuffer.Length} vertices.");

                Vector3[] vertices = new Vector3[verticeBuffer.Length];

                for (int verticeIndex = 0; verticeIndex < vertices.Length; verticeIndex++)
                {
                    vertices[verticeIndex] = verticeBuffer[verticeIndex].vertice.position;
                }

                // Triangles
                DynamicBuffer<TrianglesBuffer> triangleBuffer = entityManager.GetBuffer<TrianglesBuffer>(entityNativeArray[i]);
                int[] triangles = new int[triangleBuffer.Length];

                for (int triangleIndex = 0; triangleIndex < triangles.Length; triangleIndex++)
                { 
                    triangles[i] = triangleBuffer[triangleIndex].Value;
                }

                // Mesh
                Mesh mesh = new Mesh();

                mesh.vertices = vertices;
                mesh.triangles = triangles;

                meshFilter.sharedMesh = mesh;

                Debug.Log("Mesh Triangles = " + triangleBuffer.Length);
            }

            entityNativeArray.Dispose();
        }
    }
}