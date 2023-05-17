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
                Vertex[] vertices =
                    World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<ChunkComponent>(entityNativeArray[i]).vertices.ToArray(); 
                Debug.Log($"Chunk {i}: Tiene {vertices.Length} vertices.");

                if (entityManager.HasComponent<VerticesBuffer>(entityNativeArray[i])) 
                {
                    DynamicBuffer<VerticesBuffer> buffer = entityManager.GetBuffer<VerticesBuffer>(entityNativeArray[i]);

                    Debug.Log($"Buffer {i}: Tiene {buffer.Length} vertices.");

                    buffer.Clear();
                }
            }
            entityNativeArray.Dispose();
        }
    }
}