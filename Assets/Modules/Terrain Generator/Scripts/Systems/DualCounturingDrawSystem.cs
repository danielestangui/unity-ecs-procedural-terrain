#define DEBUG_DualContouring__DrawVertex
//#define DEBUG_DualContouring__DrawNormals



using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using TerrainGenerator.Utils;
using Unity.Mathematics;

namespace TerrainGenerator 
{
    [UpdateInGroup(typeof(TerrainGeneratorSystemGroup))]
    [UpdateAfter(typeof(DualCounturingSystem))]
    public partial struct DualCounturingDrawSystem : ISystem
    {
        private const float vertexRadius = 0.1f;
        private const float normalLenght = 0.2f;


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
            Color vertexColor = Color.red;
            Color normalColor = Color.blue;

            foreach (var VerticeBuffer in SystemAPI.Query<DynamicBuffer<VerticesBuffer>>())
            {
                for (int i = 0; i < VerticeBuffer.Length; i++)
                {
#if DEBUG_DualContouring__DrawVertex
                    Draw.DrawSphere(VerticeBuffer[i].vertice.position, vertexRadius, vertexColor);
#endif

#if DEBUG_DualContouring__DrawNormals
                    Draw.DrawLine(buffer[i].vertice.position, (buffer[i].vertice.position + buffer[i].vertice.normal * normalLenght), normalColor);
#endif
                    }
            };
        }
    }
}