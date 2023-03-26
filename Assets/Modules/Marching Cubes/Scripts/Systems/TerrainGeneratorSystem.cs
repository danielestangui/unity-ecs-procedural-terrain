using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TerrainGenerator 
{
    [BurstCompile]
    public partial struct TerrainGeneratorSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, chunk) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ChunkComponent>>()) 
            {
                GenerateTerrainData(transform.ValueRO, ref chunk.ValueRW);
            };
        }

        private void GenerateTerrainData(LocalTransform transform, ref ChunkComponent chunk) 
        {
            float gizmoSphereRadius = 1f / chunk.resolution * 0.3f;

            Vertex[] vertices = chunk.vertices.ToArray();

            for (int i = 0; i < chunk.vertices.Length; i++)
            {
                vertices[i].position = transform.Position + IndexToPosition(i, chunk.resolution);
                vertices[i].value = Noise.PerlinNoise3D.get3DPerlinNoise(vertices[i].position, chunk.frequency) > chunk.threshold;

                Draw.DrawSphere(vertices[i].position, gizmoSphereRadius, vertices[i].value ? Color.white : Color.black);
            }

            NativeArray<Vertex>.Copy(vertices, chunk.vertices, vertices.Length);
        }

        private float3 IndexToPosition(int index, int resolution)
        {
            int resolution2 = resolution * resolution;

            int x = (index % resolution);
            int y = ((index - x) / resolution) % resolution;
            int z = (index - (y * resolution) - x) / resolution2;

            return (new float3(x, y, z)) / (resolution - 1);
        }
    }
}

