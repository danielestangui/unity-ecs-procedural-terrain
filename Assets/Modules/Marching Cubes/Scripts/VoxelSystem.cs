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

            Debug.Log("First Update");

            foreach (var (tranform, voxel) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Voxel>>())
            {
                Debug.Log($"{tranform.ValueRO.Position} positon, {voxel.ValueRO.resolution} resolution");

                VerticeBufferElement[] matrix = CreateMatrix(tranform.ValueRO.Position, Mathf.CeilToInt(voxel.ValueRO.resolution));
                UtilsServerLocator.Instance.GetService<GizmoECS>().OnDrawGizmos(() => DrawMatrix(matrix));
            }

            firstUpdate = false;
        }


        private VerticeBufferElement[] CreateMatrix(Vector3 position, int resolution) 
        {

            int matrixLenght = resolution * resolution * resolution;

            VerticeBufferElement[] matrix = new VerticeBufferElement[matrixLenght];

            for (int i = 0; i < matrixLenght; i++)
            {
                VerticeBufferElement vertice = new VerticeBufferElement();
                vertice.position = position + IndexToVector3(i, resolution);
                vertice.value = 1;
                Debug.Log($"Values = {vertice.value}");
                matrix[i] = vertice;
            }

            return matrix;
        }

        private void DrawMatrix(VerticeBufferElement[] matrix) 
        {
            foreach (VerticeBufferElement vertice in matrix) 
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black, vertice.value);
                Gizmos.DrawSphere(vertice.position, 0.1f);
            }
        }

        private Vector3 IndexToVector3(int index, int resolution) 
        {
            int resolution2 = resolution * resolution;

            int x = (index % resolution);
            int y = ((index - x) / resolution) % resolution;
            int z = (index - (y * resolution) - x) / resolution2; 

            Debug.Log($"Vertice{index}: [{x},{y},{z}]");

            return Vector3.right * x + Vector3.up * y + Vector3.forward * z;
        }

        public struct VerticeBufferElement
        {
            public Vector3 position;
            public float value;
        }

    }
}