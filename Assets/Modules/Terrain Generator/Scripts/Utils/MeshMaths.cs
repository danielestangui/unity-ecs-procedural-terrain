using Unity.Mathematics;

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

    }
}
