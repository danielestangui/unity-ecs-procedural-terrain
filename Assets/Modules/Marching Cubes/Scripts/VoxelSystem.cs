using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Utils;
using Utils.GizmosECS;

namespace MarchingCube.Test 
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial class VoxelSystem : SystemBase
    {
        private bool firstUpdate;

        protected override void OnCreate() 
        {
            firstUpdate = true;
        }
        protected override void OnUpdate()
        {
            if (!firstUpdate) 
            {
                return;
            }

            foreach (var (tranform, voxel) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Voxel>>())
            {
                Debug.Log($"{tranform.ValueRO.Position} positon, {voxel.ValueRO.resolution} resolution");

                VerticeBufferElement[] matrix = CreateMatrix(
                        tranform.ValueRO.Position, 
                        Mathf.CeilToInt(voxel.ValueRO.resolution),
                        voxel.ValueRO.frequency,
                        voxel.ValueRO.threshold
                        );
                UtilsServerLocator.Instance.GetService<GizmoECS>().OnDrawGizmos(() => DrawMatrix(matrix));
            }

            firstUpdate = false;
        }


        private VerticeBufferElement[] CreateMatrix(Vector3 position, int resolution, float frequency, float threshold) 
        {

            int matrixLenght = resolution * resolution * resolution;

            VerticeBufferElement[] matrix = new VerticeBufferElement[matrixLenght];

            for (int i = 0; i < matrixLenght; i++)
            {
                VerticeBufferElement vertice = new VerticeBufferElement();
                vertice.position = position + IndexToVector3(i, resolution);
                vertice.value = Noise.PerlinNoise3D.get3DPerlinNoise(vertice.position, frequency) > threshold;
                matrix[i] = vertice;
            }

            return matrix;
        }

        private void DrawMatrix(VerticeBufferElement[] matrix) 
        {
            foreach (VerticeBufferElement vertice in matrix) 
            {
                Gizmos.color = vertice.value ? Color.white : Color.black;
                Gizmos.DrawSphere(vertice.position, 0.02f);
            }
        }

        private int[] GetTriangles(VerticeBufferElement[] matrix, int resolution) 
        {
            List<int> triangles = new List<int>();

            foreach (VerticeBufferElement vertice in matrix) 
            {
                //triangles.AddRange(MarchingCubeLookUpTable.triangulation[VerticeToInt(vertice)]);
            }

            return triangles.ToArray();
        }

        private Vector3 IndexToVector3(int index, int resolution) 
        {
            int resolution2 = resolution * resolution;

            int x = (index % resolution);
            int y = ((index - x) / resolution) % resolution;
            int z = (index - (y * resolution) - x) / resolution2; 

            Debug.Log($"Vertice{index}: [{x},{y},{z}]");

            return (Vector3.right * x + Vector3.up * y + Vector3.forward * z) / resolution ;
        }

        private int PostionToIndex(int x, int y, int z, int resolution) 
        {
            return x + y * resolution + z * resolution * resolution;
        }

        public struct VerticeBufferElement
        {
            public Vector3 position;
            public bool value;
        }

    }
}