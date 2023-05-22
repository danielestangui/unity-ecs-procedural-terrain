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
            EntityQuery chunkEntityQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(ChunkAspect));
            NativeArray<Entity> entityNativeArray = chunkEntityQuery.ToEntityArray(Allocator.Temp);

            Debug.Log($"Numero de chunks: {entityNativeArray.Length}");

            for (int entityIndex = 0; entityIndex < entityNativeArray.Length; entityIndex++)
            {
                ChunkAspect chunk = entityManager.GetAspect<ChunkAspect>(entityNativeArray[entityIndex]);

                // Vertices
                Debug.Log($"Buffer {entityIndex}: Tiene {chunk.verticesBuffer} vertices.");

                List<Vector3> vertice3 = new List<Vector3>();

                foreach (VerticesBuffer item in chunk.verticesBuffer)
                {
                    vertice3.Add(item.vertice.position);
                }

                chunk.verticesBuffer.Clear();

                // Triangles
                int[] triangles = new int[chunk.triangleBuffer.Length];

                for (int triangleIndex = 0; triangleIndex < chunk.triangleBuffer.Length; triangleIndex++)
                {
                    triangles[triangleIndex] = chunk.triangleBuffer[triangleIndex].Value;
                }

                chunk.triangleBuffer.Clear();

                // Mesh
                Mesh mesh = new Mesh();
                mesh.vertices = vertice3.ToArray();
                mesh.triangles = triangles;

                meshFilter.sharedMesh = mesh;

            }

            entityNativeArray.Dispose();
        }
    }
}