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

                // Vetrtices Fake
                Vertex[] vertices =
                    World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<ChunkComponent>(entityNativeArray[i]).vertices.ToArray(); 
                Debug.Log($"Chunk {i}: Tiene {vertices.Length} vertices.");

                // Vertices Reales
                DynamicBuffer<VerticesBuffer> buffer = entityManager.GetBuffer<VerticesBuffer>(entityNativeArray[i]);
                Debug.Log($"Buffer {i}: Tiene {buffer.Length} vertices.");

                List<Vector3> vertice3 = new List<Vector3>();

                foreach (VerticesBuffer item in buffer)
                {
                    vertice3.Add(item.vertice.position);
                }

                buffer.Clear();

                // Triangles
                DynamicBuffer<TrianglesBuffer> trianglesbuffer = entityManager.GetBuffer<TrianglesBuffer>(entityNativeArray[i]);
                List<int> trianglesInt = new List<int>();

                foreach (TrianglesBuffer item in trianglesbuffer)
                {
                    trianglesInt.Add(item.Value);
                }
                trianglesbuffer.Clear();

                // Mesh
                Mesh mesh = new Mesh();
                mesh.vertices = vertice3.ToArray();
                mesh.triangles = trianglesInt.ToArray();

                meshFilter.sharedMesh = mesh;

                Debug.Log("Mesh Triangles = " + trianglesbuffer.Length);
            }

            entityNativeArray.Dispose();
        }
    }
}