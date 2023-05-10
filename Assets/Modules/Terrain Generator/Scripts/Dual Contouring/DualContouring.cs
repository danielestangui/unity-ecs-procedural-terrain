using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGenerator 
{
    public static class DualContouring
    {
        public static int MATERIAL_AIR = 0;
        public static int MATERIAL_SOLID = 1;

        public static void CalculatePoint(Vertex[]vertices, Cell cell)
        {
            int corners = 0;

            corners |= (vertices[cell.corner0].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 0;
            corners |= (vertices[cell.corner1].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 1;
            corners |= (vertices[cell.corner2].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 2;
            corners |= (vertices[cell.corner3].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 3;
            corners |= (vertices[cell.corner4].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 4;
            corners |= (vertices[cell.corner5].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 5;
            corners |= (vertices[cell.corner6].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 6;
            corners |= (vertices[cell.corner7].value < 0.0f ? MATERIAL_SOLID : MATERIAL_AIR) << 7;

            if (corners == 0 || corners == 255)
            {
                Debug.Log("No hay colisiones en esta Cell");
            }
        }
    }
}