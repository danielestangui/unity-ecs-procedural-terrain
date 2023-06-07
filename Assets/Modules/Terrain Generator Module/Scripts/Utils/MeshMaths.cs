using Unity.Mathematics;
using UnityEngine;

namespace TerrainGenerator.Utils
{
    public static class MeshMaths
    {
        /// <summary>
        /// Transforma la posicion dentro de la matrix tridimensional en la posición del array
        /// </summary>
        /// <param name="position"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static int PositionToIndex(int3 position, int resolution)
        {
            return position.x + position.y * resolution + position.z * resolution * resolution;
        }

        /// <summary>
        /// Transfomra la posición del array en una posicion tridimensional
        /// </summary>
        /// <param name="index"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static int3 IndexToPosition(int index, int resolution)
        {
            int resolution2 = resolution * resolution;

            int x = (index % resolution);
            int y = ((index - x) / resolution) % resolution;
            int z = (index - (y * resolution) - x) / resolution2;

            return new int3(x, y, z);
        }

        public static float3 GetCenterOfCube(float3[] corners) 
        {
            float3 center = Vector3.zero;

            if (corners.Length == 8)
            {
                foreach (float3 corner in corners) 
                {
                    center.x += corner.x;
                    center.y += corner.y;
                    center.z += corner.z;
                }

                center.x /= corners.Length;
                center.y /= corners.Length;
                center.z /= corners.Length;
            }
            else 
            {
                Debug.LogError("Conner number of elements is invalid");
            }

            return center;
        }

        public static bool VertexIsBorder(GridVertex vertex, int resolution)
        {
            int3 position = IndexToPosition(vertex.index, resolution);

            return
                (position.x == 0 || position.x == (resolution - 1)) ||
                (position.y == 0 || position.y == (resolution - 1)) ||
                (position.z == 0 || position.z == (resolution - 1));
        }

        public static bool IsInsideTheCube(float3 point, float3 cubeCenter, float cubeSideLength)
        {
            float halfSideLength = cubeSideLength / 2;

            float minX = cubeCenter.x - halfSideLength;
            float maxX = cubeCenter.x + halfSideLength;
            float minY = cubeCenter.y - halfSideLength;
            float maxY = cubeCenter.y + halfSideLength;
            float minZ = cubeCenter.z - halfSideLength;
            float maxZ = cubeCenter.z + halfSideLength;

            return point.x >= minX && point.x <= maxX &&
                   point.y >= minY && point.y <= maxY &&
                   point.z >= minZ && point.z <= maxZ;
        }

        public static bool CheckSphereCubeCollision(Vector3 sphereCenter, float sphereRadius, Vector3 cubeCenter, float cubeSideLength)
        {
            float halfSideLength = cubeSideLength / 2;

            float distanceX = Mathf.Abs(sphereCenter.x - cubeCenter.x);
            float distanceY = Mathf.Abs(sphereCenter.y - cubeCenter.y);
            float distanceZ = Mathf.Abs(sphereCenter.z - cubeCenter.z);

            float collisionX = distanceX - halfSideLength;
            float collisionY = distanceY - halfSideLength;
            float collisionZ = distanceZ - halfSideLength;

            return collisionX <= sphereRadius &&
                   collisionY <= sphereRadius &&
                   collisionZ <= sphereRadius;
        }
    }
}
